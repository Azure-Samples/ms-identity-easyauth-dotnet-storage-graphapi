---
languages:
- csharp
- powershell
- html
page_type: sample
description: "Learn how to sign-in users to your web app and call Microsoft Graph (as the signed-in user)."
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
This sample demonstrates an ASP.NET Core web app that uses authentication to limit access to users in your organization​ and then calls Microsoft Graph as the signed-in user.  The web app authenticates a user and displays some of the user's profile information.  This sample is a companion to the [Access Microsoft Graph from a secured app as the user](https://docs.microsoft.com/azure/app-service/scenario-secure-app-access-microsoft-graph-as-user) tutorial on docs.microsoft.com.

### Scenario
You want to limit access to your web app running on Azure App Service to people in your organization. App Service provides built-in authentication and authorization support, so you can sign in users and access data by writing minimal or no code in your web app.

You also want to add access to Microsoft Graph from your web app and perform some action as the signed-in user. This section describes how to grant delegated permissions to the web app and get the signed-in user's profile information from Azure Active Directory (Azure AD).

## How to run this sample

To run this sample, you'll need:
- [Visual Studio 2019](https://visualstudio.microsoft.com/) for debugging or file editing
- [.NET Core 3.1](https://dotnet.microsoft.com/) or later
- An [Azure subscription](https://docs.microsoft.com/azure/guides/developer/azure-developer-guide#understanding-accounts-subscriptions-and-billing) and an [Azure AD tenant](https://docs.microsoft.com/azure/active-directory/develop/quickstart-create-new-tenant) with one or more user accounts in the directory

### Step 1: Clone or download this repository

Clone or download this repository. From your shell or command line:

```
git clone https://github.com/Azure-Samples/ms-identity-easyauth-dotnet-storage-graphapi.git
cd 2-WebApp-graphapi-on-behalf
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

### Step 3: Grant web app access to call Microsoft Graph

Now that you've enabled authentication and authorization on your web app, the web app is registered with the Microsoft identity platform and is backed by an Azure AD application. In this step, you give the web app permissions to access Microsoft Graph for the user. For more information, read [Grant web app access](https://docs.microsoft.com/azure/app-service/scenario-secure-app-access-microsoft-graph-as-user#grant-front-end-access-to-call-microsoft-graph) in the tutorial on docs.microsoft.com.

### Step 4: Configure App Service to return a usable access token

The web app now has the required permissions to access Microsoft Graph as the signed-in user. In this step, you configure App Service authentication and authorization to give you a usable access token for accessing Microsoft Graph.  For more information, read [Configure App Service to return a usable access token](https://docs.microsoft.com/azure/app-service/scenario-secure-app-access-microsoft-graph-as-user#configure-app-service-to-return-a-usable-access-token) in the tutorial on docs.microsoft.com.

### Step 5: Visit the web app

Open a browser and navigate to the deployed web app (replace *web-app-name* with the name of your web app):  
https://&lt;web-app-name&gt;.azurewebsites.net/Graph-OBO

## About the code

This sample app was created using the [Microsoft.Identity.Web ASP.NET Core wep app template](https://github.com/AzureAD/microsoft-identity-web/wiki#asp-net-core-web-app-and-web-api-project-templates).

### Configure Microsoft.Identity.Web and Microsoft Graph in Startup.cs

The Microsoft.Identity.Web, Microsoft.Identity.Web.UI, and Microsoft.Identity.Web.MicrosoftGraph NuGet packages have been installed in the sample app project. 

In the ```public void ConfigureServices(IServiceCollection services)``` method, the following lines add support for Microsoft Graph and Microsoft.Identity.Web.

```csharp
services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi()
                           .AddMicrosoftGraph(Configuration.GetSection("GraphBeta"))
                           .AddInMemoryTokenCaches();
```

Note: for this scenario, you *do not* need to configure the *AzureAd* or *GraphBeta* section settings in the *appsettings.json* file.

Also in the ```public void ConfigureServices(IServiceCollection services)``` method, call `AddMicrosoftIdentityUI()` in order to add sign-in / sign-out options in the *Pages/Shared/_LoginPartial.cshtml* file.

```csharp
services.AddRazorPages()
    .AddMvcOptions(options => {})
    .AddMicrosoftIdentityUI();
```

### Call Microsoft Graph on behalf of the signed-in user

```csharp
public IndexModel(ILogger<IndexModel> logger, GraphServiceClient graphServiceClient)
{
    _logger = logger;
    _graphServiceClient = graphServiceClient;
}

public async Task OnGetAsync()
{
    try
    {
        var user = await _graphServiceClient.Me.Request().GetAsync();
        ViewData["Me"] = user;
        ViewData["name"] = user.DisplayName;

        using (var photoStream = await _graphServiceClient.Me.Photo.Content.Request().GetAsync())
        {
            byte[] photoByte = ((MemoryStream)photoStream).ToArray();
            ViewData["photo"] = Convert.ToBase64String(photoByte);
        }
    }
    catch (Exception ex)
    {
        ViewData["photo"] = null;
    }
}
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

Read the [Access Microsoft Graph from a secured app as the user](https://docs.microsoft.com/azure/app-service/scenario-secure-app-access-microsoft-graph-as-user) tutorial.
