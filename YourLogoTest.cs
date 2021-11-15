using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using OpenQA.Selenium.Support.UI;
using System.IO;
using Authlete.Util;
using ExcelDataReader;
using OpenQA.Selenium.Edge;

public class YourLogoTest
{

    IWebDriver driver;
    YourLogoCreateAccountPage yourLogoCreateAccountPage;
    DataTable dataTable = new DataTable();
    readonly string browserPath = "C:\\Users\\yshoker\\Desktop\\YourLogo";
    readonly string excelPath = "C:\\Users\\yshoker\\Desktop\\YourLogo\\test.xlsx";


    [OneTimeSetUp]
    public void Setup()
    {
        TestProperties.ReadProp();

        driver = new EdgeDriver(EdgeDriverService.CreateDefaultService(browserPath, "msedgedriver.exe")); 
        driver.Manage().Cookies.DeleteAllCookies();
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl(TestProperties.properties["LoginUrl"]);
        
        // INSERT EXCEL DATA INTO DATA TABLE
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader;
        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        dataTable = result.Tables[0];
        
        // SET FIRST ROW AS COULMNS
        for (int i=0; i<dataTable.Columns.Count; i++)
        {
            dataTable.Columns["Column"+i].ColumnName = dataTable.Rows[0][i].ToString();
        }
        // DELETE FIRST ROW 
        dataTable.Rows[0].Delete();
        dataTable.AcceptChanges();
    }


    [Test, Category("logo"), Order(1)]
    public void TestEnterValidEmail()
    {
        driver.FindElement(By.XPath(TestProperties.properties["SignInBut"])).Click();
        driver.FindElement(By.XPath(TestProperties.properties["EmailSignIn"])).Clear();
        driver.FindElement(By.XPath(TestProperties.properties["EmailSignIn"])).SendKeys(dataTable.Rows[0]["email"].ToString());
        driver.FindElement(By.XPath(TestProperties.properties["EmailSignInBut"])).Click();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        Assert.DoesNotThrow(() => WaitToElement(driver, driver.FindElement(By.XPath(TestProperties.properties["Title"]))));
    }


    [Test, Category("logo"), Order(2)]
    public void TestValidRegister()
    {
        yourLogoCreateAccountPage = new YourLogoCreateAccountPage(driver);

        yourLogoCreateAccountPage.chooseGender();
        yourLogoCreateAccountPage.TypeFirstName(dataTable.Rows[0]["FirstName"].ToString());
        yourLogoCreateAccountPage.TypeLastName(dataTable.Rows[0]["LastName"].ToString());
        yourLogoCreateAccountPage.TypePassword(dataTable.Rows[0]["pass"].ToString());
        yourLogoCreateAccountPage.TypePhone(dataTable.Rows[0]["phone"].ToString());
        yourLogoCreateAccountPage.TypeEmail(dataTable.Rows[0]["email"].ToString());
        yourLogoCreateAccountPage.InputDateOfBirth();
        yourLogoCreateAccountPage.InputAdress(dataTable.Rows[0]["adress"].ToString(), dataTable.Rows[0]["city"].ToString(),
                                                                                            dataTable.Rows[0]["zip"].ToString());
        yourLogoCreateAccountPage.ClickRegisterButton();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(6);
        Assert.DoesNotThrow(() => WaitToElement(driver, driver.FindElement(By.XPath(TestProperties.properties["MyAccountHeader"]))));
    }

    [OneTimeTearDown]
    public void Finish()
    {
        driver.Quit();
    }

    public static void WaitToElement(IWebDriver driver, IWebElement elem)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(90));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        wait.IgnoreExceptionTypes(typeof(ElementClickInterceptedException));
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        wait.Until(ExpectedConditions.ElementToBeClickable(elem));

    }
}

class YourLogoCreateAccountPage
{

    IWebDriver driver;

    public YourLogoCreateAccountPage(IWebDriver driver)
    {
        this.driver = driver;
    }

     public void chooseGender()
     {
          driver.FindElement(By.XPath(TestProperties.properties["Title"])).Click();
     }

    public void TypeFirstName(string userEmail)
    {
        driver.FindElement(By.XPath(TestProperties.properties["FirstName"])).Clear();
        driver.FindElement(By.XPath(TestProperties.properties["FirstName"])).SendKeys(userEmail);
        }
    public void TypeLastName(string userEmail)
    {
        driver.FindElement(By.XPath(TestProperties.properties["LastName"])).Clear();
        driver.FindElement(By.XPath(TestProperties.properties["LastName"])).SendKeys(userEmail);
    }

    public void TypeEmail(string userEmail)
    {
        driver.FindElement(By.XPath(TestProperties.properties["Email"])).Clear();
        driver.FindElement(By.XPath(TestProperties.properties["Email"])).SendKeys(userEmail);
    }

    public void TypePassword(string passsword)
    {
        driver.FindElement(By.XPath(TestProperties.properties["Password"])).Clear();
        driver.FindElement(By.XPath(TestProperties.properties["Password"])).SendKeys(passsword);

    }

    public void TypePhone(string passsword)
    {
        driver.FindElement(By.XPath(TestProperties.properties["MobilePhone"])).Clear();
        driver.FindElement(By.XPath(TestProperties.properties["MobilePhone"])).SendKeys(passsword);

    }
    
    public void InputDateOfBirth()
    {
        SelectElement DateOfBirth = new SelectElement(driver.FindElement(By.XPath(TestProperties.properties["DateofBirthDay"])));
        DateOfBirth.SelectByIndex(2);
        DateOfBirth = new SelectElement(driver.FindElement(By.XPath(TestProperties.properties["DateofBirthMonth"])));
        DateOfBirth.SelectByIndex(2);
        DateOfBirth = new SelectElement(driver.FindElement(By.XPath(TestProperties.properties["DateofBirthYear"])));
        DateOfBirth.SelectByIndex(2);
    }

    public void InputAdress(string adress, string city,  string zipCode)
    {
        driver.FindElement(By.XPath(TestProperties.properties["Address"])).Clear();
        driver.FindElement(By.XPath(TestProperties.properties["Address"])).SendKeys(adress);

        driver.FindElement(By.XPath(TestProperties.properties["City"])).Clear();
        driver.FindElement(By.XPath(TestProperties.properties["City"])).SendKeys(city);

        SelectElement Sadress = new SelectElement(driver.FindElement(By.XPath(TestProperties.properties["State"])));
        Sadress.SelectByIndex(2);

        driver.FindElement(By.XPath(TestProperties.properties["Zip_PostalCode"])).Clear();
        driver.FindElement(By.XPath(TestProperties.properties["Zip_PostalCode"])).SendKeys(zipCode);

        Sadress = new SelectElement(driver.FindElement(By.XPath(TestProperties.properties["Country"])));
        Sadress.SelectByIndex(1);

    }

    public void ClickRegisterButton()
    {
        driver.FindElement(By.XPath(TestProperties.properties["submitBtn"])).Click();
    }
   
}


class TestProperties
{
    public static IDictionary<string, string> properties = new Dictionary<string, string>();
    private static readonly string file = "C:\\Users\\yshoker\\Desktop\\YourLogo\\properties.txt";  

    public static void ReadProp()
    {
        using (TextReader reader = new StreamReader(file))
        {
            properties = PropertiesLoader.Load(reader);
        }

    }
}

