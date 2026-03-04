## ADDED Requirements

### Requirement: CompleteToAssignedCommand state command exists
The system SHALL include a new state command `CompleteToAssignedCommand` as a record in `src/Core/Model/StateCommands/CompleteToAssignedCommand.cs` that extends `StateCommandBase`. The command transitions a work order from Complete status to Assigned status.

#### Scenario: Command defines correct begin and end statuses
- **WHEN** `GetBeginStatus()` is called on `CompleteToAssignedCommand`
- **THEN** it SHALL return `WorkOrderStatus.Complete`
- **AND** `GetEndStatus()` SHALL return `WorkOrderStatus.Assigned`

#### Scenario: Command uses "Reassign" as the transition verb
- **WHEN** the command's `TransitionVerbPresentTense` is inspected
- **THEN** it SHALL be `"Reassign"`
- **AND** `TransitionVerbPastTense` SHALL be `"Reassigned"`

### Requirement: Only the work order creator can execute the command
The `CompleteToAssignedCommand` SHALL only allow execution by the work order's creator. The `UserCanExecute` method SHALL return `true` only when the `currentUser` equals the `WorkOrder.Creator`.

#### Scenario: Creator can execute
- **GIVEN** a work order in Complete status with a creator
- **WHEN** `IsValid()` is called with the creator as the current user
- **THEN** it SHALL return `true`

#### Scenario: Non-creator cannot execute
- **GIVEN** a work order in Complete status with a creator
- **WHEN** `IsValid()` is called with a different employee as the current user
- **THEN** it SHALL return `false`

#### Scenario: Assignee who is not creator cannot execute
- **GIVEN** a work order in Complete status where the assignee is different from the creator
- **WHEN** `IsValid()` is called with the assignee as the current user
- **THEN** it SHALL return `false`

### Requirement: Command clears CompletedDate and updates AssignedDate on execution
When the `CompleteToAssignedCommand` is executed, it SHALL clear the `CompletedDate` (set to `null`) and update the `AssignedDate` to the current date/time from the `StateCommandContext`.

#### Scenario: CompletedDate is cleared
- **GIVEN** a work order in Complete status with a non-null `CompletedDate`
- **WHEN** the command is executed
- **THEN** `WorkOrder.CompletedDate` SHALL be `null`

#### Scenario: AssignedDate is updated
- **GIVEN** a work order in Complete status
- **WHEN** the command is executed with a `StateCommandContext` containing a specific `CurrentDateTime`
- **THEN** `WorkOrder.AssignedDate` SHALL equal the `CurrentDateTime` from the context

#### Scenario: Status changes to Assigned
- **GIVEN** a work order in Complete status
- **WHEN** the command is executed
- **THEN** `WorkOrder.Status` SHALL be `WorkOrderStatus.Assigned`

### Requirement: Command is invalid for non-Complete work orders
The `CompleteToAssignedCommand.IsValid()` SHALL return `false` when the work order is not in Complete status, even if the current user is the creator.

#### Scenario: Draft work order
- **GIVEN** a work order in Draft status
- **WHEN** `IsValid()` is called with the creator
- **THEN** it SHALL return `false`

#### Scenario: Assigned work order
- **GIVEN** a work order in Assigned status
- **WHEN** `IsValid()` is called with the creator
- **THEN** it SHALL return `false`

#### Scenario: InProgress work order
- **GIVEN** a work order in InProgress status
- **WHEN** `IsValid()` is called with the creator
- **THEN** it SHALL return `false`

#### Scenario: Cancelled work order
- **GIVEN** a work order in Cancelled status
- **WHEN** `IsValid()` is called with the creator
- **THEN** it SHALL return `false`

## MODIFIED Requirements

### Requirement: StateCommandList includes CompleteToAssignedCommand
The `StateCommandList.GetAllStateCommands()` method in `src/Core/Services/Impl/StateCommandList.cs` SHALL include `CompleteToAssignedCommand` in the list of all state commands.

#### Scenario: CompleteToAssignedCommand is registered
- **WHEN** `GetAllStateCommands()` is called with any work order and employee
- **THEN** the returned array SHALL contain an instance of `CompleteToAssignedCommand`

#### Scenario: Command appears in valid commands for Complete work order viewed by creator
- **GIVEN** a work order in Complete status
- **WHEN** `GetValidStateCommands()` is called with the work order's creator
- **THEN** the returned array SHALL contain exactly one command with `TransitionVerbPresentTense` equal to `"Reassign"`

#### Scenario: Command does not appear for Complete work order viewed by non-creator
- **GIVEN** a work order in Complete status
- **WHEN** `GetValidStateCommands()` is called with an employee who is not the creator
- **THEN** the returned array SHALL NOT contain any command with `TransitionVerbPresentTense` equal to `"Reassign"`

### Constraints
- The command SHALL be a `record` extending `StateCommandBase(WorkOrder, Employee)`, following the same pattern as all existing state commands
- The command file SHALL be located at `src/Core/Model/StateCommands/CompleteToAssignedCommand.cs`
- The command SHALL define a `public const string Name = "Reassign"` constant, consistent with other commands
- The command SHALL NOT introduce any new project references to Core
- The existing `StateCommandHandler` in DataAccess SHALL handle this command without modification
