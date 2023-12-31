﻿using System.Reflection;
using Ardalis.GuardClauses;
using Microsoft.Playwright;

namespace lalalai;

public static class Worker
{
    private const string Resource = "https://www.lalal.ai/";

    //https://github.com/microsoft/playwright-dotnet/issues/2259#issuecomment-1649784033
    public static void SetDefaultWebFirstAssertionTimeout(float timeout)
    {
        Guard.Against.NegativeOrZero(timeout, nameof(timeout),
            "The default timeout for web-first assertions must be greater than zero.");

        var assembly = Assembly.LoadFrom("Microsoft.Playwright.dll");
        var type = assembly.GetType("Microsoft.Playwright.Core.AssertionsBase");

        if (type is null)
        {
            throw new InvalidOperationException(
                "Unable to set the default web first assertion timeout because the proper Playwright type, " +
                "AssertionsBase, could not be found.");
        }

        var method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .SingleOrDefault(m => m.Name is "SetDefaultTimeout");

        if (method is null)
        {
            throw new InvalidOperationException(
                "Unable to set the default web first assertion timeout because the proper method could not be found.");
        }

        method.Invoke(null, new object[] { timeout });
    }
    public static async Task<Dictionary<string, string>> Beast()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(Resource);
        var fileChooser = await page.RunAndWaitForFileChooserAsync(async () =>
        {
            await page.GetByText("Select Files").First.ClickAsync();
        });
        await fileChooser.SetFilesAsync("test.mp3");
        var locator = page.Locator("h2[class='xxlarge-font font-color--primary promo__title']").First;
        await Assertions.Expect(locator).ToHaveTextAsync("Your previews are ready!");
        var vocalsUrl = await page.WaitForResponseAsync(r => r.Url.Contains("d.lalal.ai") && r.Url.Contains("/vocals"));
        var instrumentalUrl = await page.WaitForResponseAsync(r => r.Url.Contains("d.lalal.ai") && r.Url.Contains("/no_vocals"));
        return new Dictionary<string, string>() { { "vocals", vocalsUrl.Url }, { "instrumental", instrumentalUrl.Url } };
    }
}