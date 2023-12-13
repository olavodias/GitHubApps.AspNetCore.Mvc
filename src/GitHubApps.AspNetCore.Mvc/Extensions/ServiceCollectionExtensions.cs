// *****************************************************************************
// Extensions.cs
//
// Author:
//       Olavo Henrique Dias <olavodias@gmail.com>
//
// Copyright (c) 2023 Olavo Henrique Dias
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// *****************************************************************************

#if NET6_0_OR_GREATER

using System;
using Microsoft.Extensions.DependencyInjection;

using GitHubAuth.Jwt;
using GitHubAuth;

namespace GitHubApps.AspNetCore.Mvc.Extensions;

/// <summary>
/// Extension methods for adding GitHub Apps to an <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a <see cref="IGitHubJwt"/> object to be used by <see cref="IAuthenticator">Authenticators</see> when a JWT is needed
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> where the service is being added to</param>
    /// <param name="implementationFactory">The factory to Initialize the <see cref="IGitHubJwt"/></param>
    /// <returns></returns>
    public static IServiceCollection AddGitHubJwt(this IServiceCollection services, Func<IServiceProvider, IGitHubJwt> implementationFactory)
    {
        return services.AddSingleton<IGitHubJwt>(implementationFactory);
    }

    /// <summary>
    /// Adds a GitHub App into Dependency Injection
    /// </summary>
    /// <typeparam name="TGitHubApp">The type of the GitHub App</typeparam>
    /// <typeparam name="TAuthenticator">The type of the Authenticator</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> where the service is being added to</param>
    /// <returns>The Service Collection</returns>
    public static IServiceCollection AddGitHubApp<TGitHubApp, TAuthenticator>(this IServiceCollection services)
        where TGitHubApp : class, IGitHubApp
        where TAuthenticator : class, GitHubAuth.IAuthenticator
    {
        return services.AddSingleton<IAuthenticator, TAuthenticator>()
            .AddScoped<IGitHubApp, TGitHubApp>();
    }

    /// <summary>
    /// Adds a GitHub App into Dependency Injection, using the <see cref="GitHubJwtWithRS256"/> class to generate the JWT
    /// </summary>
    /// <typeparam name="TGitHubApp">The type of the GitHub App</typeparam>
    /// <typeparam name="TAuthenticator">The type of the Authenticator</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> where the service is being added to</param>
    /// <param name="privateKeyFileName">The path to the PEM file</param>
    /// <param name="appId">The GitHub Application ID</param>
    /// <param name="authenticatorFactory">A factory to initialize the <typeparamref name="TAuthenticator"/></param>
    /// <returns>The Service Collection</returns>
    public static IServiceCollection AddGitHubApp<TGitHubApp, TAuthenticator>(this IServiceCollection services, string privateKeyFileName, long appId, Func<IServiceProvider, TAuthenticator> authenticatorFactory)
        where TGitHubApp : class, IGitHubApp
        where TAuthenticator : class, GitHubAuth.IAuthenticator
    {
        return services.AddSingleton<IGitHubJwt>(new GitHubJwtWithRS256(privateKeyFileName, appId))
            .AddSingleton<IAuthenticator, TAuthenticator>(authenticatorFactory)
            .AddScoped<IGitHubApp, TGitHubApp>();
    }

    /// <summary>
    /// Adds a singleton GitHub App for dependency injection
    /// </summary>
    /// <typeparam name="TGitHubApp">The type of the GitHub App</typeparam>
    /// <typeparam name="TAuthenticator">The type of the Authenticator</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> where the service is being added to</param>
    /// <param name="privateKeyFileName">The path to the PEM file</param>
    /// <param name="appId">The GitHub Application ID</param>
    /// <param name="authenticatorFactory">A factory to initialize the <typeparamref name="TAuthenticator"/></param>
    /// <returns>The Service Collection</returns>
    /// <remarks>This method will add a GitHub App and set it to be used by dependency injection when reffering to the type defined at <typeparamref name="TGitHubApp"/>, insteadf of the <see cref="IGitHubApp"/></remarks>
    public static IServiceCollection AddTypedGitHubApp<TGitHubApp, TAuthenticator>(this IServiceCollection services, string privateKeyFileName, long appId, Func<IServiceProvider, TAuthenticator> authenticatorFactory)
        where TGitHubApp : class, IGitHubApp
        where TAuthenticator : class, GitHubAuth.IAuthenticator
    {
        return services.AddSingleton<IGitHubJwt>(new GitHubJwtWithRS256(privateKeyFileName, appId))
            .AddSingleton<IAuthenticator, TAuthenticator>(authenticatorFactory)
            .AddScoped<TGitHubApp>();
    }
}

#endif
