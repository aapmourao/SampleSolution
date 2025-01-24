namespace SharedKernel.Infrastructure.Persistence.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using SharedKernel.Domain.Common;

    public interface IDbVersionDbContext : IGenericDbContext
    {
        DbSet<DomainEnum> DomainEnums { get; set; }
    }
}