## ADDED Requirements

### Requirement: Integration test for CompleteToAssigned state command persistence
Integration tests SHALL be added in `src/IntegrationTests/DataAccess/` that verify the `CompleteToAssignedCommand` correctly persists the status transition through the `StateCommandHandler` and EF Core. Tests SHALL follow the existing patterns in the IntegrationTests project: clean the database first with `new DatabaseTests().Clean()`, use `TestHost` for service resolution, and use Shouldly assertions.

#### Scenario: ShouldPersistReassignedWorkOrder
- **GIVEN** test method `ShouldPersistReassignedWorkOrder` exists in `src/IntegrationTests/DataAccess/`
- **AND** the test calls `new DatabaseTests().Clean()` at the start
- **AND** a work order is created in Complete status with a creator, assignee, and `CompletedDate`
- **AND** the work order is persisted to the database
- **WHEN** a `CompleteToAssignedCommand` is sent via `IBus` (or handled through `StateCommandHandler`)
- **THEN** the work order retrieved from the database SHALL have `Status` equal to `WorkOrderStatus.Assigned`
- **AND** `CompletedDate` SHALL be `null`
- **AND** `AssignedDate` SHALL be set to the current date/time

#### Scenario: ShouldPersistNewAssigneeOnReassign
- **GIVEN** test method `ShouldPersistNewAssigneeOnReassign` exists in `src/IntegrationTests/DataAccess/`
- **AND** the test calls `new DatabaseTests().Clean()` at the start
- **AND** a work order is created in Complete status with creator Employee A and assignee Employee B
- **AND** the work order's Assignee is changed to Employee C before executing the command
- **WHEN** a `CompleteToAssignedCommand` is sent via `IBus`
- **THEN** the work order retrieved from the database SHALL have `Assignee` equal to Employee C
- **AND** `Status` SHALL be `WorkOrderStatus.Assigned`

### Constraints
- Integration tests SHALL follow the existing patterns in `src/IntegrationTests/DataAccess/` (e.g., `WorkOrderMappingTests.cs`, `StateCommandHandlerTests.cs`)
- Tests SHALL clean the database at the start using `new DatabaseTests().Clean()`
- Tests SHALL use `TestHost.GetRequiredService<>()` for resolving services
- Tests SHALL use Shouldly assertions (`.ShouldBe()`, `.ShouldBeNull()`)
- Tests SHALL use NUnit 4.x with `[Test]` attributes
- Tests SHALL work with both SQL Server (Docker container) and SQLite fallback
