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

Add the nuget package [GitHubApps.AspNetCore.Mvc](https://www.nuget.org/packages/GitHubApps.AspNetCore.Mvc/) into your project.

Create your own `GitHubApp`, by creating a class that inherits from the `GitHubAppBase` class.

```cs
using GitHubApps;

public class MyGitHubApp: GitHubAppBase
{
   //TODO: Override the virtual methods for each event
}
```

This class contains several virtual methods that can be overwritten to handle the proper event and action. For example, to handle the GitHub event `installation` with action `created`, you can override the `OnEventInstallationCreated()` method.

```cs
using GitHubApps;

public class MyGitHubApp: GitHubAppBase
{
   public override EventResult OnEventInstallationCreated(GitHubDelivery<GitHubEventInstallation> payload)
   {
      // Your code goes here...
   }
}
```

For more information, refer to the [API Documentation](https://olavodias.github.io/GitHubApps).

The next step is to use the `GitHubAppController`.

```cs
// Program.cs

app.MapControllerROute(
    name: "MyGitHubApp",
    pattern: new { controller = "GitHubAppController" }
);

```

You can also implement your own controller.

```cs
// MyController.cs
public class MyController: GitHubAppController
{
    public MyController(IGitHubApp gitHubApp): base(null, gitHubApp)
    {

    }
}
```

On your `Program.cs` class, add your custom GitHub App. This will configure Dependency Injection to recognize your GitHubApp.

If you creating a custom controller, add the `IGitHubApp` interface to your controller constructor.

```cs
// Program.cs
services.builder.AddGitHubApp<MyGitHubApp>();

// MyController.cs
public class MyController: GitHubAppController
{
    public MyController(IGitHubApp gitHubApp): base(null, gitHubApp)
    {

    }
}
```

There could be cases when you need to implement more than one GitHub App in the same project. If this is the case, you can call the method and add multiple GitHubApps. However, you will need to specify which GitHub App to use in your controller, instead of refering to the `IGitHubApp`.

```cs
// Program.cs
services.builder.AddGitHubApp<MyGitHubApp01>();
services.builder.AddGitHubApp<MyGitHubApp02>();

// MyController01.cs
public class MyController01: GitHubAppController
{
    public MyController01(MyGitHubApp01 gitHubApp01): base(null, gitHubApp01)
    {

    }
}

// MyController02.cs
public class MyController02: GitHubAppController
{
    public MyController02(MyGitHubApp02 gitHubApp01): base(null, gitHubApp01)
    {

    }
}
```

> Optionally, you can create a class that implements the `IGitHubApp` interface. However, this is not recommended as all the handling of the GitHub Post request will have to be handled manually.

## Links

This section contains links that are relevant to this repository.

| Link | Description |
| :-- | :-- |
| [GitHub Webhook Events and Payloads](https://docs.github.com/en/webhooks/webhook-events-and-payloads) | The specification of webhooks events and payloads |
| [Project Roadmap](https://github.com/users/olavodias/projects/2/views/2) | The project roadmap and expected delivery dates |
| [Project Board](https://github.com/users/olavodias/projects/2/views/1) | The project board containing the tasks and its status |
| [API Documentation](https://olavodias.github.io/GitHubApps) | The API Documentation |