## MODIFIED Requirements

### Requirement: WorkOrder.CanReassign() returns true for Complete status
The `CanReassign()` method in `src/Core/Model/WorkOrder.cs` SHALL return `true` when `Status == WorkOrderStatus.Complete`, in addition to the existing behavior of returning `true` for `Status == WorkOrderStatus.Draft`.

#### Scenario: Complete work order allows reassign
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.Complete`
- **WHEN** `CanReassign()` is called
- **THEN** it SHALL return `true`

#### Scenario: Draft work order still allows reassign
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.Draft`
- **WHEN** `CanReassign()` is called
- **THEN** it SHALL return `true`

#### Scenario: Assigned work order does not allow reassign
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.Assigned`
- **WHEN** `CanReassign()` is called
- **THEN** it SHALL return `false`

#### Scenario: InProgress work order does not allow reassign
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.InProgress`
- **WHEN** `CanReassign()` is called
- **THEN** it SHALL return `false`

#### Scenario: Cancelled work order does not allow reassign
- **GIVEN** a work order with `Status` set to `WorkOrderStatus.Cancelled`
- **WHEN** `CanReassign()` is called
- **THEN** it SHALL return `false`

### Requirement: Reassign button appears on Complete work orders for the creator
The `WorkOrderManage` page in `src/UI.Shared/Pages/WorkOrderManage.razor` SHALL display a "Reassign" action button when the creator of a Complete work order views the page. This happens automatically because:
1. `CompleteToAssignedCommand.IsValid()` returns `true` for the creator of a Complete work order
2. `StateCommandList.GetValidStateCommands()` includes it in the valid commands
3. The existing `@foreach (var command in ValidCommands)` loop in the Razor page renders a button for each valid command

#### Scenario: Creator sees Reassign button on Complete work order
- **GIVEN** a work order in Complete status
- **WHEN** the creator navigates to the work order manage page
- **THEN** a button with text "Reassign" SHALL be visible
- **AND** the `data-testid` attribute SHALL be `"CommandButtonReassign"` (following the pattern `Elements.CommandButton + command.TransitionVerbPresentTense`)

#### Scenario: Non-creator does not see Reassign button
- **GIVEN** a work order in Complete status
- **WHEN** a non-creator employee navigates to the work order manage page
- **THEN** no "Reassign" button SHALL be visible
- **AND** the page SHALL display the read-only message

### Requirement: Assignee dropdown is editable on Complete work orders
The Assignee `InputSelect` dropdown in `WorkOrderManage.razor` SHALL be enabled (not disabled) when the work order is in Complete status and the creator is viewing it. This is controlled by the `disabled="@(Model.WorkOrder != null && !Model.WorkOrder.CanReassign())"` attribute, which will evaluate to `false` (not disabled) because `CanReassign()` returns `true` for Complete status.

#### Scenario: Assignee dropdown is editable for creator on Complete work order
- **GIVEN** a work order in Complete status
- **WHEN** the creator views the work order manage page
- **THEN** the Assignee dropdown SHALL be enabled (not disabled)
- **AND** the creator can select a different assignee

#### Scenario: Creator submits reassign with new assignee
- **GIVEN** the creator is viewing a Complete work order with the Assignee dropdown enabled
- **WHEN** the creator selects a new assignee from the dropdown
- **AND** clicks the "Reassign" button
- **THEN** the work order SHALL transition to Assigned status with the selected assignee
- **AND** the user SHALL be redirected to the work order search page

### Constraints
- No new Razor components or pages SHALL be created — the existing `WorkOrderManage` page handles everything through its dynamic command button rendering
- The `CanReassign()` change is limited to the `WorkOrder` class in Core — no UI-layer changes to the disabled logic are needed
- The Assignee dropdown behavior is controlled entirely by `CanReassign()` — the Razor template does not need modification
