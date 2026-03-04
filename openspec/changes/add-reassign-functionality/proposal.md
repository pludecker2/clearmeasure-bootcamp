## Why

When a work order reaches Complete status, there is currently no way to reassign it back to an employee for additional work. The creator of the work order may determine that the work was not fully finished or that follow-up work is needed. Today, the only option would be to create an entirely new work order, losing the history and context of the original. Adding a reassign capability from Complete back to Assigned allows creators to reopen completed work orders and assign them to an employee, preserving continuity.

## What Changes

- Add `CompleteToAssignedCommand` state command in `src/Core/Model/StateCommands/` that transitions a work order from Complete to Assigned status. Only the work order's creator can execute this command.
- Register the new command in `StateCommandList` so it appears as a valid action button on the work order manage page.
- Update `WorkOrder.CanReassign()` to return `true` when the work order is in Complete status, enabling the Assignee dropdown on the UI.
- The "Reassign" button appears on the work order manage page when the creator views a Complete work order, allowing them to select a new assignee and trigger the transition.

## Capabilities

### New Capabilities
- `complete-to-assigned-command`: A state command that transitions a work order from Complete to Assigned status, executable only by the creator.
- `reassign-ui-button`: The work order manage page shows a "Reassign" button with an editable Assignee dropdown when the creator views a Complete work order.

### Modified Capabilities
- `WorkOrder.CanReassign()` is updated to also return `true` for Complete status (currently only returns `true` for Draft).
- `StateCommandList.GetAllStateCommands()` includes the new `CompleteToAssignedCommand`.

## Impact

- **Core** — New `CompleteToAssignedCommand` state command; modified `CanReassign()` method on `WorkOrder`
- **Core** — Modified `StateCommandList` to register the new command
- **UI.Shared** — No structural changes needed; the existing dynamic button rendering in `WorkOrderManage` will automatically show the "Reassign" button because it iterates `ValidCommands`
- **DataAccess** — No changes needed; the existing `StateCommandHandler` handles all `StateCommandBase`-derived commands
- **Database** — No schema changes required
