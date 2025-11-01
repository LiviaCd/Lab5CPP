namespace Lab5CPP
{
    public static class Logger
    {
        public static void LogStep(string message, string type)
        {
            var originalColor = Console.ForegroundColor;

            switch (type)
            {
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.Highlight:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            string timestampedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
            Console.WriteLine(timestampedMessage);

            Console.ForegroundColor = originalColor;
        }
    }
}
