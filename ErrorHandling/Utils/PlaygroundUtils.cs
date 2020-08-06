using System;

namespace ErrorHandling.Utils
{
    public static class PlaygroundUtils
    {
        public static void PrintExceptionInfo(Exception exception)
        {
            Console.WriteLine($"Exception Type: {exception.GetType().Name}");
            Console.WriteLine($"Message: {exception.Message}");
            Console.WriteLine("-------------------------------");
            //Console.WriteLine(exception.StackTrace);
        }
        
        public static void Start(string testName)
            => Console.WriteLine($"\n\nRunning: {testName}\n-----------------------");
    }
}