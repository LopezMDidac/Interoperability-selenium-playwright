using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interoperability_selenium_playwright.POM;

public class ProductsPage
{
    private IPage _page;

    private ILocator _inventory_title { get { return _page.GetByText("Products"); } }
    private ILocator _inventory_item { get { return _page.Locator(".inventory_item"); } }
    private ILocator _inventory_item_name { get { return _page.Locator(".inventory_item_name"); } }
    private ILocator _inventory_item_price { get { return _page.Locator(".inventory_item_price"); } }


    public ProductsPage(IPage page)
    {
        _page = page;
    }

    public bool IsLoaded()
    {
        return _inventory_title.IsVisibleAsync().Result;
    }

    public string GetProductPrice(string productName)
    {
        var item = _inventory_item.Filter(new() { Has = _inventory_item_name.Filter(new() { HasText = productName }) });
        var price = item.Locator(_inventory_item_price).TextContentAsync().Result;
        return price;
    }

    public void OpenProductDetails(string productName)
    {
        var item = _inventory_item.Filter(new() { Has = _inventory_item_name.Filter(new() { HasText = productName }) });
        item.Locator(_inventory_item_name).ClickAsync().Wait();
    }





}
