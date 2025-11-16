using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Lab5CPP;

class Program
{
    static void Main(string[] args)
    {
        IWebDriver driver = new ChromeDriver("D:\\chromedriver-win64");

        var test = new YouTubeTest(driver);
        test.RunTest();
    }
}
