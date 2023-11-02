// *****************************************************************************
// TestControllers.cs
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
using GitHubApps;
using GitHubApps.AspNetCore.Mvc;
using GitHubApps.Models;
using GitHubApps.Models.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GitHubApps.AspNetCore.Mvc.Testing;

/// <summary>
/// Represents a class to test the MVC controllers for the project <see cref="GitHubApps.AspNetCore.Mvc"/>
/// </summary>
[TestClass]
public class TestControllers
{

    [TestMethod]
    [DataRow(GitHubEvents.EVENT_INSTALLATION, GitHubEventActions.EVENT_ACTION_CREATED, DisplayName = "Installation Created")]
    [DataRow(GitHubEvents.EVENT_INSTALLATION, GitHubEventActions.EVENT_ACTION_DELETED, DisplayName = "Installation Deleted")]
    [DataRow(GitHubEvents.EVENT_INSTALLATION, GitHubEventActions.EVENT_ACTION_SUSPEND, DisplayName = "Installation Suspend")]
    [DataRow(GitHubEvents.EVENT_INSTALLATION, GitHubEventActions.EVENT_ACTION_UNSUSPEND, DisplayName = "Installation Unsuspend")]
    [DataRow(GitHubEvents.EVENT_FORK, null, DisplayName = "Fork")]
    public void TestGitHubAppController(string eventName, string? action)
    {
        // *************************************************************************************
        // Arrange
        // *************************************************************************************
        var sampleApp = new SampleGitHubApp();

        var httpContext = new DefaultHttpContext();

        // Setup File Name
        string requestFileName;
        string methodName;

        if (action is null)
        {
            requestFileName = $"{eventName}.json";
            methodName = $"onevent{eventName}".ToLower();
        }
        else
        {
            requestFileName = $"{eventName}.{action}.json";
            methodName = $"onevent{eventName}{action}".ToLower();
        }

        var requestFileFullPath = Path.Combine("Payload", eventName, requestFileName);

        // Load Payload Data
        var parsedFileData = TestHelper.GetPayloadFromFile(requestFileFullPath) ?? throw new NullReferenceException();

        Assert.IsNotNull(parsedFileData.Event);
        Assert.IsNotNull(parsedFileData.Payload);

        Assert.AreEqual(eventName.ToLower(), parsedFileData.Event.ToLower());

        // Generate Stream for the Request Body
        var stream = TestHelper.ConvertStringToStream(parsedFileData.Payload);
        httpContext.Request.Body = stream;
        httpContext.Request.ContentLength = stream.Length;

        // Create the Controller to Unit Test
        var controller = new GitHubAppController(null, sampleApp)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            }
        };

        // *************************************************************************************
        // Act
        // *************************************************************************************

        // Setup Request Headers
        controller.Request.Headers.Add(GitHubHeaders.HEADERS_GITHUB_EVENT, new Microsoft.Extensions.Primitives.StringValues(eventName));
        controller.Request.Method = "POST";

        controller.PostAsync().GetAwaiter().GetResult();

        // *************************************************************************************
        // Assert
        // *************************************************************************************

        Assert.AreEqual(methodName, sampleApp.LastMethodCalled);
       
    }

}



