using System.IO;
using Lab7.Base;
using Xunit;

namespace Lab7.Hooks
{
    /// <summary>
    /// xUnit collection fixture for test setup and teardown
    /// </summary>
    [CollectionDefinition("GoogleSearchTests")]
    public class TestCollectionFixture : ICollectionFixture<WebDriverFixture>
    {
    }

    /// <summary>
    /// xUnit test collection setup/teardown
    /// </summary>
    [Collection("GoogleSearchTests")]
    public class TestCollectionSetup : IDisposable
    {
        private static bool _executedFinalReport = false;

        public TestCollectionSetup()
        {
            CreateDirectory("Reports");
            CreateDirectory("allure-results");

            Console.WriteLine("[TestCollectionSetup] Environment ready.");
        }

        public void Dispose()
        {
            if (_executedFinalReport)
                return;

            _executedFinalReport = true;

            Console.WriteLine("\n[TestCollectionTeardown] Generating final reports...");

            // Flush the HTML report
            try
            {
                ReportManager.FlushReport();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Could not flush report: {ex.Message}");
            }
        }

        private static void CreateDirectory(string folder)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}

