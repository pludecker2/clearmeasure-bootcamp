## 1. Core Domain Changes

- [x] 1.1 Create `src/Core/Model/StateCommands/CompleteToAssignedCommand.cs` — record extending `StateCommandBase`, transitions Complete → Assigned, verb "Reassign", executable by Creator, clears `CompletedDate` and updates `AssignedDate`
- [x] 1.2 Update `WorkOrder.CanReassign()` in `src/Core/Model/WorkOrder.cs` to return `true` for `WorkOrderStatus.Complete` in addition to `WorkOrderStatus.Draft`
- [x] 1.3 Register `CompleteToAssignedCommand` in `StateCommandList.GetAllStateCommands()` in `src/Core/Services/Impl/StateCommandList.cs`

## 2. Unit Tests

- [x] 2.1 Create `src/UnitTests/Core/Model/StateCommands/CompleteToAssignedCommandTests.cs` with 4 standard tests: wrong status, wrong employee, valid, transition
- [x] 2.2 Update `src/UnitTests/Core/Services/StateCommandListTests.cs` to expect 7 commands and verify `CompleteToAssignedCommand` at index 6

## 3. Integration Tests

- [x] 3.1 Create `src/IntegrationTests/DataAccess/Handlers/StateCommandHandlerForReassignTests.cs` with tests for reassign persistence and reassign with different assignee

## 4. Verification

- [ ] 4.1 Build the solution with `dotnet build src/ChurchBulletin.sln`
- [ ] 4.2 Run unit tests with `dotnet test src/UnitTests/UnitTests.csproj`
- [ ] 4.3 Run integration tests with `dotnet test src/IntegrationTests/IntegrationTests.csproj`
