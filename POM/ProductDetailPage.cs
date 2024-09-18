using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interoperability_selenium_playwright.POM;

internal class ProductDetailPage
{
    private IWebDriver _driver;
    private By _detailsName = By.ClassName("inventory_details_name");
    private By _detailsPrice = By.ClassName("inventory_details_price");
    private By _detailsContainer = By.ClassName("inventory_details");

    public ProductDetailPage(IWebDriver driver)
    {
        _driver = driver;
    }

    public bool IsLoaded()
    {
        try
        {
            _driver.FindElement(_detailsContainer);
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }

    public string GetProductPrice()
    {
        return _driver.FindElement(_detailsContainer).FindElement(_detailsPrice).Text;
    }

    public string GetProductName()
    {
        return _driver.FindElement(_detailsContainer).FindElement(_detailsName).Text;
    }
}
