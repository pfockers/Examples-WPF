namespace AuthApi.Security;

public static class Permissions
{
    public const string ClaimType = "permission";
    public const string ReportsRead = "reports.read";
    public const string UsersManage = "users.manage";
}

public static class AuthorizationPolicies
{
    public const string ReportsRead = "ReportsRead";
    public const string UsersManage = "UsersManage";
}
