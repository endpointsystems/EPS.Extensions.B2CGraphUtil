# EPS.Extensions.B2CGraphUtil
Quick and easy graph utilities for Users and Groups for Azure Active Directory B2C.

## Required Permissions

You'll want to grant admin consent to the following permissions for Microsoft Graph application in Azure AD B2C:

- Directory.ReadWrite.All
- Group.ReadWrite.All
- People.Read.All
- User.ReadWrite.All
- openid
- offline_access

See [Register a Microsoft Graph Application](https://docs.microsoft.com/en-us/azure/active-directory-b2c/microsoft-graph-get-started?tabs=app-reg-ga) for more details.

## Configuration

Both `UserRepo` and `GroupRepo` depend on an instance of `GraphUtilConfig` configuration object for instantiation. This class has three properties:
- `AppId` - the application ID
- `TenantId` - the tenant ID (the GUID)
- `AppSecret` - the application secret
