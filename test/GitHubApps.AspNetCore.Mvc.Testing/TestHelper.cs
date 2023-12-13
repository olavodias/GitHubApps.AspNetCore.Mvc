// *****************************************************************************
// TestHelper.cs
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
using GitHubApps.Models.Events;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;

namespace GitHubApps.AspNetCore.Mvc.Testing;


/// <summary>
/// A static class containing methods to support Unit Testing
/// </summary>
public static class TestHelper
{

    /// <summary>
    /// Return the contents of a text file
    /// </summary>
    /// <param name="args">The path to the file</param>
    /// <returns>A string containing the contents of the file</returns>
    public static string GetFileData(params string[] args)
    {
        return File.ReadAllText(Path.Combine(args));
    }

    /// <summary>
    /// Returns a GitHub object or throw an exception if unable to create one
    /// </summary>
    /// <typeparam name="TGitHubObject">The type of the GitHub Object</typeparam>
    /// <param name="args">The path where the example file is located</param>
    /// <returns>A <see cref="GitHubDelivery{TGitHubObject}"/> of type <typeparamref name="TGitHubObject"/> or null if not found</returns>
    public static GitHubDelivery<TGitHubObject>? GetGitHubObject<TGitHubObject>(params string[] args)
        where TGitHubObject : GitHubEvent
    {
        try
        {
            var contents = GetFileData(args) ?? throw new InvalidDataException("Unable to load data from file");
            var delivery = GitHubDelivery<TGitHubObject>.ConvertFromJSON(contents);

            return delivery;
        }
        catch (Exception ex)
        {
            DumpException(ex, 0);
            return default;
        }
    }

    public static ParsedFileData? GetPayloadFromFile(params string[] args)
    {

        var serializedObject = (JObject?)Newtonsoft.Json.JsonConvert.DeserializeObject(GetFileData(args));

        if (serializedObject is null) return null;

        var parsedFileData = new ParsedFileData();

        // First node is the event
        if (serializedObject.First is JProperty jpFirst)
            parsedFileData.Event = jpFirst.Value.ToString();

        // Second node is the payload
        //if (serializedObject.Last?.First is JProperty jpLast)
        //    parsedFileData.Payload = jpLast.ToString();
        parsedFileData.Payload = serializedObject.Last?.First?.ToString();

        return parsedFileData;
    }

    public static Stream ConvertStringToStream(string input, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        var stream = new MemoryStream(encoding.GetByteCount(input));
        using var writer = new StreamWriter(stream, encoding, -1, true);
        writer.Write(input);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }

    /// <summary>
    /// Dump the exception into a formatted text
    /// </summary>
    /// <param name="e">The exception to dump</param>
    /// <param name="level">The level of indentation</param>
    public static void DumpException(Exception e, int level)
    {
        var sbDump = new StringBuilder();
        DumpException(sbDump, e, level);

        Debug.WriteLine(sbDump.ToString());
    }

    /// <summary>
    /// Dump the exception into a formatted text
    /// </summary>
    /// <param name="stringBuilder">The String Builder to append data to</param>
    /// <param name="e">The exception to dump</param>
    /// <param name="level">The level of indentation</param>
    private static void DumpException(StringBuilder stringBuilder, Exception e, int level)
    {
        var padding = (level > 0 ? '\u2514' : null) +
                      string.Empty.PadRight(level * 3, '\u2500') +
                      (level > 0 ? ' ' : null);

        stringBuilder.Append(padding);
        stringBuilder.Append("Exception......: ");
        stringBuilder.Append(e.GetType().Name);
        stringBuilder.AppendLine();

        stringBuilder.Append(padding);
        stringBuilder.Append("Message........: ");
        stringBuilder.Append(e.Message);
        stringBuilder.AppendLine();

        stringBuilder.Append(padding);
        stringBuilder.Append("Stack Trace....: ");
        stringBuilder.Append(e.StackTrace?.Replace("\n", "\n" + padding + string.Empty.PadRight(17, ' ')));

        if (e.InnerException is not null)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(padding);
            stringBuilder.Append("Inner Exception: ");
            stringBuilder.AppendLine();
            DumpException(stringBuilder, e.InnerException, level + 1);
        }

        stringBuilder.AppendLine();
    }
}

public struct ParsedFileData
{
    public string? Event;
    public string? Payload;
}

