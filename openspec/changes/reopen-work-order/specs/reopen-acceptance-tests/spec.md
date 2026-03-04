# Reopen Acceptance Tests

## Summary
End-to-end acceptance tests that verify the Reopen button appears for completed work orders and the full reopen workflow functions correctly through the Blazor UI.

## New File: `src/AcceptanceTests/WorkOrders/WorkOrderReopenTests.cs`

Test class `WorkOrderReopenTests` extending `AcceptanceTestBase`.

### Test: `ShouldReopenCompletedWorkOrder`
Attributes: `[Test, Retry(2)]`

**Steps:**
1. `await LoginAsCurrentUser()`
2. Create and save a new work order: `var order = await CreateAndSaveNewWorkOrder()`
3. Navigate to the work order: `order = await ClickWorkOrderNumberFromSearchPage(order)`
4. Assign it to the current user: `order = await AssignExistingWorkOrder(order, CurrentUser.UserName)`
5. Navigate back: `order = await ClickWorkOrderNumberFromSearchPage(order)`
6. Begin the work order: `order = await BeginExistingWorkOrder(order)`
7. Navigate back: `order = await ClickWorkOrderNumberFromSearchPage(order)`
8. Complete the work order: `order = await CompleteExistingWorkOrder(order)`
9. Navigate back: `order = await ClickWorkOrderNumberFromSearchPage(order)`
10. Click the Reopen button: `await Click(nameof(WorkOrderManage.Elements.CommandButton) + CompleteToInProgressCommand.Name)`
11. Wait for page load: `await Page.WaitForLoadStateAsync(LoadState.NetworkIdle)`
12. Navigate back to the work order: `order = await ClickWorkOrderNumberFromSearchPage(order)`
13. Assert the status is "In Progress": `await Expect(Page.GetByTestId(nameof(WorkOrderManage.Elements.Status))).ToHaveTextAsync(WorkOrderStatus.InProgress.FriendlyName)`
14. Assert the work order is not read-only (editable fields should be enabled): `await Expect(Page.GetByTestId(nameof(WorkOrderManage.Elements.Title))).ToBeEnabledAsync()`

## New Helper Method: `AcceptanceTestBase.ReopenExistingWorkOrder`

Add to `src/AcceptanceTests/AcceptanceTestBase.cs` after `CompleteExistingWorkOrder`:

```csharp
protected async Task<WorkOrder> ReopenExistingWorkOrder(WorkOrder order)
{
    var woNumberLocator = Page.GetByTestId(nameof(WorkOrderManage.Elements.WorkOrderNumber));
    await woNumberLocator.WaitForAsync();
    await Expect(woNumberLocator).ToHaveTextAsync(order.Number!);

    await Click(nameof(WorkOrderManage.Elements.CommandButton) + CompleteToInProgressCommand.Name);
    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    await Task.Delay(GetInputDelayMs());
    WorkOrder rehyratedOrder = await Bus.Send(new WorkOrderByNumberQuery(order.Number!)) ?? throw new InvalidOperationException();
    return rehyratedOrder;
}
```

## Constraints
- Follow exact structure of `WorkOrderCompleteTests.cs` and `WorkOrderShelvedTests.cs`
- Use `[Test, Retry(2)]` attributes
- Use `AcceptanceTestBase` helper methods for workflow steps
- Add `using ClearMeasure.Bootcamp.Core.Model.StateCommands;` for `CompleteToInProgressCommand.Name`
- Namespace: `ClearMeasure.Bootcamp.AcceptanceTests.WorkOrders`
