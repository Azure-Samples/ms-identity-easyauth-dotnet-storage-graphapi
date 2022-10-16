---
languages:
- csharp
- powershell
- html
page_type: sample
description: "Learn how to sign-in users to your web app and call Azure storage"
products:
- azure
- azure-active-directory
- dotnet
- azure-storage
- azure-app-service
- aspnet
---
# Tutorial: enable authentication in App Service and call Azure storage

## About this sample
### Overview
This sample demonstrates an ASP.NET Core web app that uses authentication to limit access to users in your organization​ and then calls Azure storage as the web app (not the signed-in user).  The web app authenticates users and also uploads, displays, and deletes text blobs in Azure storage. This sample is a companion to the [Access Azure Storage from a web app](https://learn.microsoft.com/azure/active-directory/develop/multi-service-web-app-access-storage) tutorial on docs.microsoft.com.

### Scenario
You want to limit access to your web app running on Azure App Service to people in your organization. App Service provides built-in authentication and authorization support, so you can sign in users and access data by writing minimal or no code in your web app.

You also want to add access to the Azure data plane (Azure Storage, Azure SQL Database, Azure Key Vault, or other services) from your web app. You could use a shared key, but then you have to worry about operational security of who can create, deploy, and manage the secret. It's also possible that the key could be checked into GitHub, which hackers know how to scan for. A safer way to give your web app access to data is to use managed identities.

A managed identity from Azure Active Directory (Azure AD) allows App Service to access resources through role-based access control (RBAC), without requiring app credentials. After assigning a managed identity to your web app, Azure takes care of the creation and distribution of a certificate. People don't have to worry about managing secrets or app credentials.

## How to run this sample

To run this sample, you'll need:
- [Visual Studio 2019](https://visualstudio.microsoft.com/) for debugging or file editing
- [.NET Core 3.1](https://dotnet.microsoft.com/) or later
- An [Azure subscription](https://docs.microsoft.com/azure/guides/developer/azure-developer-guide#understanding-accounts-subscriptions-and-billing) and an [Azure AD tenant](https://docs.microsoft.com/azure/active-directory/develop/quickstart-create-new-tenant) with one or more user accounts in the directory

### Step 1: Clone or download this repository

Clone or download this repository. From your shell or command line:

```
git clone https://github.com/Azure-Samples/ms-identity-easyauth-dotnet-storage-graphapi.git
cd 1-WebApp-storage-managed-identity
```

Build the project in Visual Studio, or run the following command:
```
dotnet build
```

### Step 2: Create a resource group, storage account, and Blob Storage container
Create a storage account and Blob Storage container for the web app to access.

Every storage account must belong to an Azure resource group. A resource group is a logical container for grouping your Azure services. When you create a storage account, you have the option to either create a new resource group or use an existing resource group. 

This article shows how to [create a new resource group, a storage account, and a Blob Storage container](https://learn.microsoft.com/azure/active-directory/develop/multi-service-web-app-access-storage?tabs=azure-portal%2Cprogramming-language-csharp#create-a-storage-account-and-blob-storage-container).

Make sure you take note of the storage account name and the Blob Storage container name, which you will need in the next step.

### Step 3: Configure the sample to use your Azure storage account and blob container

In Visual Studio, open the *appsettings.json* file.  In the *AzureStorageConfig* section, update the *AccountName* and *ContainerName* settings with the values from the previous step.

```json
"AzureStorageConfig": {
    "AccountName": "<storage-account-name>",
    "ContainerName": "<blob-container-name>"

  }
```

### Step 4: Deploy the web app and configure App Service authentication

This project has one WebApp project. To deploy it to Azure App Service, you'll need to:

- configure a deployment user
- create an Azure App Service plan
- create a web app
- publish the web app to Azure

For information on how to do this from Visual Studio, read the [.NET Core quickstart](https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore).  Use the same resource group that you used in step 2 instead of creating a new resource group.

After you've deployed the web app to Azure, [configure the Azure App Service authentication/authorization module](https://learn.microsoft.com/azure/active-directory/develop/multi-service-web-app-authentication-app-service).  Also verify that only users in your organization can access the web site.

### Step 5: Enable managed identity on an app

If you create and publish your web app through Visual Studio, the managed identity was enabled on your app for you.  Read this article to learn how to [enable a managed identity on a web app](https://learn.microsoft.com/azure/active-directory/develop/multi-service-web-app-access-storage?tabs=azure-portal%2Cprogramming-language-csharp#enable-managed-identity-on-an-app).

### Step 6: Grant access to the storage account

You need to grant your web app access to the storage account before you can create, read, or delete blobs. In a previous step, you configured the web app running on App Service with a managed identity. Using Azure RBAC, you can give the managed identity access to another resource, just like any security principal. The Storage Blob Data Contributor role gives the web app (represented by the system-assigned managed identity) read, write, and delete access to the blob container and data.  Read this article to learn how to [grant access to the storage account](https://learn.microsoft.com/azure/active-directory/develop/multi-service-web-app-access-storage?tabs=azure-portal%2Cprogramming-language-csharp#grant-access-to-the-storage-account).

### Step 7: Visit the web app

Open a browser and navigate to the deployed web app (replace *web-app-name* with the name of your web app):  
https://&lt;web-app-name&gt;.azurewebsites.net/Storage-MSI

## About the code

This sample app was created using the [Microsoft.Identity.Web ASP.NET Core wep app template](https://github.com/AzureAD/microsoft-identity-web/wiki#asp-net-core-web-app-and-web-api-project-templates).

### Configure Microsoft.Identity.Web and Azure Storage in Startup.cs

The Microsoft.Identity.Web, Microsoft.Identity.Web.UI, and Azure.Storage.Blobs NuGet packages have been installed in the sample app project. 

In the ```public void ConfigureServices(IServiceCollection services)``` method, the following lines add support for Azure storage blobs and Microsoft.Identity.Web.

```csharp
services.AddTransient<CommentsContext>();
services.Configure<AzureStorageConfig>(Configuration.GetSection("AzureStorageConfig"));
services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));
```

Note: for this scenario, you *do not* need to configure the *AzureAd* section settings in the *appsettings.json* file.

Also in the ```public void ConfigureServices(IServiceCollection services)``` method, call AddMicrosoftIdentityUI() in order to add sign-in / sign-out options in the *Pages/Shared/_LoginPartial.cshtml* file. 

```csharp
services.AddRazorPages()
    .AddMvcOptions(options => {})
    .AddMicrosoftIdentityUI();
```

### Call storage using managed identities

The methods for uploading, getting, and deleting blobs are in *Helpers/StorageHelper.cs*. The DefaultAzureCredential class is used to get a token credential for your code to authorize requests to Azure Storage. Create an instance of the DefaultAzureCredential class, which uses the managed identity to fetch tokens and attach them to the service client. The following code example gets the authenticated token credential and uses it to create a service client object, which uploads a new blob.

```csharp
static public async Task UploadBlob(string accountName, string containerName, string blobName, string blobContents)
{
    // Construct the blob container endpoint from the arguments.
    string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                                                accountName,
                                                containerName);

    // Get a credential and create a client object for the blob container.
    BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                    new DefaultAzureCredential());


    try
    {
        // Create the container if it does not exist.
        await containerClient.CreateIfNotExistsAsync();

        // Upload text to a new block blob.
        byte[] byteArray = Encoding.ASCII.GetBytes(blobContents);

        using (MemoryStream stream = new MemoryStream(byteArray))
        {
            await containerClient.UploadBlobAsync(blobName, stream);
        }
    }
    catch (Exception e)
    {
        throw e;
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

Read the [Access Azure Storage from a web app](https://learn.microsoft.com/azure/active-directory/develop/multi-service-web-app-access-storage) tutorial.
