## 1. Core State Command
- [ ] 1.1 Create `src/Core/Model/StateCommands/CompleteToInProgressCommand.cs`
- [ ] 1.2 Register `CompleteToInProgressCommand` in `StateCommandList.GetAllStateCommands()`

## 2. Unit Tests
- [ ] 2.1 Create `src/UnitTests/Core/Model/StateCommands/CompleteToInProgressCommandTests.cs`
- [ ] 2.2 Update `StateCommandListTests.ShouldReturnAllStateCommandsInCorrectOrder` for new command count and ordering

## 3. Integration Tests
- [ ] 3.1 Create `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForReopenTests.cs`

## 4. Acceptance Tests
- [ ] 4.1 Add `ReopenExistingWorkOrder` helper to `AcceptanceTestBase.cs`
- [ ] 4.2 Create `src/AcceptanceTests/WorkOrders/WorkOrderReopenTests.cs`

## 5. MCP Server Update
- [ ] 5.1 Update status-transitions in `ReferenceResources.cs` to include Complete → InProgress
