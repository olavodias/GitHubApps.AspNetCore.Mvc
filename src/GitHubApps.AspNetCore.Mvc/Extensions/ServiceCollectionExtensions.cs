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
using System;
using GitHubApps.Models;
using Microsoft.Extensions.DependencyInjection;

#if NET6_0_OR_GREATER

using System.Diagnostics.CodeAnalysis;

#endif

namespace GitHubApps.AspNetCore.Mvc.Extensions;

/// <summary>
/// Extension methods for adding GitHub Apps to an <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{

#if NET6_0_OR_GREATER

    /// <summary>
    /// Adds a singleton GitHub App of the type specified in <typeparamref name="TGitHubApp"/> to the  
    /// specified <see cref="IServiceCollection"/>
    /// </summary>
    /// <typeparam name="TGitHubApp">The type of the GitHub App</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to</param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IServiceCollection AddGitHubApp<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TGitHubApp>(this IServiceCollection services)
        where TGitHubApp: GitHubAppBase
    {
        return services.AddSingleton<TGitHubApp>();
    }

#else

    /// <summary>
    /// Adds a singleton GitHub App of the type specified in <typeparamref name="TGitHubApp"/> to the  
    /// specified <see cref="IServiceCollection"/>
    /// </summary>
    /// <typeparam name="TGitHubApp">The type of the GitHub App</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to</param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IServiceCollection AddGitHubApp<TGitHubApp>(this IServiceCollection services)
        where TGitHubApp : GitHubAppBase
    {
        return services.AddSingleton<TGitHubApp>();
    }


#endif
}

