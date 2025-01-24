using SharedKernel.Domain.Core;

namespace SharedKernel.Domain.Common;
public class DomainEnum : EntityRaw<Guid>
{
    public static string ConfigurationName = "InstanceDomainEnums";
    public string EnumName { get; init; }
    public string Name { get; init; }
    public int Value { get; init; }
    public string Metadata { get; init; }

    public DomainEnum(string enumName, string name, int value, string metadata) : base(Guid.NewGuid())
    {
        EnumName = enumName;
        Name = name;
        Value = value;
        Metadata = metadata;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private DomainEnum() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}