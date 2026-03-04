# MCP Status Transitions Update

## Summary
Update the MCP server's status-transitions reference resource to include the new Complete → InProgress (Reopen) transition.

## Updated File: `src/McpServer/Resources/ReferenceResources.cs`

In the `GetStatusTransitions()` method, replace the empty array for `Complete`:

**Before:**
```csharp
Complete = System.Array.Empty<object>()
```

**After:**
```csharp
Complete = new[]
{
    new { Command = "CompleteToInProgressCommand", TargetStatus = "InProgress" }
}
```

## Constraints
- No other changes to the MCP server — the `execute-work-order-command` tool already resolves commands dynamically via `StateCommandList`, so it will automatically support "Reopen" without code changes
- Only the static reference resource needs updating to accurately document the available transitions
