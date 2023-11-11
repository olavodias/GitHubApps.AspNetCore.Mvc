# GitHubApps.AspNetCore.Mvc

[![nuget](https://img.shields.io/nuget/v/GitHubApps.AspNetCore.Mvc.svg)](https://www.nuget.org/packages/GitHubApps.AspNetCore.Mvc/) 
![GitHub release](https://img.shields.io/github/release/olavodias/GitHubApps.AspNetCore.Mvc.svg)
![NuGet](https://img.shields.io/nuget/dt/GitHubApps.AspNetCore.Mvc.svg)
![license](https://img.shields.io/github/license/olavodias/GitHubApps.AspNetCore.Mvc.svg)

The *GitHub Apps AspNetCore Mvc* is a component that allows the creation of GitHub Apps using .NET.

A GitHub App is a service that receives a post request from GitHub when certain actions are executed.

This component uses the Nuget Package [GitHubApps](https://www.nuget.org/packages/GitHubApps) and exposes a class `GitHubAppController`, which inherits from the `ControllerBase` class, exposing a MVC Controller.

Refer to the [API Documentation](https://olavodias.github.io/GitHubApps) to have a better understanding of each class and how to use it.

## Usage

### Creating the GitHub App

Add the nuget package [GitHubApps.AspNetCore.Mvc](https://www.nuget.org/packages/GitHubApps.AspNetCore.Mvc/) into your project. It will automatically add the [GitHubApps](https://www.nuget.org/packages/GitHubApps/), and [GitHubAuth](https://www.nuget.org/packages/GitHubAuth/) to your project.

Create your own `GitHubApp`, by creating a class that inherits from the `GitHubAppBase` class.

```cs
using GitHubApps;

// If your App does not need to connect to GitHub and do any action, you don't need authentication
public class MyGitHubAppWithoutAuthentication: GitHubAppBase
{
    public MyGitHubApp()
    {
        
    }

   //TODO: Override the virtual methods for each event
}

// If your App needs to connect to GitHub and do any action, you will need authentication
public class MyGitHubAppWithAuthentication: GitHubAppBase
{
    // override the property to a non-nullable property, this will prevent the warning CS8602 (Dereference of a possibly null reference) from being displayed
    public new IAuthenticator Authenticator { get; set; }

    // require the authenticator to come from dependency injection
    public MyGitHubApp(IAuthenticator authenticator)
    {
        Authenticator = authenticator;
    }

   //TODO: Override the virtual methods for each event
}
```

This class contains several virtual methods that can be overwritten to handle the proper event and action. For example, to handle the GitHub event `installation` with action `created`, you can override the `OnEventInstallationCreated()` method.

```cs
using GitHubApps;

public class MyGitHubApp: GitHubAppBase
{
   //TODO: Initialization of the class goes here
   public class MyGitHubApp()
   {
       ...
   }

   public override EventResult OnEventInstallationCreated(GitHubDelivery<GitHubEventInstallation> payload)
   {
      // Your code goes here...
   }
}
```

For more information, refer to the [API Documentation](https://olavodias.github.io/GitHubApps).

> Optionally, you can create a class that implements the `IGitHubApp` interface. However, this is not recommended as all the handling of the GitHub Post request will have to be handled manually.

### Creating the ASP.Net Core Web Api

After you created the `GitHubApp`, you need to implement the Controller.

In a ASP.Net Core Web Api, create a class that derives from the `GitHubAppController` class. This will allow you to define a `Route` for the class.

```cs
[ApiController]
[Route("GitHubApp")]
public class MyGitHubAppController: GitHubAppController
{
	public MyGitHubAppController(IGitHubApp gitHubApp): base(null, gitHubApp)
	{

	}
}
```

On your `Program.cs` class, add your custom GitHub App. This will configure Dependency Injection to recognize your GitHubApp.

```cs
// Program.cs

// Adds the GitHub App With the Default JWT (GitHubJwtWithRS256)
// This method automaticaly adds the IGitHubJwt into the dependency injection framework
builder.Services.AddGitHubApp<MyGitHubApp, AppAuthenticator>("path_to_pem_file.pem", 123456, provider =>
{
    var authenticator = new AppAuthenticator(provider.GetRequiredService<GitHubAuth.Jwt.IGitHubJwt>())
    {
        GetClient = () =>
        {
            //TODO: Implement a function to return a client to the GitHub API
        }
    };

    return authenticator;
});

// MyGitHubAppController.cs
public class MyGitHubAppController: GitHubAppController
{
    public MyGitHubAppController(IGitHubApp gitHubApp): base(null, gitHubApp)
    {

    }
}
```

There could be cases when you need to implement more than one GitHub App in the same project. If this is the case, you can call the method and add multiple GitHubApps. However, you will need to specify which GitHub App to use in your controller, instead of refering to the `IGitHubApp` interface.

```cs
// Program.cs

// Adds a Typed GitHub App
builder.Services.AddTypedGitHubApp<MyGitHubApp01, AppAuthenticator>("path_to_pem_file01.pem", 123456, provider =>
{
    var authenticator = new AppAuthenticator(provider.GetRequiredService<GitHubAuth.Jwt.IGitHubJwt>())
    {
        GetClient = () =>
        {
            //TODO: Implement a function to return a client to the GitHub API        }
    };

    return authenticator;
});

builder.Services.AddTypedGitHubApp<MyGitHubApp02, AppAuthenticator>("path_to_pem_file02.pem", 789012, provider =>
{
    var authenticator = new AppAuthenticator(provider.GetRequiredService<GitHubAuth.Jwt.IGitHubJwt>())
    {
        GetClient = () =>
        {
            //TODO: Implement a function to return a client to the GitHub API
        }
    };

    return authenticator;
});

// MyController01.cs
[ApiController]
[Route("GitHubApp01")]
public class MyController01: GitHubAppController
{
    // Note that you have to refer to the MyGitHubApp02, instead of the IGitHubApp
    public MyController01(MyGitHubApp01 gitHubApp01): base(null, gitHubApp01)
    {

    }
}

// MyController02.cs
[ApiController]
[Route("GitHubApp02")]
public class MyController02: GitHubAppController
{
    // Note that you have to refer to the MyGitHubApp02, instead of the IGitHubApp
    public MyController02(MyGitHubApp02 gitHubApp01): base(null, gitHubApp01)
    {

    }
}
```

## Links

This section contains links that are relevant to this repository.

| Link | Description |
| :-- | :-- |
| [GitHub Webhook Events and Payloads](https://docs.github.com/en/webhooks/webhook-events-and-payloads) | The specification of webhooks events and payloads |
| [Project Roadmap](https://github.com/users/olavodias/projects/2/views/2) | The project roadmap and expected delivery dates |
| [Project Board](https://github.com/users/olavodias/projects/2/views/1) | The project board containing the tasks and its status |
| [API Documentation](https://olavodias.github.io/GitHubApps) | The API Documentation |