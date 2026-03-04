## ADDED Requirements

### Requirement: Unit tests for AssignedToDraftCommand validation
Unit tests SHALL be added in a new file `src/UnitTests/Core/Model/StateCommands/AssignedToDraftCommandTests.cs` that verify the command's `IsValid()` logic. Tests SHALL follow the naming convention and patterns of existing state command tests (e.g., `AssignedToInProgressCommandTests.cs`, `DraftToAssignedCommandTests.cs`).

#### Scenario: ShouldBeValid
- **GIVEN** test method `ShouldBeValid` exists in `src/UnitTests/Core/Model/StateCommands/AssignedToDraftCommandTests.cs`
- **AND** a work order is created with `Status = WorkOrderStatus.Assigned` and a `Creator` employee
- **WHEN** an `AssignedToDraftCommand` is created with that work order and the Creator as `CurrentUser`
- **THEN** `command.IsValid()` SHALL return `true`

#### Scenario: ShouldNotBeValidInWrongStatus
- **GIVEN** test method `ShouldNotBeValidInWrongStatus` exists in `src/UnitTests/Core/Model/StateCommands/AssignedToDraftCommandTests.cs`
- **AND** a work order is created with `Status = WorkOrderStatus.Draft` and a `Creator` employee
- **WHEN** an `AssignedToDraftCommand` is created with that work order and the Creator as `CurrentUser`
- **THEN** `command.IsValid()` SHALL return `false`

#### Scenario: ShouldNotBeValidWithWrongEmployee
- **GIVEN** test method `ShouldNotBeValidWithWrongEmployee` exists in `src/UnitTests/Core/Model/StateCommands/AssignedToDraftCommandTests.cs`
- **AND** a work order is created with `Status = WorkOrderStatus.Assigned` and a `Creator` employee
- **AND** a different employee is used as `CurrentUser`
- **WHEN** an `AssignedToDraftCommand` is created with that work order and the different employee
- **THEN** `command.IsValid()` SHALL return `false`

### Requirement: Unit test for AssignedToDraftCommand execution
A unit test SHALL verify that executing the command transitions the status and clears the assignee fields.

#### Scenario: ShouldTransitionStateProperly
- **GIVEN** test method `ShouldTransitionStateProperly` exists in `src/UnitTests/Core/Model/StateCommands/AssignedToDraftCommandTests.cs`
- **AND** a work order is in Assigned status with an Assignee and AssignedDate set
- **WHEN** the `AssignedToDraftCommand` is executed with a `StateCommandContext`
- **THEN** `workOrder.Status` SHALL be `WorkOrderStatus.Draft`
- **AND** `workOrder.Assignee` SHALL be `null`
- **AND** `workOrder.AssignedDate` SHALL be `null`

### Requirement: StateCommandList tests updated
The existing `StateCommandListTests` in `src/UnitTests/Core/Services/Impl/StateCommandListTests.cs` SHALL be updated to account for the new `AssignedToDraftCommand`.

#### Scenario: ShouldReturnAllStateCommandsInCorrectOrder includes Unassign
- **GIVEN** the existing test `ShouldReturnAllStateCommandsInCorrectOrder`
- **WHEN** the test asserts the count of commands returned by `GetAllStateCommands()`
- **THEN** the expected count SHALL be incremented by 1 to account for `AssignedToDraftCommand`

#### Scenario: ShouldGetValidMatchingCommands includes Unassign
- **GIVEN** the existing test `ShouldGetValidMatchingCommands` or a new test
- **WHEN** `GetValidStateCommands()` is called with an Assigned work order and the Creator
- **THEN** the returned commands SHALL include one with `TransitionVerbPresentTense == "Unassign"`

### Constraints
- Test file SHALL be `src/UnitTests/Core/Model/StateCommands/AssignedToDraftCommandTests.cs`
- Tests SHALL use NUnit `[TestFixture]` and `[Test]` attributes
- Tests SHALL follow the naming pattern: `ShouldBeValid`, `ShouldNotBeValidInWrongStatus`, `ShouldNotBeValidWithWrongEmployee`, `ShouldTransitionStateProperly`
- Tests SHALL use the same assertion style as existing state command tests (NUnit `Assert.That` or Shouldly)
- Tests SHALL NOT require database access (pure unit tests)
