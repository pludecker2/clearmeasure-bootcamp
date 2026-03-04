## Why

When a work order is assigned to the wrong person or the assignment needs to be reconsidered, the creator currently has no way to undo the assignment without cancelling the work order entirely. The `AssignedToCancelledCommand` clears the assignee but also moves the work order to a terminal Cancelled status, which is destructive. Creators need a non-destructive way to pull a work order back to Draft status so they can reassign it to a different employee.

## What Changes

- New state command `AssignedToDraftCommand` that transitions a work order from Assigned back to Draft status, clearing the assignee and assigned date
- Registration of the new command in `StateCommandList` so it appears as a valid action
- The UI automatically renders an "Unassign" button on the work order manage page when the command is valid (no Razor template changes needed)
- Unit tests for the new state command's validation and execution logic
- Integration tests verifying the command persists correctly through the `StateCommandHandler` and database
- Acceptance tests verifying the "Unassign" button appears for the creator and performs the expected transition in the browser

## Capabilities

### New Capabilities
- `unassign-state-command`: State command that transitions a work order from Assigned to Draft, clearing assignee and assigned date, executable only by the work order's creator
- `unassign-unit-tests`: Unit tests for the `AssignedToDraftCommand` validation logic and execution behavior
- `unassign-integration-tests`: Integration tests verifying the unassign command persists correctly through the handler and database round-trip
- `unassign-acceptance-tests`: Browser-based acceptance tests verifying the "Unassign" button visibility and workflow in the Blazor UI

### Modified Capabilities
<!-- No existing spec-level behavior changes. The unassign command is additive and follows the existing state command pattern. -->

## Impact

- **New file**: `src/Core/Model/StateCommands/AssignedToDraftCommand.cs` — new state command record
- **Modified file**: `src/Core/Services/Impl/StateCommandList.cs` — register the new command in `GetAllStateCommands()`
- **No UI changes**: The Razor template in `WorkOrderManage.razor` dynamically renders buttons for all valid commands; the "Unassign" button will appear automatically
- **No database changes**: No schema migration needed; the command only clears existing fields (Assignee, AssignedDate)
- **No new project references**: The command lives in the Core project, following Onion Architecture
- **Test additions**: New test files in UnitTests, IntegrationTests, and AcceptanceTests projects
