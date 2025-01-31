using SharedKernel.Domain.Core;

namespace Domain.UserRolesAggregate;

public class User : AggregateRoot<UserId, Guid>
{
    public string UserName { get; set; }

    // Navigation property  
    public ICollection<UserRole> UserRoles { get; set; }
}

public class Role : AggregateRoot<RoleId, Guid>
{
    public string RoleName { get; set; }

    // Navigation property  
    public ICollection<UserRole> UserRoles { get; set; }

}

// Join table for many-to-many relationship  
public class UserRole
{
    public UserId UserId { get; set; }
    public User User { get; set; }

    public RoleId RoleId { get; set; }
    public Role Role { get; set; }
}

public class RoleId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public RoleId(Guid value) { Value = value; }
    public static RoleId CreateUnique() => new(Guid.NewGuid());
    public static RoleId Create(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}


public class UserId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public UserId(Guid value) { Value = value; }
    public static UserId CreateUnique() => new(Guid.NewGuid());
    public static UserId Create(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}