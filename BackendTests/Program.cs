using System;
using Backend.ServiceLayer;

namespace BackendTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Tests...\n");

            UserTests userTests = new UserTests();
            BoardTests boardTests = new BoardTests();
            TaskTests taskTests = new TaskTests();

            userTests.RunAll();

            Console.WriteLine("\n-------------------\n");

            boardTests.RunAll();

            Console.WriteLine("\n-------------------\n");

            taskTests.RunAll();

            Console.WriteLine("\nFinished Tests.");
        }
    }
}
