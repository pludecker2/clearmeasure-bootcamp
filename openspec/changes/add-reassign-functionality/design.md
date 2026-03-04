## Context

The ChurchBulletin system uses a state machine pattern for work order lifecycle management. State transitions are implemented as `StateCommandBase`-derived records in `src/Core/Model/StateCommands/`. Each command defines its begin status, end status, authorization check (`UserCanExecute`), and transition verbs. All commands are processed by a single `StateCommandHandler` in DataAccess that calls `Execute()`, persists via EF Core, and publishes events.

The current state transitions are:
- Draft -> Draft (`SaveDraftCommand`, verb: "Save", by Creator)
- Draft -> Assigned (`DraftToAssignedCommand`, verb: "Assign", by Creator)
- Assigned -> InProgress (`AssignedToInProgressCommand`, verb: "Begin", by Assignee)
- InProgress -> Assigned (`InProgressToAssignedCommand`, verb: "Shelve", by Assignee)
- InProgress -> Complete (`InProgressToCompleteCommand`, verb: "Complete", by Assignee)
- Assigned -> Cancelled (`AssignedToCancelledCommand`, verb: "Cancel", by Creator)

There is no transition out of Complete status. Once a work order is completed, the creator sees it as read-only with no action buttons.

The `WorkOrderManage` page dynamically renders action buttons by iterating `ValidCommands` — the list of `IStateCommand` instances where `IsValid()` returns `true` for the current work order and user. The Assignee dropdown is controlled by `WorkOrder.CanReassign()`, which currently returns `true` only for Draft status.

## Goals / Non-Goals

**Goals:**
- Enable the creator of a completed work order to reassign it, transitioning it from Complete back to Assigned
- Allow the creator to select a (potentially different) assignee when reassigning
- Follow the existing state command pattern — no new handlers, no new UI components
- The reassign button appears automatically via the existing `ValidCommands` rendering in `WorkOrderManage`

**Non-Goals:**
- Reassignment from other statuses (Assigned, InProgress) — those are separate concerns covered by existing "Shelve" and future features
- Changing who can reassign (only the creator can)
- Adding a separate "Reassign" dialog or modal — the existing form with Assignee dropdown and action buttons is sufficient
- Modifying the database schema

## Decisions

### Decision 1: Create a new `CompleteToAssignedCommand` state command

**Rationale:** Following the established pattern, each state transition has its own command record. The new command will define `GetBeginStatus()` as `Complete` and `GetEndStatus()` as `Assigned`, with `UserCanExecute` checking that the current user is the work order's creator. The transition verb is "Reassign".

**Alternatives considered:**
- Reusing `DraftToAssignedCommand` with a modified begin status: Would break the single-responsibility pattern where each command represents exactly one transition.
- A generic "Reassign" command that works from multiple statuses: Over-engineering for the current requirement.

### Decision 2: Update `WorkOrder.CanReassign()` to include Complete status

**Rationale:** The `CanReassign()` method controls whether the Assignee dropdown is enabled in the UI. For the creator to select a new assignee during reassignment from Complete, the dropdown must be editable. The method should return `true` when `Status == WorkOrderStatus.Complete` in addition to `Status == WorkOrderStatus.Draft`.

### Decision 3: Clear `CompletedDate` on reassignment

**Rationale:** When a completed work order is reassigned back to Assigned, the `CompletedDate` should be cleared since the work order is no longer complete. This follows the same pattern as `AssignedToCancelledCommand` which clears `AssignedDate` and `Assignee` when cancelling. The `AssignedDate` should be updated to the current date since this is a new assignment.

### Decision 4: No database migration required

**Rationale:** The work order table already has all necessary columns (`Status`, `Assignee`, `AssignedDate`, `CompletedDate`). The state transition only updates existing column values. No new fields or tables are needed.

### Decision 5: Register the command in `StateCommandList`

**Rationale:** `StateCommandList.GetAllStateCommands()` is the single registry for all state commands. Adding `CompleteToAssignedCommand` here ensures it is automatically included in the `ValidCommands` list on the UI and available through `GetMatchingCommand()` for form submission.

## Risks / Trade-offs

- **[State machine complexity]** Adding a transition out of Complete means Complete is no longer a terminal state. → Mitigation: This is an intentional business requirement. The state diagram should be updated to reflect this.
- **[UI behavior change]** Creators will now see a "Reassign" button on completed work orders instead of read-only text. → Mitigation: This is the desired behavior per the issue. Non-creators will still see the work order as read-only.
- **[Assignee dropdown re-enabled]** The Assignee dropdown becomes editable on Complete work orders for the creator. The creator could reassign to the same person or a different person. → Mitigation: This is correct behavior — the creator should be able to choose who to assign the reopened work order to.

## Open Questions

- Should the reassigned work order preserve the original assignee as a default in the dropdown, or should it clear the assignee selection?
