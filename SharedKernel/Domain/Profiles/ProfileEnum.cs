using Ardalis.SmartEnum;

namespace SharedKernel.Domain.Profiles;

public class ProfileEnum : SmartEnum<ProfileEnum>
{
    public static readonly ProfileEnum Admin = new(nameof(Admin), 0);
    public static readonly ProfileEnum Provider = new(nameof(Provider), 1);
    public static readonly ProfileEnum Participant = new(nameof(Participant), 2);

    private ProfileEnum(string name, int value) : base(name, value)
    {
    }
}