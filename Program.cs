using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

class Program
{
    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = true
            });

        var page = await browser.NewPageAsync();

        // ---------------------------
        // 1. Login
        // ---------------------------
        await page.GotoAsync("https://demo.snipeitapp.com/login");

        await page.FillAsync("#username", "admin");
        await page.FillAsync("#password", "password");
        await page.ClickAsync("button[type='submit']");

        await page.WaitForURLAsync("**/dashboard");

        // ---------------------------
        // 2. Navigate to Create Asset
        // ---------------------------
        await page.ClickAsync("text=Assets");
        await page.ClickAsync("text=Create New");

        // ---------------------------
        // 3. Create Asset
        // ---------------------------
        await page.SelectOptionAsync("#model_select_id", new SelectOptionValue { Label = "MacBook Pro 13\"" });
        await page.SelectOptionAsync("#status_select_id", new SelectOptionValue { Label = "Ready to Deploy" });

        // Assign to first available user (random-ish for demo)
        await page.SelectOptionAsync("#assigned_to", new SelectOptionValue { Index = 1 });

        await page.ClickAsync("button:has-text('Save')");

        // ---------------------------
        // 4. Verify Asset Creation
        // ---------------------------
        await page.WaitForSelectorAsync("text=Asset created successfully");

        // Capture asset tag for validation
        var assetTag = await page.InnerTextAsync(".asset-tag");

        // ---------------------------
        // 5. Find Asset in List
        // ---------------------------
        await page.ClickAsync("text=Assets");
        await page.FillAsync("input[type='search']", assetTag);
        await page.WaitForSelectorAsync($"text={assetTag}");

        // ---------------------------
        // 6. Navigate to Asset Details
        // ---------------------------
        await page.ClickAsync($"text={assetTag}");

        // ---------------------------
        // 7. Validate Asset Details
        // ---------------------------
        await page.WaitForSelectorAsync("text=MacBook Pro 13\"");
        await page.WaitForSelectorAsync("text=Ready to Deploy");

        // ---------------------------
        // 8. Validate History Tab
        // ---------------------------
        await page.ClickAsync("text=History");
        await page.WaitForSelectorAsync("text=checked out");

        Console.WriteLine("✅ Asset created and validated successfully");

        await browser.CloseAsync();
    }
}
