
using Interoperability_selenium_playwright.POM;
using Microsoft.Playwright;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace Interoperability_selenium_playwright;

[TestClass]
public class InteroperabilityWithPOM
{
    private IWebDriver _selenium;
    private IBrowser _playwright;

    private LoginPage _loginPage;
    private ProductsPage _productsPage;
    private ProductDetailPage _productDetailPage;

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

    [TestCleanup]
    public async Task TestCleanup()
    {
        await _playwright.DisposeAsync();
        _selenium.Dispose();
        _selenium.Quit();

    }

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
}
