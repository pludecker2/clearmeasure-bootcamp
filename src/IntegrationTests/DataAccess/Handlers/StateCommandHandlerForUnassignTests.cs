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

        var creator = Faker<Employee>();
        creator.Id = Guid.NewGuid();
        var assignee = Faker<Employee>();
        assignee.Id = Guid.NewGuid();

        await using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.Add(creator);
            context.Add(assignee);
            await context.SaveChangesAsync();
        }

        var workOrder = Faker<WorkOrder>();
        workOrder.Creator = creator;
        workOrder.Assignee = assignee;
        workOrder.AssignedDate = DateTime.Now;
        workOrder.Status = WorkOrderStatus.Assigned;

        await using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.Add(workOrder);
            await context.SaveChangesAsync();
        }

        var command = RemotableRequestTests.SimulateRemoteObject(new AssignedToDraftCommand(workOrder, creator));
        var handler = TestHost.GetRequiredService<StateCommandHandler>();
        var result = await handler.Handle(command);

        result.WorkOrder.Status.ShouldBe(WorkOrderStatus.Draft);
        result.WorkOrder.Assignee.ShouldBeNull();

        var context3 = TestHost.GetRequiredService<DbContext>();
        var order = context3.Find<WorkOrder>(workOrder.Id) ?? throw new InvalidOperationException();
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

        var creator = Faker<Employee>();
        creator.Id = Guid.NewGuid();
        var assignee = Faker<Employee>();
        assignee.Id = Guid.NewGuid();
        var newAssignee = Faker<Employee>();
        newAssignee.Id = Guid.NewGuid();

        await using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.Add(creator);
            context.Add(assignee);
            context.Add(newAssignee);
            await context.SaveChangesAsync();
        }

        var workOrder = Faker<WorkOrder>();
        workOrder.Creator = creator;
        workOrder.Assignee = assignee;
        workOrder.AssignedDate = DateTime.Now;
        workOrder.Status = WorkOrderStatus.Assigned;

        await using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.Add(workOrder);
            await context.SaveChangesAsync();
        }

        // Unassign
        var unassignCommand = RemotableRequestTests.SimulateRemoteObject(new AssignedToDraftCommand(workOrder, creator));
        var handler = TestHost.GetRequiredService<StateCommandHandler>();
        var result = await handler.Handle(unassignCommand);

        result.WorkOrder.Status.ShouldBe(WorkOrderStatus.Draft);
        result.WorkOrder.Assignee.ShouldBeNull();

        // Reassign
        workOrder.Assignee = newAssignee;
        var assignCommand = RemotableRequestTests.SimulateRemoteObject(new DraftToAssignedCommand(workOrder, creator));
        var result2 = await handler.Handle(assignCommand);

        result2.WorkOrder.Status.ShouldBe(WorkOrderStatus.Assigned);
        result2.WorkOrder.Assignee.ShouldBe(newAssignee);
    }
}
