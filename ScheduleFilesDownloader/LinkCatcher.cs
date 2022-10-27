using PuppeteerSharp;

namespace ScheduleFilesDownloader;

public static class LinkCatcher
{
    public static async Task<IEnumerable<string?>> Catch()
    {
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        
        await page.GoToAsync(Environment.GetEnvironmentVariable("LINK_TO_UNIVERSITY_SITE"));

        var links = page.XPathAsync("//*[@class=\"panel-body\"]//tr/td[1]/a").Result
            .Select(el => el
                .GetPropertyAsync("href").Result
                .ToString()?
                .Split('=')
                .Last())
            .SkipLast(1);

        return links;
    }
}

