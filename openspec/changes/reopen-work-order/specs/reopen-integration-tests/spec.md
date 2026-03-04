# Reopen Integration Tests

## Summary
Integration test verifying the `StateCommandHandler` can execute the `CompleteToInProgressCommand` against the database, persisting the status change and clearing the completed date.

## New File: `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForReopenTests.cs`

Test class `StateCommandHandlerForReopenTests` extending `IntegratedTestBase`.

### Test: `ShouldReopenWorkOrder`

**Arrange:**
1. Call `new DatabaseTests().Clean()`
2. Create a `WorkOrder` using `Faker<WorkOrder>()` with `Id = Guid.Empty`
3. Create an `Employee` using `Faker<Employee>()`
4. Set `order.Creator = currentUser`, `order.Assignee = currentUser`
5. Set `order.Status = WorkOrderStatus.Complete`
6. Set `order.CompletedDate = DateTime.UtcNow` (to verify it gets cleared)
7. Save the employee and work order to the database via `DbContext`

**Act:**
8. Set `order.Title = "new title"`, `order.Description = "new desc"`, `order.RoomNumber = "new room"`
9. Create `new CompleteToInProgressCommand(order, currentUser)`
10. Simulate remote object via `RemotableRequestTests.SimulateRemoteObject(command)`
11. Resolve `StateCommandHandler` from `TestHost` and call `handler.Handle(remotedCommand)`

**Assert:**
12. Retrieve the work order from a new `DbContext` using `context3.Find<WorkOrder>(result.WorkOrder.Id)`
13. Assert `order.Title` matches
14. Assert `order.Description` matches
15. Assert `order.Creator` equals `currentUser`
16. Assert `order.Assignee` equals `currentUser`
17. Assert `order.Status` equals `WorkOrderStatus.InProgress`
18. Assert `order.CompletedDate` is `null`

## Constraints
- Follow exact structure of `StateCommandHandlerForCompleteTests.cs`
- Use Shouldly assertions (matching existing integration test style)
- Namespace: `ClearMeasure.Bootcamp.IntegrationTests.DataAccess.Handlers`
