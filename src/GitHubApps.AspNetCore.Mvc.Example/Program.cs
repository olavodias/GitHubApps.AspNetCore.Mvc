﻿using GitHubApps;
using GitHubApps.AspNetCore.Mvc.Example;
using GitHubApps.AspNetCore.Mvc.Extensions;
using GitHubAuth;
using Microsoft.AspNetCore.Routing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adds the GitHub App
/*builder.Services.AddGitHubApp(options =>
{
    var jwt = new GitHubAuth.Jwt.GitHubJwtWithRS256("appkey.pem", 383002);
    var authenticator = new GitHubAuth.AppAuthenticator(jwt);
    
    return new MyGitHubApp(authenticator);    
});*/

/*
builder.Services.AddSingleton<GitHubAuth.Jwt.IGitHubJwt>(new GitHubAuth.Jwt.GitHubJwtWithRS256("appkey.pem", 383002));
builder.Services.AddSingleton<GitHubAuth.IAuthenticator, GitHubAuth.AppAuthenticator>();
builder.Services.AddSingleton<IGitHubApp, MyGitHubApp>();
*/

builder.Services.AddHttpClient<GitHubService>(c => c.BaseAddress = new Uri("https://api.github.com/"));
builder.Services.AddGitHubApp<MyGitHubApp, AppAuthenticator>("appkey.pem", 383002, provider =>
{
    var authenticator = new AppAuthenticator(provider.GetRequiredService<GitHubAuth.Jwt.IGitHubJwt>())
    {
        GetClient = () =>
        {
            var currentClient = provider.GetService<GitHubService>();
            return currentClient is null
                ? throw new NullReferenceException("Unable to create a Http Client for GitHub")
                : currentClient.Client;
        }
    };

    return authenticator;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

