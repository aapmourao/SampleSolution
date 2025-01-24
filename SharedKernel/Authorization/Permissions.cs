using SharedKernel.Domain;
using SharedKernel.Domain.Common;

namespace SharedKernel.Authorization;

public class Permissions : DynamicSmartEnum<Permissions>, IDomainEnum
{
    public static string EnumName => nameof(Permissions);

    public Permissions(string name, int value) : base(name, value)
    {
    }

    public static Permissions AddElement(string name, int value)
    {
        if (TryFromName(name, out var categoryByName))
        {
            throw new InvalidOperationException($"Permission with name '{categoryByName.Name}' already exists.");
        }

        if (TryFromValue(value, out var categoryByValue))
        {
            throw new InvalidOperationException($"Permission with value '{categoryByValue.Value}' already exists.");
        }

        var category = new Permissions(name, value);
        Add(category);
        return category;
    }

    public static IEnumerable<Permissions> GetAllCategories()
    {
        return List;
    }
}