---
languages:
- csharp
- powershell
- html
page_type: sample
description: "Learn how to sign-in users to your web app and call Microsoft Graph (as the app)."
products:
- azure
- azure-active-directory
- dotnet
- azure-app-service
- aspnet
- ms-graph
---
# Tutorial: enable authentication in App Service and call Microsoft Graph (as the signed-in user)

## About this sample
### Overview
This sample demonstrates an ASP.NET Core web app that uses authentication to limit access to users in your organization​ and then calls Microsoft Graph as the app.  The web app authenticates a user and displays some of the user's profile information.  The web app then displays a list of users in the Azure Active Directory tenant. This sample is a companion to the [Access Microsoft Graph from a secured app as the app](https://docs.microsoft.com/azure/app-service/scenario-secure-app-access-microsoft-graph-as-app) tutorial on docs.microsoft.com.

### Scenario
You want to limit access to your web app running on Azure App Service to people in your organization. App Service provides built-in authentication and authorization support, so you can sign in users and access data by writing minimal or no code in your web app.

You also want to call Microsoft Graph from the web app, as the app (not the signed-in user). A safe way to give your web app access to data is to use a system-assigned managed identity. A managed identity from Azure Active Directory allows App Service to access resources through role-based access control (RBAC), without requiring app credentials. After assigning a managed identity to your web app, Azure takes care of the creation and distribution of a certificate. You don't have to worry about managing secrets or app credentials.

## How to run this sample

To run this sample, you'll need:
- [Visual Studio 2019](https://visualstudio.microsoft.com/) for debugging or file editing
- [.NET Core 3.1](https://dotnet.microsoft.com/) or later
- An [Azure subscription](https://docs.microsoft.com/azure/guides/developer/azure-developer-guide#understanding-accounts-subscriptions-and-billing) and an [Azure AD tenant](https://docs.microsoft.com/azure/active-directory/develop/quickstart-create-new-tenant) with one or more user accounts in the directory

### Step 1: Clone or download this repository

Clone or download this repository. From your shell or command line:

```
git clone https://github.com/Azure-Samples/ms-identity-easyauth-dotnet-storage-graphapi.git
cd 3-WebApp-graphapi-managed-identity
```

Build the project in Visual Studio, or run the following command:

```
dotnet build
```

### Step 2: Deploy the web app and configure App Service authentication

This project has one WebApp project. To deploy it to Azure App Service, you'll need to:

- configure a deployment user
- create an Azure App Service plan
- create a web app
- publish the web app to Azure

For information on how to do this from Visual Studio, read the [.NET Core quickstart](https://docs.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore).  

After you've deployed the web app to Azure, [configure the Azure App Service authentication/authorization module](https://docs.microsoft.com/azure/app-service/scenario-secure-app-authentication-app-service).  Also verify that only users in your organization can access the web site.

### Step 3: Enable managed identity on an app

If you create and publish your web app through Visual Studio, the managed identity was enabled on your app for you.  Read this article to learn how to [enable a managed identity on a web app](https://docs.microsoft.com/azure/app-service/scenario-secure-app-access-storage#enable-managed-identity-on-an-app).

### Step 4: Grant access to Microsoft Graph

When accessing the Microsoft Graph, the managed identity needs to have proper permissions for the operation it wants to perform. Currently, there's no option to assign such permissions through the Azure portal. Use PowerShell or the Azure CLI to add the requested Microsoft Graph API permissions to the managed identity service principal object. For more information, read [Grant access to Microsoft Graph](https://docs.microsoft.com/azure/app-service/scenario-secure-app-access-microsoft-graph-as-app#grant-access-to-microsoft-graph) in the tutorial on docs.microsoft.com.

### Step 5: Visit the web app

Open a browser and navigate to the deployed web app (replace *web-app-name* with the name of your web app):  
https://&lt;web-app-name&gt;.azurewebsites.net/Graph-MSI

## About the code

This sample app was created using the [Microsoft.Identity.Web ASP.NET Core wep app template](https://github.com/AzureAD/microsoft-identity-web/wiki#asp-net-core-web-app-and-web-api-project-templates).

### Configure Microsoft.Identity.Web in Startup.cs

The Microsoft.Identity.Web, Microsoft.Identity.Web.UI, and Microsoft.Identity.Web.MicrosoftGraph NuGet packages have been installed in the sample app project. 

In the `public void ConfigureServices(IServiceCollection services)` method, the following lines add support for Microsoft.Identity.Web.

```csharp
services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));
```

Note: for this scenario, you *do not* need to configure the *AzureAd* or *GraphBeta* section settings in the *appsettings.json* file.

Also in the `public void ConfigureServices(IServiceCollection services)` method, call `AddMicrosoftIdentityUI()` in order to add sign-in / sign-out options in the *Pages/Shared/_LoginPartial.cshtml* file.

```csharp
services.AddRazorPages()
    .AddMvcOptions(options => {})
    .AddMicrosoftIdentityUI();
```

### Call Microsoft Graph as the app
The call to Microsoft Graph is performed in the *Pages/Graph-MSI/Index.cshtml.cs* file, in the `public async Task OnGetAsync()` method. The DefaultAzureCredential class is used to get a token credential for your code to authorize requests to Azure Storage. Create an instance of the DefaultAzureCredential class, which uses the managed identity to fetch tokens and attach them to the service client. The following code example gets the authenticated token credential and uses it to create a service client object, which gets the users in the group.

```csharp
// Create the Graph service client with a DefaultAzureCredential which gets an access token using the available Managed Identity
var credential = new DefaultAzureCredential();
var token = credential.GetToken(
    new Azure.Core.TokenRequestContext(
        new[] { "https://graph.microsoft.com/.default" }));

var accessToken = token.Token;
var graphServiceClient = new GraphServiceClient(
    new DelegateAuthenticationProvider((requestMessage) =>
    {
        requestMessage
        .Headers
        .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

        return Task.CompletedTask;
    }));
```

### Display name of the signed-in user
When you access the web app running on Azure, you see a "Hello <user-name>!" message and also a sign in/sign out option at the top of the page.  The code for this is found in the *Pages/Shared/_LoginPartial.cshtml* file.  The Microsoft.Identity.Web library integrates with the Azure App Service authentication/authorization module.  When a user signs in to the web app, Microsoft.Identity.Web gets the user's name and displays it on the page.  The sign in/sign out options are enabled by the Microsoft.Identity.Web.Ui library:

```html
@if (User.Identity.IsAuthenticated)
{
        <li class="nav-item">
            <span class="navbar-text text-dark">Hello @User.Identity.Name!</span>
        </li>
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="MicrosoftIdentity" asp-controller="Account" asp-action="SignOut">Sign out</a>
        </li>
}
else
{
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="MicrosoftIdentity" asp-controller="Account" asp-action="SignIn">Sign in</a>
        </li>
}
```

## Resources

Read the [Access Microsoft Graph from a secured app as the app](https://docs.microsoft.com/azure/app-service/scenario-secure-app-access-microsoft-graph-as-app) tutorial.
