
using Microsoft.Playwright;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace Interoperability_selenium_playwright;

[TestClass]
public class RawInteroperability
{

    [TestMethod]
    public async Task LinearSeleniumPlaywright()
    {
        var address = "http://localhost:5959";
        var options = new ChromeOptions();

        options.AddArgument("--remote-debugging-port=5959");
        options.AddArgument("--disable-search-engine-choice-screen");
        var selenium = new ChromeDriver(options);

        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.ConnectOverCDPAsync(address);
        var page = browser.Contexts[0].Pages[0];


        await page.GotoAsync("https://www.saucedemo.com/v1/");
        selenium.FindElement(By.Id("user-name")).SendKeys("standard_user");
        await page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        selenium.FindElement(By.XPath("//*[@type='submit']")).Click();

        await browser.DisposeAsync();
        selenium.Dispose();
        selenium.Quit();


    }

    [TestMethod]
    public async Task LinearPlaywrightSelenium()
    {

        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false, Args = ["--remote-debugging-port=5959"], Channel = "chrome" });
        var page = await browser.NewPageAsync();

        var address = "localhost:5959";
        var options = new ChromeOptions();
        options.DebuggerAddress = address;

        var selenium = new ChromeDriver(options);


        await page.GotoAsync("https://www.saucedemo.com/v1/");
        selenium.FindElement(By.Id("user-name")).SendKeys("standard_user");
        await page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        selenium.FindElement(By.XPath("//*[@type='submit']")).Click();

        selenium.Dispose();
        await browser.CloseAsync();
    }
}
