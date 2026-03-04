## ADDED Requirements

### Requirement: Acceptance test for Unassign button visibility
An acceptance test SHALL verify that the "Unassign" button is visible on the work order manage page when the creator views an Assigned work order.

#### Scenario: ShouldShowUnassignButtonForCreatorOfAssignedWorkOrder
- **GIVEN** test method `ShouldShowUnassignButtonForCreatorOfAssignedWorkOrder` exists in `src/AcceptanceTests/WorkOrders/WorkOrderUnassignTests.cs`
- **AND** the test class extends `AcceptanceTestBase`
- **AND** `[Test, Retry(2)]` attributes are applied
- **WHEN** the test logs in via `LoginAsCurrentUser()`
- **AND** creates a new work order via `CreateAndSaveNewWorkOrder()`
- **AND** navigates to the work order and assigns it via `AssignExistingWorkOrder(order, CurrentUser.UserName)`
- **AND** navigates back to the work order via `ClickWorkOrderNumberFromSearchPage(order)`
- **THEN** `Page.GetByTestId(nameof(WorkOrderManage.Elements.CommandButton) + "Unassign")` SHALL be visible
- **AND** `await Expect(unassignButton).ToBeVisibleAsync()` SHALL pass

### Requirement: Acceptance test for full Unassign workflow
An acceptance test SHALL verify the complete unassign workflow: create a work order, assign it, unassign it, and verify it returns to Draft status with no assignee.

#### Scenario: ShouldUnassignAssignedWorkOrder
- **GIVEN** test method `ShouldUnassignAssignedWorkOrder` exists in `src/AcceptanceTests/WorkOrders/WorkOrderUnassignTests.cs`
- **AND** `[Test, Retry(2)]` attributes are applied
- **WHEN** the test logs in via `LoginAsCurrentUser()`
- **AND** creates a new work order via `CreateAndSaveNewWorkOrder()`
- **AND** navigates to the work order and assigns it via `AssignExistingWorkOrder(order, CurrentUser.UserName)`
- **AND** navigates back to the work order via `ClickWorkOrderNumberFromSearchPage(order)`
- **AND** clicks the "Unassign" button via `Click(nameof(WorkOrderManage.Elements.CommandButton) + "Unassign")`
- **AND** waits for navigation to the search page
- **AND** navigates back to the work order via `ClickWorkOrderNumberFromSearchPage(order)`
- **THEN** `Page.GetByTestId(nameof(WorkOrderManage.Elements.Status))` SHALL have text `"Draft"`
- **AND** the Assignee dropdown SHALL have no value selected (empty string)
- **AND** the work order SHALL be editable (not read-only)

### Requirement: Acceptance test for Unassign then Reassign workflow
An acceptance test SHALL verify that after unassigning, the work order can be reassigned to a different employee.

#### Scenario: ShouldUnassignAndReassignWorkOrder
- **GIVEN** test method `ShouldUnassignAndReassignWorkOrder` exists in `src/AcceptanceTests/WorkOrders/WorkOrderUnassignTests.cs`
- **AND** `[Test, Retry(2)]` attributes are applied
- **WHEN** the test logs in, creates a work order, assigns it, and unassigns it (as in previous scenario)
- **AND** navigates back to the unassigned work order
- **AND** selects a new assignee via `Select(nameof(WorkOrderManage.Elements.Assignee), CurrentUser.UserName)`
- **AND** clicks the "Assign" button via `Click(nameof(WorkOrderManage.Elements.CommandButton) + DraftToAssignedCommand.Name)`
- **AND** navigates back to the work order
- **THEN** `Page.GetByTestId(nameof(WorkOrderManage.Elements.Status))` SHALL have text `"Assigned"`

### Constraints
- Test file SHALL be `src/AcceptanceTests/WorkOrders/WorkOrderUnassignTests.cs`
- Test class SHALL extend `AcceptanceTestBase`
- Tests SHALL use `[Test, Retry(2)]` for resilience against intermittent failures
- Tests SHALL use `LoginAsCurrentUser()` to log in before interacting with work orders
- Tests SHALL locate elements via `Page.GetByTestId(nameof(WorkOrderManage.Elements.*))` — the same pattern as all existing acceptance tests
- Tests SHALL use `Click()`, `Input()`, `Select()` helper methods from `AcceptanceTestBase`
- Tests SHALL use Playwright's `await Expect(locator).ToBeVisibleAsync()` and `ToHaveTextAsync()` for assertions
- Tests SHALL follow the import pattern: `using ClearMeasure.Bootcamp.UI.Shared.Pages;` for `WorkOrderManage.Elements` access
