## 1. Unassign State Command

- [ ] 1.1 Create `src/Core/Model/StateCommands/AssignedToDraftCommand.cs` extending `StateCommandBase` with begin status `Assigned`, end status `Draft`, verb "Unassign", and creator-only authorization
- [ ] 1.2 Override `Execute()` to clear `WorkOrder.Assignee` and `WorkOrder.AssignedDate` before calling `base.Execute()`
- [ ] 1.3 Register `AssignedToDraftCommand` in `StateCommandList.GetAllStateCommands()` in `src/Core/Services/Impl/StateCommandList.cs`

## 2. Unit Tests

- [ ] 2.1 Create `src/UnitTests/Core/Model/StateCommands/AssignedToDraftCommandTests.cs` with tests following the pattern in existing state command test files
- [ ] 2.2 Test `ShouldBeValid` — command is valid when work order is Assigned and current user is Creator
- [ ] 2.3 Test `ShouldNotBeValidInWrongStatus` — command is invalid when work order is not in Assigned status
- [ ] 2.4 Test `ShouldNotBeValidWithWrongEmployee` — command is invalid when current user is not the Creator
- [ ] 2.5 Test `ShouldTransitionStateProperly` — executing the command changes status to Draft
- [ ] 2.6 Test that `Execute()` clears Assignee and AssignedDate
- [ ] 2.7 Update `StateCommandListTests` to include `AssignedToDraftCommand` in the expected command list and verify it returns as a valid command for the correct scenario

## 3. Integration Tests

- [ ] 3.1 Create or add to integration test file for unassign handler tests in `src/IntegrationTests/DataAccess/Handlers/`
- [ ] 3.2 Test that unassigning a work order persists Draft status, null Assignee, and null AssignedDate through the `StateCommandHandler` and database round-trip
- [ ] 3.3 Test that unassigning preserves the Creator, Title, Description, and other work order fields

## 4. Acceptance Tests

- [ ] 4.1 Create `src/AcceptanceTests/WorkOrders/WorkOrderUnassignTests.cs` extending `AcceptanceTestBase`
- [ ] 4.2 Test that the "Unassign" button is visible when the creator views an Assigned work order
- [ ] 4.3 Test the full unassign workflow: create → assign → unassign → verify work order returns to Draft with no assignee
- [ ] 4.4 Verify the "Unassign" button is NOT visible when the assignee (non-creator) views the work order

## 5. Verification

- [ ] 5.1 Run `dotnet build src/ChurchBulletin.sln` and verify no build errors
- [ ] 5.2 Run unit tests and verify all pass
- [ ] 5.3 Run integration tests and verify all pass
