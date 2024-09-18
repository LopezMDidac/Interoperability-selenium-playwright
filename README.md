# Interoperability-selenium-playwright
This repo demonstrates how to configure a test framework that ensures interoperability between Selenium and Playwright.

There is an example of direct interoperability on the same page and an example of interoperability using POM.

> [!IMPORTANT]  
> Crucial information necessary for users to succeed.
> This repository focuses on demonstrating interoperability capabilities rather than developing a correct page object model or architectural approach. A flexible implementation of the configuration is also not considered in this repo.
> This example is executed with Chrome and potentially works with any browser that uses Chromium.

## How to use
- Download the repo
- Restore Nuget
- build
- Run the tests

## Configuration
The main approach is to run a browser for testing with Selenium or Playwright and connect the other through the DevTools protocols.

### Launch Selenium and add Playwright

#### Configure Selenium
It's important to open the debugging port of the WebDriver to allow Playwright to connect.

```csharp
var options = new ChromeOptions();

options.AddArgument("--remote-debugging-port=5959");
options.AddArgument("--disable-search-engine-choice-screen");
var selenium = new ChromeDriver(options);
```

#### Configure Playwright
We create a new Chromium instance, but this time we ask Playwright to connect to an existing instance through the CDP by using the address and port opened by Selenium.

```csharp
var address = "http://localhost:5959";

var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.ConnectOverCDPAsync(address);
var page = browser.Contexts[0].Pages[0];
```

### Launch Playwright and add Selenium

#### Configure Playwright
It's important to open the debugging port of the WebDriver to allow Selenium to connect.

```csharp
var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false, Args = ["--remote-debugging-port=5959"], Channel = "chrome" });
var page = await browser.NewPageAsync();
```

#### Configure Selenium
We should configure the debuggerAddress of the running instance. After that, Selenium will connect to the browser instance raised by Playwright.

```csharp
var address = "localhost:5959";
var options = new ChromeOptions();
options.DebuggerAddress = address;

var selenium = new ChromeDriver(options);
```

## Single page Interoperability
Regardless of how the configuration is done, once both technologies point to the same browser instance, we will be able to interact with it interchangeably.

In the following example, we navigate using Playwright, fill a field using Selenium, another field using Playwright, and finally click using Selenium to achieve a successful login.

```csharp
await page.GotoAsync("https://www.saucedemo.com/v1/");
selenium.FindElement(By.Id("user-name")).SendKeys("standard_user");
await page.Locator("[data-test=\\"password\\"]").FillAsync("secret_sauce");
selenium.FindElement(By.XPath("//*[@type='submit']")).Click();
```

See the full example at RawInteroperability.cs

## Interoperability with POM
Let's see the potential of this interoperability from a Page Object Model scheme.

Here we could have some pages implemented with Selenium and others with Playwright, leaving the test agnostic to the technology that finally implements the communication with the system under test.

### Initialize
```csharp
[TestInitialize]
public async Task TestInitialize()
{
    var address = "http://localhost:5959";
    var options = new ChromeOptions();

    options.AddArgument("--remote-debugging-port=5959");
    options.AddArgument("--disable-search-engine-choice-screen");
    _selenium = new ChromeDriver(options);

    var playwright = await Playwright.CreateAsync();
    _playwright = await playwright.Chromium.ConnectOverCDPAsync(address);
    var currentPage = _playwright.Contexts[0].Pages[0];

    _loginPage = new(_selenium);
    _productsPage = new(currentPage);
    _productDetailPage = new(_selenium);
    await currentPage.GotoAsync("https://www.saucedemo.com/v1/");
}
```

### Test
```csharp
[TestMethod]
public void CheckSamePriceInInventoryAndDetails()
{
    var productName = "Sauce Labs Onesie";
    Assert.IsTrue(_loginPage.IsLoaded());
    _loginPage.SetUserName("standard_user");
    _loginPage.SetPassword("secret_sauce");
    _loginPage.SubmitCredentials();

    Assert.IsTrue(_productsPage.IsLoaded());
    var price = _productsPage.GetProductPrice(productName);
    _productsPage.OpenProductDetails(productName);

    Assert.IsTrue(_productDetailPage.IsLoaded());
    var detailedProduct = _productDetailPage.GetProductName();
    var detailedPrice = _productDetailPage.GetProductPrice();

    Assert.AreEqual(productName, detailedProduct);
    Assert.AreEqual(price, detailedPrice);
}
```
The test has no dependency on the specific implementation of each page, making a migration process between technologies much more feasible.

See the full example at InteroperabilityWithPOM.cs

Happy testing"
