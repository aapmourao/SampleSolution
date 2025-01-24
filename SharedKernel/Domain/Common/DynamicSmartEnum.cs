namespace SharedKernel.Domain.Common;

public abstract class DynamicSmartEnum<T> where T : DynamicSmartEnum<T>
{
    public string Name { get; }
    public int Value { get; }

    protected DynamicSmartEnum(string name, int value)
    {
        Name = name;
        Value = value;
    }

    private static readonly List<T> _allInstances = new List<T>();

    public static void Add(T instance)
    {
        _allInstances.Add(instance);
    }

    public static IEnumerable<T> List => _allInstances.AsReadOnly();

    public static T FromValue(int value)
    {
        return _allInstances.FirstOrDefault(i => i.Value == value)!;
    }
    public static bool TryFromValue(int value, out T instance)
    {
        instance = _allInstances.FirstOrDefault(i => i.Value == value)!;
        return instance != null;
    }
    public static T FromName(string name)
    {
        return _allInstances.FirstOrDefault(i => i.Name == name)!;
    }
    public static bool TryFromName(string name, out T instance)
    {
        instance = _allInstances.FirstOrDefault(i => i.Name == name)!;
        return instance != null;
    }
}