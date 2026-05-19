using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer
{
    public sealed class UpdateCustomerCommandHandler(
        ILogger<UpdateCustomerCommandHandler> logger,
        IAppDbContext context,
        HybridCache cache) : IRequestHandler<UpdateCustomerCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateCustomerCommand command, CancellationToken ct)
        {
            var customer = await context.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.Id == command.CustomerId);

            if(customer is null)
            {
                logger.LogWarning("Customer {CustomerId} not found for update.", command.CustomerId);

                return ApplicationErrors.CustomerNotFound;
            }

            var validatedVehicles = new List<Vehicle>();

            foreach(var v in command.Vehicles)
            {
                var vehicleId = v.VehicleId ?? Guid.NewGuid();

                var vehicleResult = Vehicle.Create(vehicleId, v.Make, v.Model, v.Year, v.LicensePlate);

                if(vehicleResult.IsError)
                {
                    return vehicleResult.Errors!;
                }

                validatedVehicles.Add(vehicleResult.Value);
            }

            var updateCustomerResult = customer.Update(customer.Name!, customer.Phone!, customer.Email!);

            if(updateCustomerResult.IsError)
                return updateCustomerResult.Errors!;

            var upsertPartsResult = customer.UpsertParts(validatedVehicles);
            
            if(upsertPartsResult.IsError)
                return upsertPartsResult.Errors!;

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("customer", ct);

            return Result.Updated;
        }
    }
}
