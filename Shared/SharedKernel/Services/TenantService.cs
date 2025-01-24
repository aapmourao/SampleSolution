namespace SharedKernel.Services;
public class TenantService : ITenantService
{
    private string _tenant;
    private  List<string> _tenants { get; init; }

    public TenantService(IEnumerable<string> tenants)
    {
        _tenants = tenants?.ToList() ?? new List<string>();
        _tenant = string.Empty;
    }

    public string Tenant
    {
        get => _tenant;
        private set
        {
            if (_tenant != value)
            {
                _tenant = value;
                OnTenantChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void SetTenant(string tenant)
    {
        if (_tenants.Contains(tenant))
        {
            Tenant = tenant;
        }
        else
        {
            throw new ArgumentException("Invalid tenant");
        }
    }

    public List<string> GetTenants()
    {
        return _tenants;
    }

    public event TenantChangedEventHandler? OnTenantChanged;
}