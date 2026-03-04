using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.DataAccess.Handlers;
using ClearMeasure.Bootcamp.UnitTests.Core.Queries;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace ClearMeasure.Bootcamp.IntegrationTests.DataAccess.Handlers;

public class StateCommandHandlerForUnassignTests : IntegratedTestBase
{
    [Test]
    public async Task ShouldUnassignWorkOrder()
    {
        new DatabaseTests().Clean();

        var workOrder = Faker<WorkOrder>();
        workOrder.Id = Guid.Empty;
        var creator = Faker<Employee>();
        var assignee = Faker<Employee>();
        workOrder.Creator = creator;
        workOrder.Assignee = assignee;
        workOrder.Status = WorkOrderStatus.Assigned;

        await using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.Add(creator);
            context.Add(assignee);
            await context.SaveChangesAsync();
        }

        var command = RemotableRequestTests.SimulateRemoteObject(new AssignedToDraftCommand(workOrder, creator));
        var handler = TestHost.GetRequiredService<StateCommandHandler>();
        var result = await handler.Handle(command);

        result.WorkOrder.Status.ShouldBe(WorkOrderStatus.Draft);
        result.WorkOrder.Assignee.ShouldBeNull();

        var context3 = TestHost.GetRequiredService<DbContext>();
        var order = context3.Find<WorkOrder>(result.WorkOrder.Id) ?? throw new InvalidOperationException();
        order.Status.ShouldBe(WorkOrderStatus.Draft);
        order.Assignee.ShouldBeNull();
        order.AssignedDate.ShouldBeNull();
        order.Creator.ShouldBe(creator);
        order.Title.ShouldBe(workOrder.Title);
        order.Description.ShouldBe(workOrder.Description);
    }

    [Test]
    public async Task ShouldUnassignAndReassignWorkOrder()
    {
        new DatabaseTests().Clean();

        var workOrder = Faker<WorkOrder>();
        workOrder.Id = Guid.Empty;
        var creator = Faker<Employee>();
        var assignee = Faker<Employee>();
        var newAssignee = Faker<Employee>();
        workOrder.Creator = creator;
        workOrder.Assignee = assignee;
        workOrder.Status = WorkOrderStatus.Assigned;

        await using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.Add(creator);
            context.Add(assignee);
            context.Add(newAssignee);
            await context.SaveChangesAsync();
        }

        // Unassign
        var unassignCommand = RemotableRequestTests.SimulateRemoteObject(new AssignedToDraftCommand(workOrder, creator));
        var handler = TestHost.GetRequiredService<StateCommandHandler>();
        var result = await handler.Handle(unassignCommand);

        result.WorkOrder.Status.ShouldBe(WorkOrderStatus.Draft);
        result.WorkOrder.Assignee.ShouldBeNull();

        // Reassign
        var unassignedOrder = result.WorkOrder;
        unassignedOrder.Assignee = newAssignee;
        var assignCommand = RemotableRequestTests.SimulateRemoteObject(new DraftToAssignedCommand(unassignedOrder, creator));
        var handler2 = TestHost.GetRequiredService<StateCommandHandler>();
        var result2 = await handler2.Handle(assignCommand);

        result2.WorkOrder.Status.ShouldBe(WorkOrderStatus.Assigned);
        result2.WorkOrder.Assignee.ShouldBe(newAssignee);
    }
}
