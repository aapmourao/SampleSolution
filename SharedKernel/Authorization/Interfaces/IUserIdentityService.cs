namespace SharedKernel.Authorization.Interfaces;

public interface IUserIdentityService
{
    public string UserHasPermission(string permission);
    public string GetUserId();
    public string GetUserName();
    public string GetUserEmail();
    public bool IsAuthenticated { get; }
    public bool IsAdmin { get; }

}