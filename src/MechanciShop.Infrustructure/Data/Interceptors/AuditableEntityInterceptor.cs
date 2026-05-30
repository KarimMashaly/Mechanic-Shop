using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MechanciShop.Infrustructure.Data.Interceptors
{
    public class AuditableEntityInterceptor(IUser user, TimeProvider dateTime) : SaveChangesInterceptor
    {
        private readonly IUser _user = user;
        private readonly TimeProvider _dateTime = dateTime;

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            UpdateEntites(eventData.Context);

            return base.SavedChanges(eventData, result);
        }

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            UpdateEntites(eventData.Context);

            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntites(DbContext? context)
        {
            if (context is null)
                return;

            foreach(var entry in context.ChangeTracker.Entries<AuditableEntity>())
            {
                var utcNow = _dateTime.GetUtcNow();

                if(entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    if(entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedBy = _user.Id;
                        entry.Entity.CreatedAtUtc = utcNow;
                    }

                    entry.Entity.LastModifiedBy = _user.Id;
                    entry.Entity.LastModifiedAtUtc = utcNow;

                    foreach(var ownedEntry in entry.References)
                    {
                        if(ownedEntry.TargetEntry is { Entity: AuditableEntity ownedEntity} &&
                            ownedEntry.TargetEntry.State is EntityState.Added or EntityState.Modified)
                        {
                            if(ownedEntry.TargetEntry.State == EntityState.Added)
                            {
                                ownedEntity.CreatedBy = _user.Id;
                                ownedEntity.CreatedAtUtc = utcNow;
                            }

                            ownedEntity.LastModifiedBy= _user.Id;
                            ownedEntity.LastModifiedAtUtc = utcNow;
                        }
                    }
                }
            }
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
            r.EntityEntry?.Metadata.IsOwned() == true &&
            (r.TargetEntry?.State == EntityState.Added || r.TargetEntry?.State == EntityState.Modified));
    }
}
