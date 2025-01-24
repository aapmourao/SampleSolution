namespace SharedKernel.Services;

public delegate void TenantChangedEventHandler(object sender, EventArgs e);

public interface ITenantService
{
    string  Tenant { get; }

    void SetTenant(string tenant);

    List<string>  GetTenants();

    event TenantChangedEventHandler OnTenantChanged;
}