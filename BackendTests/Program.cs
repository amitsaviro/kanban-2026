using System;
using Backend.ServiceLayer;
using log4net;
using log4net.Config;



namespace BackendTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("../Backend/log4net.config"));
            Console.WriteLine("Starting Tests...\n");

            UserTests userTests = new UserTests();
           // BoardTests boardTests = new BoardTests();
           // TaskTests taskTests = new TaskTests();

            userTests.RunAll();

            //Console.WriteLine("\n-------------------\n");

            //boardTests.RunAll();

            //Console.WriteLine("\n-------------------\n");

            //taskTests.RunAll();

            Console.WriteLine("\nFinished Tests.");
        }
    }
}
