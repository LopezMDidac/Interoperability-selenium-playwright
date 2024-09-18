using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interoperability_selenium_playwright.POM;
public class LoginPage
{
    private IWebDriver _driver;
    private By _userNameInput = By.Id("user-name");
    private By _passwordInput = By.Name("password");
    private By _submitBtn = By.XPath("//*[@type='submit']");
    private By _loginContainer = By.ClassName("login_wrapper");

    public LoginPage(IWebDriver driver)
    {
        _driver = driver;
    }

    public bool IsLoaded()
    {
        try
        {
            _driver.FindElement(_loginContainer);
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }

    public void SetUserName(string value)
    {
        _driver.FindElement(_userNameInput).SendKeys(value);
    }

    public void SetPassword(string value)
    {
        _driver.FindElement(_passwordInput).SendKeys(value);
    }

    public void SubmitCredentials()
    {
        _driver.FindElement(_submitBtn).Click();
    }
}