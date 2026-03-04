# Reopen Unit Tests

## Summary
Unit tests for the `CompleteToInProgressCommand` and updated `StateCommandList` registration.

## New File: `src/UnitTests/Core/Model/StateCommands/CompleteToInProgressCommandTests.cs`

Test class `CompleteToInProgressCommandTests` extending `StateCommandBaseTests`.

### Test: `ShouldNotBeValidInWrongStatus`
- Create a `WorkOrder` with `Status = WorkOrderStatus.Draft`
- Create an `Employee` and set as `Assignee`
- Create `CompleteToInProgressCommand(order, employee)`
- Assert `command.IsValid()` is `false`

### Test: `ShouldNotBeValidWithWrongEmployee`
- Create a `WorkOrder` with `Status = WorkOrderStatus.Complete`
- Create an `Employee` and set as `Assignee`
- Create `CompleteToInProgressCommand(order, new Employee())` — different employee
- Assert `command.IsValid()` is `false`

### Test: `ShouldBeValid`
- Create a `WorkOrder` with `Status = WorkOrderStatus.Complete`
- Create an `Employee` and set as `Assignee`
- Create `CompleteToInProgressCommand(order, employee)`
- Assert `command.IsValid()` is `true`

### Test: `ShouldTransitionStateProperly`
- Create a `WorkOrder` with `Status = WorkOrderStatus.Complete`, `Number = "123"`, `CompletedDate = DateTime.UtcNow`
- Create an `Employee` and set as `Assignee`
- Create `CompleteToInProgressCommand(order, employee)`
- Call `command.Execute(new StateCommandContext())`
- Assert `order.Status` equals `WorkOrderStatus.InProgress`
- Assert `order.CompletedDate` is `null`

### Override: `GetStateCommand`
- Return `new CompleteToInProgressCommand(order, employee)`

## Updated File: `src/UnitTests/Core/Services/StateCommandListTests.cs`

### Test: `ShouldReturnAllStateCommandsInCorrectOrder`
- Update the expected count from `6` to `7`
- Add assertion at index 6: `Assert.That(commands[6], Is.InstanceOf(typeof(CompleteToInProgressCommand)))`
- Shift the `AssignedToCancelledCommand` assertion to index 7 (count becomes 8 if it was 6 before... but actually the new command goes after index 4 `InProgressToCompleteCommand`)

Wait — re-examining the ordering: the spec says to position after `InProgressToCompleteCommand` (index 4) and before `AssignedToCancelledCommand` (index 5). So:
- Index 5 becomes `CompleteToInProgressCommand`
- Index 6 becomes `AssignedToCancelledCommand`
- Total count: `7`

Updated assertions:
```
Assert.That(commands.Length, Is.EqualTo(7));
Assert.That(commands[0], Is.InstanceOf(typeof(SaveDraftCommand)));
Assert.That(commands[1], Is.InstanceOf(typeof(DraftToAssignedCommand)));
Assert.That(commands[2], Is.InstanceOf(typeof(AssignedToInProgressCommand)));
Assert.That(commands[3], Is.InstanceOf(typeof(InProgressToAssignedCommand)));
Assert.That(commands[4], Is.InstanceOf(typeof(InProgressToCompleteCommand)));
Assert.That(commands[5], Is.InstanceOf(typeof(CompleteToInProgressCommand)));
Assert.That(commands[6], Is.InstanceOf(typeof(AssignedToCancelledCommand)));
```

## Constraints
- Use NUnit 4 `Assert.That` syntax (not Shouldly for these tests, matching the existing pattern)
- Follow exact structure of existing state command test classes
- Namespace: `ClearMeasure.Bootcamp.UnitTests.Core.Model.StateCommands`
