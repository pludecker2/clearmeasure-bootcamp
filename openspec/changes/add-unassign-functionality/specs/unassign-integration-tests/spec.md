## ADDED Requirements

### Requirement: Integration test for unassign command persistence
Integration tests SHALL be added that verify the `AssignedToDraftCommand` persists correctly through the `StateCommandHandler` and database round-trip. Tests SHALL follow the patterns in `StateCommandHandlerForSaveTests.cs`: clean the database, create employees and work orders via `DbContext`, execute the command through `StateCommandHandler`, and verify the result by querying the database.

#### Scenario: ShouldUnassignWorkOrder
- **GIVEN** test method `ShouldUnassignWorkOrder` exists in `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForUnassignTests.cs`
- **AND** the test calls `new DatabaseTests().Clean()` at the start
- **AND** a Creator employee and an Assignee employee are saved to the database
- **AND** a work order is created with the Creator, Assignee, and `Status = WorkOrderStatus.Assigned`
- **AND** the work order is saved to the database
- **WHEN** an `AssignedToDraftCommand` is created with the work order and Creator as `CurrentUser`
- **AND** the command is passed through `RemotableRequestTests.SimulateRemoteObject()` to test serialization
- **AND** `StateCommandHandler.Handle()` is called with the command
- **THEN** the result's `WorkOrder.Status` SHALL be `WorkOrderStatus.Draft`
- **AND** the result's `WorkOrder.Assignee` SHALL be `null`
- **AND** when the work order is retrieved from the database using a fresh `DbContext`
- **THEN** the rehydrated work order's `Status` SHALL be `WorkOrderStatus.Draft`
- **AND** the rehydrated work order's `Assignee` SHALL be `null`
- **AND** the rehydrated work order's `AssignedDate` SHALL be `null`
- **AND** the rehydrated work order's `Creator` SHALL match the original Creator
- **AND** the rehydrated work order's `Title` and `Description` SHALL be preserved

#### Scenario: ShouldUnassignAndReassignWorkOrder
- **GIVEN** test method `ShouldUnassignAndReassignWorkOrder` exists in `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForUnassignTests.cs`
- **AND** a work order is created and assigned as in the previous scenario
- **WHEN** the work order is unassigned via `AssignedToDraftCommand`
- **AND** a new assignee is set on the work order
- **AND** a `DraftToAssignedCommand` is executed to reassign
- **THEN** the work order's `Status` SHALL be `WorkOrderStatus.Assigned`
- **AND** the work order's `Assignee` SHALL be the new assignee

### Constraints
- Test file SHALL be `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForUnassignTests.cs`
- Test class SHALL extend `IntegratedTestBase`
- Tests SHALL use `[Test]` and `async Task` patterns matching existing handler test files
- Tests SHALL use `TestHost.GetRequiredService<StateCommandHandler>()` to get the handler
- Tests SHALL use `TestHost.GetRequiredService<DbContext>()` for database operations
- Tests SHALL use Shouldly assertions (`ShouldBe`, `ShouldBeNull`, `ShouldNotBeNull`)
- Tests SHALL use `RemotableRequestTests.SimulateRemoteObject()` to verify serialization round-trip
