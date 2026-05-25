using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results.Abstractions;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviours
{
    public class CachingBehaviour<TRequest, TResponse>(
        HybridCache cache,
        ILogger<CachingBehaviour<TRequest, TResponse>> logger
    ) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly HybridCache _cache = cache;
        private readonly ILogger<CachingBehaviour<TRequest, TResponse>> _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (request is not ICachedQuery cachedRequest)
                return await next(ct);

            _logger.LogInformation("Checking cache for {RequestName}", typeof(TRequest).Name);

            var result = await _cache.GetOrCreateAsync(
                cachedRequest.CacheKey,
                _ => new ValueTask<TResponse>((TResponse)(object)null!),
                new HybridCacheEntryOptions
                {
                    Flags = HybridCacheEntryFlags.DisableUnderlyingData
                },
                cancellationToken: ct);

            if(result is null)
            {
                await next(ct);

                if(result is IResult res && res.IsSuccess)
                {
                    _logger.LogInformation("Caching result for {RequestName}", typeof(TRequest).Name);

                    await _cache.SetAsync(
                        cachedRequest.CacheKey,
                        result,
                        new HybridCacheEntryOptions
                        {
                            Expiration = cachedRequest.Expiration
                        }, 
                        cancellationToken: ct);
                }
            }

            return result;
        }
    }
}
