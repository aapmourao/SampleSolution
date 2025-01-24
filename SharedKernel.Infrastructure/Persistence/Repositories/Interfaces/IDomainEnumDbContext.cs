using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Common;

namespace SharedKernel.Infrastructure.Persistence.Repositories;

public interface IDomainEnumDbContext : IGenericDbContext
{
    DbSet<DomainEnum> DomainEnums { get; set; }
}
