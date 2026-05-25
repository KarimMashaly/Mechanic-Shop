using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomerQueryHandler(
        IAppDbContext context)
        : IRequestHandler<GetCustomerQuery, Result<List<CustomerDto>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<List<CustomerDto>>> Handle(GetCustomerQuery query, CancellationToken ct)
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .Include(c => c.Vehicles)
                .ToListAsync(ct);

            return customers.ToDtos();
        }
    }
}
