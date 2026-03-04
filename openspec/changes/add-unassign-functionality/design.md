## Context

The ChurchBulletin work order system uses a state machine pattern where each transition is represented by a `StateCommandBase` record in `src/Core/Model/StateCommands/`. Each command defines its begin status, end status, authorization rule (`UserCanExecute`), and optional side effects in `Execute()`. Commands are registered in `StateCommandList.GetAllStateCommands()`, and the UI dynamically renders buttons for all valid commands for the current user.

The existing state transitions are:
- **Draft → Draft** (`SaveDraftCommand`): Creator saves changes
- **Draft → Assigned** (`DraftToAssignedCommand`): Creator assigns to an employee
- **Assigned → InProgress** (`AssignedToInProgressCommand`): Assignee begins work
- **Assigned → Cancelled** (`AssignedToCancelledCommand`): Creator cancels (clears assignee/date)
- **InProgress → Assigned** (`InProgressToAssignedCommand`): Assignee shelves work
- **InProgress → Complete** (`InProgressToCompleteCommand`): Assignee completes work

The `AssignedToCancelledCommand` is the closest analog: it transitions from Assigned, is restricted to the Creator, and clears `Assignee` and `AssignedDate` in its `Execute()` override. The new Unassign command follows the same pattern but transitions to Draft instead of Cancelled.

## Goals / Non-Goals

**Goals:**
- Allow the creator of a work order to unassign it, moving it from Assigned back to Draft status
- Clear the assignee and assigned date when unassigning, so the work order is ready for reassignment
- Follow the existing state command pattern exactly — no architectural changes
- The "Unassign" button appears automatically in the UI for the creator when viewing an Assigned work order

**Non-Goals:**
- Allowing the assignee to unassign themselves (only the creator can unassign)
- Unassigning from InProgress status (that requires shelving first, then unassigning from Assigned)
- Adding any notification or event when unassigning (can be added later if needed)
- Modifying the database schema

## Decisions

### Decision 1: Create `AssignedToDraftCommand` following the `StateCommandBase` pattern

**Rationale:** Every state transition in the system is a record extending `StateCommandBase`. The new command follows the identical pattern: define begin/end status, authorization, verb strings, and optional `Execute()` side effects. This is consistent and requires no framework changes.

### Decision 2: Only the Creator can execute Unassign

**Rationale:** The issue states "the creator of a work order should be able to unassign." This matches the authorization model for `DraftToAssignedCommand` and `AssignedToCancelledCommand`, which are also creator-only actions. The assignee should not be able to unassign themselves — they use "Begin" to take ownership or the work order stays assigned.

### Decision 3: Clear Assignee and AssignedDate on unassign

**Rationale:** Mirroring `AssignedToCancelledCommand.Execute()`, the unassign command should null out both `Assignee` and `AssignedDate`. This returns the work order to a clean Draft state where the creator can select a new assignee and re-assign.

### Decision 4: No UI template changes needed

**Rationale:** The `WorkOrderManage.razor` template iterates over `ValidCommands` and renders a button for each. Registering the new command in `StateCommandList` is sufficient — the "Unassign" button will appear automatically when the command is valid for the current user and work order status.

### Decision 5: Use verb "Unassign" for the command name

**Rationale:** The button text comes from `TransitionVerbPresentTense`. "Unassign" is clear, concise, and describes the action. Past tense "Unassigned" is used for the result message.

## Risks / Trade-offs

- **[Competing transitions from Assigned]** The Assigned status now has three outbound transitions: Begin (assignee), Cancel (creator), and Unassign (creator). The creator will see both "Cancel" and "Unassign" buttons. → Mitigation: These are distinct actions with different semantics (Cancel is terminal; Unassign returns to Draft). Both should be available to the creator.
- **[No confirmation dialog]** Clicking "Unassign" immediately executes the command. → Mitigation: This is consistent with all other command buttons in the system. A confirmation dialog could be added later as a cross-cutting UI concern.

## Open Questions

- Should there be an `IStateTransitionEvent` published when unassigning (e.g., to notify the assignee)? The current implementation omits this for simplicity, matching most other commands.
