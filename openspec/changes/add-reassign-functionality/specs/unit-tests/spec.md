## ADDED Requirements

### Requirement: Unit tests for CompleteToAssignedCommand
Unit tests SHALL be added to `src/UnitTests/Core/Model/` for the `CompleteToAssignedCommand`. Tests SHALL follow the existing naming conventions and patterns used in the project (e.g., matching the style of tests for `DraftToAssignedCommand`, `InProgressToCompleteCommand`). Tests SHALL use NUnit 4.x with Shouldly assertions.

#### Scenario: ShouldTransitionFromCompleteToAssigned
- **GIVEN** test method exists in `src/UnitTests/Core/Model/`
- **AND** a work order in Complete status with a creator and assignee
- **WHEN** the `CompleteToAssignedCommand` is executed with a `StateCommandContext`
- **THEN** `workOrder.Status.ShouldBe(WorkOrderStatus.Assigned)` SHALL pass

#### Scenario: ShouldClearCompletedDate
- **GIVEN** a work order in Complete status with a non-null `CompletedDate`
- **WHEN** the `CompleteToAssignedCommand` is executed
- **THEN** `workOrder.CompletedDate.ShouldBeNull()` SHALL pass

#### Scenario: ShouldUpdateAssignedDate
- **GIVEN** a work order in Complete status
- **WHEN** the `CompleteToAssignedCommand` is executed with a `StateCommandContext` containing `CurrentDateTime` of `2026-03-04`
- **THEN** `workOrder.AssignedDate` SHALL equal the `CurrentDateTime` from the context

#### Scenario: ShouldBeValidForCreator
- **GIVEN** a work order in Complete status
- **WHEN** `IsValid()` is called with the work order's creator as the current user
- **THEN** `command.IsValid().ShouldBeTrue()` SHALL pass

#### Scenario: ShouldBeInvalidForNonCreator
- **GIVEN** a work order in Complete status
- **WHEN** `IsValid()` is called with an employee who is not the creator
- **THEN** `command.IsValid().ShouldBeFalse()` SHALL pass

#### Scenario: ShouldBeInvalidForNonCompleteStatus
- **GIVEN** a work order in Draft status (or Assigned, InProgress, Cancelled)
- **WHEN** `IsValid()` is called with the creator
- **THEN** `command.IsValid().ShouldBeFalse()` SHALL pass

#### Scenario: ShouldMatchReassignCommandName
- **GIVEN** a `CompleteToAssignedCommand` instance
- **WHEN** `Matches("Reassign")` is called
- **THEN** it SHALL return `true`

#### Scenario: ShouldNotMatchOtherCommandNames
- **GIVEN** a `CompleteToAssignedCommand` instance
- **WHEN** `Matches("Assign")` is called
- **THEN** it SHALL return `false`

### Requirement: Unit tests for updated CanReassign() method
Unit tests SHALL verify the updated `CanReassign()` method on `WorkOrder` returns the correct values for all statuses.

#### Scenario: CanReassignReturnsTrueForComplete
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.Complete`
- **WHEN** `CanReassign()` is called
- **THEN** `workOrder.CanReassign().ShouldBeTrue()` SHALL pass

#### Scenario: CanReassignReturnsTrueForDraft
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.Draft`
- **WHEN** `CanReassign()` is called
- **THEN** `workOrder.CanReassign().ShouldBeTrue()` SHALL pass

#### Scenario: CanReassignReturnsFalseForAssigned
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.Assigned`
- **WHEN** `CanReassign()` is called
- **THEN** `workOrder.CanReassign().ShouldBeFalse()` SHALL pass

#### Scenario: CanReassignReturnsFalseForInProgress
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.InProgress`
- **WHEN** `CanReassign()` is called
- **THEN** `workOrder.CanReassign().ShouldBeFalse()` SHALL pass

#### Scenario: CanReassignReturnsFalseForCancelled
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.Cancelled`
- **WHEN** `CanReassign()` is called
- **THEN** `workOrder.CanReassign().ShouldBeFalse()` SHALL pass

### Requirement: Unit tests for StateCommandList including CompleteToAssignedCommand
Unit tests SHALL verify that `StateCommandList` includes the new `CompleteToAssignedCommand` and returns it as a valid command for the correct scenarios.

#### Scenario: GetAllStateCommandsIncludesCompleteToAssigned
- **GIVEN** a work order and employee
- **WHEN** `GetAllStateCommands()` is called
- **THEN** the returned array SHALL contain an instance of `CompleteToAssignedCommand`

#### Scenario: GetValidStateCommandsReturnsReassignForCompleteWorkOrderAndCreator
- **GIVEN** a work order in Complete status with a creator
- **WHEN** `GetValidStateCommands()` is called with the creator
- **THEN** the returned array SHALL contain exactly one command with `TransitionVerbPresentTense` of `"Reassign"`

#### Scenario: GetValidStateCommandsExcludesReassignForNonCreator
- **GIVEN** a work order in Complete status
- **WHEN** `GetValidStateCommands()` is called with a non-creator employee
- **THEN** the returned array SHALL NOT contain any command with `TransitionVerbPresentTense` of `"Reassign"`

### Constraints
- Unit tests SHALL use NUnit 4.x with `[Test]` attributes and Shouldly assertions
- Tests SHALL follow the AAA pattern without section comments
- Tests SHALL use the `Stub` prefix for test doubles, consistent with existing conventions
- Tests SHALL be placed in `src/UnitTests/Core/Model/` following the project's test organization
- Tests SHALL NOT require database access or external dependencies
