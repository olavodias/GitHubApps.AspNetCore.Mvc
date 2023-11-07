// *****************************************************************************
// GitHubAppController.cs
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GitHubApps.AspNetCore.Mvc;

/// <summary>
/// Represents a controller to handle a GitHub App Post Request
/// </summary>
/// <remarks>Inherit from this class and implement your own controller</remarks>
public abstract class GitHubAppController: ControllerBase
{
	/// <summary>
	/// Reference to the <see cref="IGitHubApp"/> to be implemented
	/// </summary>
	protected readonly IGitHubApp GitHubApp;
	/// <summary>
	/// The Logger for performing logging
	/// </summary>
	protected readonly ILogger? Logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubAppController"/> class
    /// </summary>
    /// <param name="logger">The logger for performing logging</param>
    /// <param name="gitHubApp">The app to be implemented</param>
    public GitHubAppController(IGitHubApp gitHubApp, ILogger? logger = null)
	{
        Logger = logger;
        GitHubApp = gitHubApp;
	}

	/// <summary>
	/// The handler for a post request
	/// </summary>
	/// <returns>The results of the post request</returns>
	/// <exception cref="TaskCanceledException">Thrown when the execution is cancelled by the function <see cref="OnBeforeProcessRequest(Dictionary{string, string}, string)"/></exception>
	[HttpPost]
	public virtual async Task<IActionResult> PostAsync()
	{
		try
		{
			// Header dictionary
			Dictionary<string, string> headers = new();

            // Read headers
			foreach (var r in Request.Headers)
                headers.Add(r.Key, r.Value);

            // Read request body into a string
            if (!Request.Body.CanSeek)
				Request.EnableBuffering();

			Request.Body.Position = 0;
            using var reader = new StreamReader(Request.Body, true);
			var requestBody = await reader.ReadToEndAsync().ConfigureAwait(true);
			Request.Body.Position = 0;

			// Process the request itself
			if (!OnBeforeProcessRequest(headers, requestBody))
				throw new TaskCanceledException($"The request execution pipeline was cancelled by the \"{nameof(OnBeforeProcessRequest)}\" function");

            var eventResult = await GitHubApp.ProcessRequest(headers, requestBody);
            OnAfterProcessRequest(headers, requestBody, eventResult);

            return Ok();
        }
        catch (Exception ex)
		{
			Logger?.LogError(LoggingEvents.GitHubAppControllerPost, ex, $"Unable to run '{nameof(GitHubApp.ProcessRequest)}'");
			return new StatusCodeResult(StatusCodes.Status500InternalServerError);
		}
	}

	/// <summary>
	/// Method called right before the <see cref="GitHubAppBase.ProcessRequest(Dictionary{string, string}, string)"/> method is called
	/// </summary>
	/// <param name="headers">A dictionary containig the headers of the request</param>
	/// <param name="requestBody">The request body</param>
	/// <returns>A boolean to define whether to continue processing the request or not</returns>
	/// <remarks>Throw exceptions in this method to suspend the processing of the request</remarks>
	protected virtual bool OnBeforeProcessRequest(Dictionary<string, string> headers, string requestBody)
	{
		return true;
	}

    /// <summary>
    /// Method called right after the <see cref="GitHubAppBase.ProcessRequest(Dictionary{string, string}, string)"/> method is called
    /// </summary>
	/// <param name="headers">A dictionary containig the headers of the request</param>
	/// <param name="requestBody">The request body</param>
    /// <param name="eventResult">The results of the event processing</param>
    /// <remarks>Avoid throwing exceptions from this method because the request itself has already been processed</remarks>
    protected virtual void OnAfterProcessRequest(Dictionary<string, string> headers, string requestBody, EventResult eventResult)
	{

	}
}
