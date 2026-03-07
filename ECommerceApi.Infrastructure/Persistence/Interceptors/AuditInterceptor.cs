using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ECommerceApi.Infrastructure.Persistence.Interceptors
{
    public sealed class AuditInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
                DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
            )
        {
            if (eventData.Context is null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            
            var now = DateTime.UtcNow;

            foreach( var entry in eventData.Context.ChangeTracker.Entries() )
            { 
                if(entry.State == EntityState.Added)
                {
                    var createdAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
                    if( createdAt is not null && createdAt.CurrentValue is DateTime currentCreated && currentCreated == default)
                    {
                        // Only set if not already set by the domain (domain sets it too,
                        // but we guard against overwriting a meaningful value)

                        createdAt.CurrentValue = now;
                    }
                    var updatedAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
                    if (updatedAt is not null)
                        updatedAt.CurrentValue = now;
                }
                else if(entry.State == EntityState.Modified)
                {
                    var updatedAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
                    if(updatedAt is not null) 
                        updatedAt.CurrentValue = now;   
                }

            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
