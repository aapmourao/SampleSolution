using System.Reflection;
using Microsoft.Extensions.Logging;
using SharedKernel.Domain;
using SharedKernel.Domain.Common;

namespace SharedKernel.Infrastructure.Persistence.DomainEnums;

public interface IDomainEnumFactory
{
    public void PopulateEnums(Assembly[]? assemblies, IEnumerable<DomainEnum> domainEnums);
}

public class DomainEnumFactory : IDomainEnumFactory
{
    private readonly ILogger<DomainEnumFactory> _logger;

    public DomainEnumFactory(ILogger<DomainEnumFactory> logger)
    {
        _logger = logger;
    }

    public void PopulateEnums(Assembly[]? assemblies, IEnumerable<DomainEnum> domainEnums)
    {
        _logger.LogInformation("DomainEnumFactory.Execute");

        // Get all assemblies with IDomainEnum types
        var domainEnumType = typeof(IDomainEnum);
        //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var domainEnumTypes = assemblies?
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => domainEnumType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

        foreach (var type in domainEnumTypes!)
        {
            // Read the static EnumName property
            var enumNameProperty = type.GetProperty("EnumName", BindingFlags.Public | BindingFlags.Static);
            if (enumNameProperty != null)
            {
                var enumName = enumNameProperty.GetValue(null);
                _logger.LogInformation($"  > EnumName: {enumName}");


                // Call the static AddCategory method
                var addCategoryMethod = type.GetMethod("AddElement", BindingFlags.Public | BindingFlags.Static);
                if (addCategoryMethod != null && enumName != null)
                {
                    foreach (var domainEnum in domainEnums.Where(de => de.EnumName == enumName.ToString()))
                    {
                        var domainEnumName = domainEnum.Name;
                        var domainEnumValue = domainEnum.Value;

                        _logger.LogInformation($"  > DomainEnum: {domainEnumName} - {domainEnumValue}");

                        addCategoryMethod.Invoke(null, new object[] { domainEnumName, domainEnumValue });
                    }
                }
            }
        }
    }
}