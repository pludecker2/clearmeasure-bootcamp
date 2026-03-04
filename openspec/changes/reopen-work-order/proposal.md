### Why
Work orders that have been completed cannot currently be revisited. If an assignee realizes that additional work is needed on a completed work order, there is no way to reopen it — they must create a new work order or work outside the system. This creates friction and breaks the audit trail. Adding a Reopen transition from Complete back to InProgress allows the assignee to resume work on a completed work order, keeping the full history in one place.

### What Changes
- New state command `CompleteToInProgressCommand` ("Reopen") that transitions a work order from Complete to InProgress status
- Only the assignee on the work order can execute the Reopen command
- The Reopen button appears on the work order manage page when the assignee views a completed work order
- `CompletedDate` is cleared when a work order is reopened
- The `StateCommandList` is updated to include the new command
- The MCP server's status-transitions reference resource is updated to reflect the new transition
- Unit tests, integration tests, and acceptance tests cover the new functionality

### Capabilities

**New Capabilities:**
- `reopen-state-command`: New `CompleteToInProgressCommand` state command and registration in `StateCommandList`
- `reopen-unit-tests`: Unit tests for the new command class and updated `StateCommandList` test
- `reopen-integration-tests`: Integration test for the `StateCommandHandler` executing the Reopen command against the database
- `reopen-acceptance-tests`: End-to-end acceptance test that completes a work order and then reopens it via the UI
- `mcp-status-transitions-update`: Update the MCP reference resource to include the Complete → InProgress transition

### Impact
- **Core project**: New `CompleteToInProgressCommand.cs` in `src/Core/Model/StateCommands/`, updated `StateCommandList.cs`
- **McpServer project**: Updated `ReferenceResources.cs` status-transitions map
- **Database**: No schema changes — reuses existing status codes
- **UI**: No Razor changes needed — the existing `ValidCommands` loop in `WorkOrderManage.razor` will automatically render the Reopen button when the command is valid
- **Dependencies**: No new NuGet packages
