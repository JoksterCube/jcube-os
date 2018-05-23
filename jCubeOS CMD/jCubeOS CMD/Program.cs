using jCubeOS_CMD.Real;
using System;

namespace jCubeOS_CMD
{
    class Program
    {
        private static RealMachine RealMachine { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello this is jCubeOS");
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Write task program name:");
                string filePath = Console.ReadLine();

                Input inputHandler = new ConsoleInput();
                Output outputHandler = new ConsoleOutput();

                outputHandler.WriteBlock(inputHandler.ReadBlock());

                RealMachine = new RealMachine(inputHandler, outputHandler);

                RealMachine.LoadVirtualMachine(filePath);

                bool incorrect = false;
                do
                {
                    Console.WriteLine("Choce execution mode:");
                    Console.WriteLine("1. Execute");
                    Console.WriteLine("2. Step");
                    Console.WriteLine("----------------------");
                    Console.WriteLine("0. Exit");
                    string executionMode = Console.ReadLine();
                    switch (executionMode)
                    {
                        case "1":
                            RealMachine.GetProcessor().Execute();
                            break;
                        case "2":
                            RealMachine.GetProcessor().Step();
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Incorrect execution mode.");
                            incorrect = true;
                            break;
                    }
                } while (incorrect);
            }
        }
    }
}
