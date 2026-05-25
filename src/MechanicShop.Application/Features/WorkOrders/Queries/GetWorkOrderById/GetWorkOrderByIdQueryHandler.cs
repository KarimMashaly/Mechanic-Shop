using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Application.Features.WorkOrders.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdQueryHandler(
        IAppDbContext context,
        ILogger<GetWorkOrderByIdQueryHandler> logger)
        : IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetWorkOrderByIdQueryHandler> _logger = logger;

        public async Task<Result<WorkOrderDto>> Handle(GetWorkOrderByIdQuery query, CancellationToken ct)
        {
            var workOrder = await _context.WorkOrders
                .AsNoTracking()
                .Include(wo => wo.Vehicle)
                    .ThenInclude(v => v.Customer)
                .Include(wo => wo.RepairTasks)
                    .ThenInclude(rt => rt.Parts)
                .Include(wo => wo.Invoice)
                .Include(wo => wo.Labor)
                .FirstOrDefaultAsync(wo => wo.Id == query.WorkOrderId, ct);

            if(workOrder is null)
            {
                _logger.LogWarning("WorkOrder with id {WorkOrderId} was not found", query.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }

            return workOrder.ToDto();
        }
    }
}
