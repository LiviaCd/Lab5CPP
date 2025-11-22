using System.Text;
using OpenQA.Selenium;

namespace Lab7.Base
{
    /// <summary>
    /// Manages HTML test report generation
    /// </summary>
    public class ReportManager
    {
        private static TestReport? _currentReport;
        private static TestCase? _currentTestCase;
        private static string? _reportHtmlPath;
        private static readonly string ReportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");

        static ReportManager()
        {
            if (!Directory.Exists(ReportDirectory))
            {
                Directory.CreateDirectory(ReportDirectory);
            }
        }

        public static void InitializeReport()
        {
            _reportHtmlPath = Path.Combine(ReportDirectory, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            _currentReport = new TestReport
            {
                Title = "Google Search Test Report",
                ReportName = "Lab7 - Google Search Automation Report",
                Browser = "Chrome",
                Environment = "Test",
                OS = Environment.OSVersion.ToString(),
                StartTime = DateTime.Now,
                TestCases = new List<TestCase>()
            };
        }

        public static void CreateTest(string testName, string description = "")
        {
            _currentTestCase = new TestCase
            {
                Name = testName,
                Description = description,
                Status = "Running",
                Logs = new List<LogEntry>(),
                Screenshots = new List<string>(),
                StartTime = DateTime.Now
            };
            _currentReport?.TestCases.Add(_currentTestCase);
        }

        public static void LogInfo(string message)
        {
            _currentTestCase?.Logs.Add(new LogEntry
            {
                Level = "Info",
                Message = message,
                Timestamp = DateTime.Now
            });
        }

        public static void LogPass(string message)
        {
            _currentTestCase?.Logs.Add(new LogEntry
            {
                Level = "Pass",
                Message = message,
                Timestamp = DateTime.Now
            });
        }

        public static void LogFail(string message)
        {
            _currentTestCase?.Logs.Add(new LogEntry
            {
                Level = "Fail",
                Message = message,
                Timestamp = DateTime.Now
            });
            if (_currentTestCase != null)
            {
                _currentTestCase.Status = "Failed";
            }
        }

        public static void LogFail(string message, Exception exception)
        {
            LogFail($"{message}: {exception.Message}");
            _currentTestCase?.Logs.Add(new LogEntry
            {
                Level = "Error",
                Message = exception.ToString(),
                Timestamp = DateTime.Now
            });
        }

        public static void CompleteTestCase(string status)
        {
            // Set end time and status for current test case
            if (_currentTestCase != null)
            {
                _currentTestCase.EndTime = DateTime.Now;
                
                if (status.Equals("Pass", StringComparison.OrdinalIgnoreCase))
                {
                    _currentTestCase.Status = "Passed";
                }
                else
                {
                    _currentTestCase.Status = "Failed";
                }
            }
        }


        public static void FlushReport()
        {
            if (_currentReport == null || string.IsNullOrEmpty(_reportHtmlPath))
                return;

            _currentReport.EndTime = DateTime.Now;
            _currentReport.Duration = _currentReport.EndTime - _currentReport.StartTime;

            var html = GenerateHtmlReport(_currentReport);
            File.WriteAllText(_reportHtmlPath, html, Encoding.UTF8);
            
            Console.WriteLine($"\nTest execution completed. Report saved in: {_reportHtmlPath}");
        }

        public static string GetReportPath()
        {
            return ReportDirectory;
        }

        private static string GenerateHtmlReport(TestReport report)
        {
            var html = new StringBuilder();
            var totalTests = report.TestCases.Count;
            var passedTests = report.TestCases.Count(t => t.Status == "Passed");
            var failedTests = report.TestCases.Count(t => t.Status == "Failed");
            var passRate = totalTests > 0 ? (passedTests * 100.0 / totalTests) : 0;

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='en'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine($"    <title>{report.Title}</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        * { margin: 0; padding: 0; box-sizing: border-box; }");
            html.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f5f5f5; padding: 20px; }");
            html.AppendLine("        .container { max-width: 1200px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }");
            html.AppendLine("        h1 { color: #333; border-bottom: 3px solid #4CAF50; padding-bottom: 10px; margin-bottom: 20px; }");
            html.AppendLine("        .summary { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin: 20px 0; }");
            html.AppendLine("        .summary-card { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 8px; text-align: center; }");
            html.AppendLine("        .summary-card.passed { background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%); }");
            html.AppendLine("        .summary-card.failed { background: linear-gradient(135deg, #f44336 0%, #da190b 100%); }");
            html.AppendLine("        .summary-card h2 { font-size: 2.5em; margin: 10px 0; }");
            html.AppendLine("        .summary-card p { font-size: 0.9em; opacity: 0.9; }");
            html.AppendLine("        .test-case { border: 1px solid #ddd; margin: 20px 0; border-radius: 8px; overflow: hidden; }");
            html.AppendLine("        .test-case-header { background: #f8f9fa; padding: 15px; border-bottom: 1px solid #ddd; display: flex; justify-content: space-between; align-items: center; }");
            html.AppendLine("        .test-case-header.passed { background: #d4edda; border-left: 4px solid #28a745; }");
            html.AppendLine("        .test-case-header.failed { background: #f8d7da; border-left: 4px solid #dc3545; }");
            html.AppendLine("        .test-name { font-weight: bold; font-size: 1.1em; color: #333; }");
            html.AppendLine("        .test-status { padding: 5px 15px; border-radius: 20px; font-weight: bold; color: white; }");
            html.AppendLine("        .test-status.passed { background: #28a745; }");
            html.AppendLine("        .test-status.failed { background: #dc3545; }");
            html.AppendLine("        .test-content { padding: 15px; }");
            html.AppendLine("        .log-entry { margin: 10px 0; padding: 10px; border-radius: 4px; border-left: 3px solid #ccc; }");
            html.AppendLine("        .log-entry.info { background: #e7f3ff; border-left-color: #2196F3; }");
            html.AppendLine("        .log-entry.pass { background: #d4edda; border-left-color: #28a745; }");
            html.AppendLine("        .log-entry.fail { background: #f8d7da; border-left-color: #dc3545; }");
            html.AppendLine("        .log-entry.error { background: #fff3cd; border-left-color: #ffc107; }");
            html.AppendLine("        .screenshot { margin: 10px 0; }");
            html.AppendLine("        .screenshot img { max-width: 600px; border: 1px solid #ddd; border-radius: 4px; cursor: pointer; }");
            html.AppendLine("        .screenshot img:hover { box-shadow: 0 4px 8px rgba(0,0,0,0.2); }");
            html.AppendLine("        .info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 15px; margin: 20px 0; padding: 15px; background: #f8f9fa; border-radius: 4px; }");
            html.AppendLine("        .info-item { display: flex; justify-content: space-between; }");
            html.AppendLine("        .info-label { font-weight: bold; color: #666; }");
            html.AppendLine("        .info-value { color: #333; }");
            html.AppendLine("        .timestamp { color: #666; font-size: 0.9em; margin-top: 5px; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine($"    <div class='container'>");
            html.AppendLine($"        <h1>{report.Title}</h1>");
            html.AppendLine("        <div class='info-grid'>");
            html.AppendLine($"            <div class='info-item'><span class='info-label'>Report Name:</span><span class='info-value'>{report.ReportName}</span></div>");
            html.AppendLine($"            <div class='info-item'><span class='info-label'>Browser:</span><span class='info-value'>{report.Browser}</span></div>");
            html.AppendLine($"            <div class='info-item'><span class='info-label'>Environment:</span><span class='info-value'>{report.Environment}</span></div>");
            html.AppendLine($"            <div class='info-item'><span class='info-label'>OS:</span><span class='info-value'>{report.OS}</span></div>");
            html.AppendLine($"            <div class='info-item'><span class='info-label'>Start Time:</span><span class='info-value'>{report.StartTime:yyyy-MM-dd HH:mm:ss}</span></div>");
            html.AppendLine($"            <div class='info-item'><span class='info-label'>End Time:</span><span class='info-value'>{report.EndTime:yyyy-MM-dd HH:mm:ss}</span></div>");
            html.AppendLine($"            <div class='info-item'><span class='info-label'>Duration:</span><span class='info-value'>{report.Duration.TotalSeconds:F2} seconds</span></div>");
            html.AppendLine("        </div>");
            html.AppendLine("        <div class='summary'>");
            html.AppendLine($"            <div class='summary-card'><h2>{totalTests}</h2><p>Total Tests</p></div>");
            html.AppendLine($"            <div class='summary-card passed'><h2>{passedTests}</h2><p>Passed</p></div>");
            html.AppendLine($"            <div class='summary-card failed'><h2>{failedTests}</h2><p>Failed</p></div>");
            html.AppendLine($"            <div class='summary-card'><h2>{passRate:F1}%</h2><p>Pass Rate</p></div>");
            html.AppendLine("        </div>");

            foreach (var testCase in report.TestCases)
            {
                var statusClass = testCase.Status == "Passed" ? "passed" : "failed";
                html.AppendLine("        <div class='test-case'>");
                html.AppendLine($"            <div class='test-case-header {statusClass}'>");
                html.AppendLine($"                <div class='test-name'>{testCase.Name}</div>");
                html.AppendLine($"                <div class='test-status {statusClass}'>{testCase.Status}</div>");
                html.AppendLine("            </div>");
                html.AppendLine("            <div class='test-content'>");
                
                if (!string.IsNullOrEmpty(testCase.Description))
                {
                    html.AppendLine($"                <p><strong>Description:</strong> {testCase.Description}</p>");
                }

                html.AppendLine($"                <p><strong>Duration:</strong> {(testCase.EndTime - testCase.StartTime).TotalSeconds:F2} seconds</p>");

                if (testCase.Logs.Any())
                {
                    html.AppendLine("                <h3>Logs:</h3>");
                    foreach (var log in testCase.Logs)
                    {
                        html.AppendLine($"                <div class='log-entry {log.Level.ToLower()}'>");
                        html.AppendLine($"                    <strong>{log.Level}:</strong> {EscapeHtml(log.Message)}");
                        html.AppendLine($"                    <div class='timestamp'>{log.Timestamp:yyyy-MM-dd HH:mm:ss}</div>");
                        html.AppendLine("                </div>");
                    }
                }

                if (testCase.Screenshots.Any())
                {
                    html.AppendLine("                <h3>Screenshots:</h3>");
                    foreach (var screenshot in testCase.Screenshots)
                    {
                        html.AppendLine("                <div class='screenshot'>");
                        html.AppendLine($"                    <a href='{screenshot}' target='_blank'><img src='{screenshot}' alt='Screenshot' /></a>");
                        html.AppendLine("                </div>");
                    }
                }

                html.AppendLine("            </div>");
                html.AppendLine("        </div>");
            }

            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private static string EscapeHtml(string text)
        {
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }
    }

    internal class TestReport
    {
        public string Title { get; set; } = string.Empty;
        public string ReportName { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string OS { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public List<TestCase> TestCases { get; set; } = new();
    }

    internal class TestCase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Running";
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now;
        public List<LogEntry> Logs { get; set; } = new();
        public List<string> Screenshots { get; set; } = new();
    }

    internal class LogEntry
    {
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}

