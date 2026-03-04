# Reopen State Command

## Summary
Add a `CompleteToInProgressCommand` that transitions a work order from `Complete` to `InProgress`, allowing the assignee to resume work on a previously completed work order.

## New File: `src/Core/Model/StateCommands/CompleteToInProgressCommand.cs`

- Record inheriting `StateCommandBase(WorkOrder, Employee)`
- `Name` constant: `"Reopen"`
- `TransitionVerbPresentTense`: `"Reopen"`
- `TransitionVerbPastTense`: `"Reopened"`
- `GetBeginStatus()` returns `WorkOrderStatus.Complete`
- `GetEndStatus()` returns `WorkOrderStatus.InProgress`
- `UserCanExecute(Employee currentUser)` returns `true` only when `currentUser == WorkOrder.Assignee`
- Override `Execute(StateCommandContext context)`: set `WorkOrder.CompletedDate = null` before calling `base.Execute(context)` — clearing the completed date since the work order is no longer complete

## Updated File: `src/Core/Services/Impl/StateCommandList.cs`

- Add `new CompleteToInProgressCommand(workOrder, currentUser)` to the commands list in `GetAllStateCommands()`
- Position it after `InProgressToCompleteCommand` and before `AssignedToCancelledCommand` to maintain logical ordering by workflow progression

## Constraints
- Follow the exact same pattern as `InProgressToAssignedCommand` (the existing "reverse" transition)
- The `InProgressToCompleteCommand.Execute()` sets `CompletedDate`; the reverse (`CompleteToInProgressCommand.Execute()`) must clear it
- No changes to `WorkOrderStatus.cs` — the `Complete` and `InProgress` statuses already exist
- No changes to `WorkOrder.cs` — `CompletedDate` is already a nullable `DateTime?`
- No UI Razor changes — the existing `ValidCommands` iteration in `WorkOrderManage.razor` automatically renders command buttons for all valid state commands

## Updated File: `src/McpServer/Resources/ReferenceResources.cs`

- In `GetStatusTransitions()`, replace `Complete = System.Array.Empty<object>()` with a new transition entry:
  ```
  Complete = new[] { new { Command = "CompleteToInProgressCommand", TargetStatus = "InProgress" } }
  ```
