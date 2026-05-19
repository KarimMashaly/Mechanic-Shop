using MechanicShop.Domain.Customers;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.WorkOrders;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Customer> Customers { get; }
        DbSet<WorkOrder> WorkOrders { get; }
        DbSet<RepairTask> RepairTasks { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
