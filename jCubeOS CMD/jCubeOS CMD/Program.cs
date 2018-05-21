﻿using System;

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

                RealMachine = new RealMachine();

                Input inputHandler = new ConsoleInput();
                Output outputHandler = new ConsoleOutput();

                RealMachine.LoadVirtualMachine(filePath, inputHandler, outputHandler);

                Console.WriteLine(Utility.IntToBytes(16));
                Console.WriteLine(Utility.IntToBytes(32));
                Console.WriteLine(Utility.IntToBytes(512));
                Console.WriteLine(Utility.IntToBytes(64));

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