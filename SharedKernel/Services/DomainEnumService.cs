using SharedKernel.Domain.Common;

namespace SharedKernel.Services;

public class DomainEnumService
{
    public List<DomainEnum> DomainEnums { get; private set; } = new List<DomainEnum>();

    public void SetDomainEnum(List<DomainEnum> domainEnum)
    {
        DomainEnums = domainEnum;
    }
}