## ADDED Requirements

### Requirement: AssignedToDraftCommand state command exists
The system SHALL include a state command `AssignedToDraftCommand` in `src/Core/Model/StateCommands/AssignedToDraftCommand.cs` that extends `StateCommandBase` and transitions a work order from Assigned status to Draft status.

#### Scenario: Command defines correct begin and end statuses
- **GIVEN** an `AssignedToDraftCommand` instance
- **WHEN** `GetBeginStatus()` is called
- **THEN** it SHALL return `WorkOrderStatus.Assigned`
- **AND** `GetEndStatus()` SHALL return `WorkOrderStatus.Draft`

#### Scenario: Command uses "Unassign" as verb
- **GIVEN** an `AssignedToDraftCommand` instance
- **WHEN** `TransitionVerbPresentTense` is accessed
- **THEN** it SHALL return `"Unassign"`
- **AND** `TransitionVerbPastTense` SHALL return `"Unassigned"`

### Requirement: Only the work order creator can unassign
The `AssignedToDraftCommand` SHALL only be valid when the current user is the creator of the work order. The `UserCanExecute` method SHALL return `true` only when `currentUser == WorkOrder.Creator`.

#### Scenario: Creator can execute unassign
- **GIVEN** a work order in Assigned status
- **AND** the current user is the work order's Creator
- **WHEN** `IsValid()` is called on the `AssignedToDraftCommand`
- **THEN** it SHALL return `true`

#### Scenario: Non-creator cannot execute unassign
- **GIVEN** a work order in Assigned status
- **AND** the current user is NOT the work order's Creator (e.g., the Assignee or another employee)
- **WHEN** `IsValid()` is called on the `AssignedToDraftCommand`
- **THEN** it SHALL return `false`

#### Scenario: Command is invalid when work order is not in Assigned status
- **GIVEN** a work order in Draft, InProgress, Complete, or Cancelled status
- **AND** the current user is the work order's Creator
- **WHEN** `IsValid()` is called on the `AssignedToDraftCommand`
- **THEN** it SHALL return `false`

### Requirement: Unassign clears assignee and assigned date
When the `AssignedToDraftCommand` is executed, it SHALL set `WorkOrder.Assignee` to `null` and `WorkOrder.AssignedDate` to `null` before transitioning the status to Draft. This returns the work order to a clean state ready for reassignment.

#### Scenario: Execute clears assignee and date
- **GIVEN** a work order in Assigned status with an Assignee and AssignedDate set
- **WHEN** the `AssignedToDraftCommand` is executed
- **THEN** `WorkOrder.Assignee` SHALL be `null`
- **AND** `WorkOrder.AssignedDate` SHALL be `null`
- **AND** `WorkOrder.Status` SHALL be `WorkOrderStatus.Draft`

#### Scenario: Execute preserves other work order fields
- **GIVEN** a work order in Assigned status with Title, Description, RoomNumber, Creator, and CreatedDate set
- **WHEN** the `AssignedToDraftCommand` is executed
- **THEN** `WorkOrder.Title`, `WorkOrder.Description`, `WorkOrder.RoomNumber`, `WorkOrder.Creator`, and `WorkOrder.CreatedDate` SHALL remain unchanged

### Requirement: Command is registered in StateCommandList
The `AssignedToDraftCommand` SHALL be included in the list returned by `StateCommandList.GetAllStateCommands()` in `src/Core/Services/Impl/StateCommandList.cs`.

#### Scenario: StateCommandList includes AssignedToDraftCommand
- **WHEN** `GetAllStateCommands()` is called with any work order and employee
- **THEN** the returned list SHALL include an instance of `AssignedToDraftCommand`

#### Scenario: Unassign button appears in UI for creator of assigned work order
- **GIVEN** a work order in Assigned status
- **AND** the current user is the Creator
- **WHEN** `GetValidStateCommands()` is called
- **THEN** the returned list SHALL include a command with `TransitionVerbPresentTense` equal to `"Unassign"`

### Constraints
- The command SHALL be a `record` extending `StateCommandBase`, matching the pattern of all existing state commands
- The command SHALL live in `src/Core/Model/StateCommands/` (Core project, no new project references)
- The command SHALL include a `public const string Name = "Unassign"` field matching the pattern of existing commands
- No changes to `WorkOrderManage.razor` or `WorkOrderManage.razor.cs` are needed — the UI dynamically renders buttons for all valid commands
- No database schema changes are needed
