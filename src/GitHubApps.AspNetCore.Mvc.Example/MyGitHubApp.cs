// *****************************************************************************
// MyGitHubApp.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using GitHubApps.Models;
using GitHubApps.Models.Events;
using GitHubAuth;

namespace GitHubApps.AspNetCore.Mvc.Example;

public class MyGitHubApp : GitHubAppBase
{
    public MyGitHubService Service { get; init; }
    public new IAuthenticator Authenticator { get; set; }

    public MyGitHubApp(IAuthenticator authenticator, MyGitHubService gitHubService)
    {
        Authenticator = authenticator;
        Service = gitHubService;
    }

    public override EventResult OnEventIssuesOpened(GitHubDelivery<GitHubEventIssuesChanged> payload)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(payload.Payload);
            ArgumentNullException.ThrowIfNull(payload.Payload.Installation);

            var authData = Authenticator.GetToken<long>(payload.Payload.Installation.ID);

            // Set The Comments Content
            var comments = "{\"body\" : \"Comment made by GitHub App\"}";

            Service.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authData.TokenType.Mode, authData.Token);
            var response = Service.Client.PostAsync(payload.Payload.Issue!.CommentsURL, new StringContent(comments, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();

            if (response is null) return EventResult.ErrorEventResult;
            response.EnsureSuccessStatusCode();

            return EventResult.SuccessEventResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return EventResult.ErrorEventResult;
        }
    }

}

