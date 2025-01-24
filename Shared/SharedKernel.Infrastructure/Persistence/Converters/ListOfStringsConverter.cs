using System.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SharedKernel.Infrastructure.Persistence.Converters;

public class ListOfStringsConverter : ValueConverter<List<string>, string>
{
    internal static string _separator = "#!#";
    public ListOfStringsConverter(ConverterMappingHints? mappingHints = null)
        : base(
            v => string.Join(_separator, v),
            v => v.Split(_separator, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList(),
            mappingHints)
    {
    }
}

public class ListOfStringsComparer : ValueComparer<List<string>>
{
    public ListOfStringsComparer() : base(
      (t1, t2) => t1!.SequenceEqual(t2!),
      t => t.Select(x => x!.GetHashCode()).Aggregate((x, y) => x ^ y),
      t => t)
    {
    }
}