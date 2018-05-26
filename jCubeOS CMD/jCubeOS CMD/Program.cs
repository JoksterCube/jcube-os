using jCubeOS_CMD.Real;
using System;
using System.Collections.Generic;
using System.IO;

namespace jCubeOS_CMD
{
    class Program
    {
        private static RealMachine RealMachine { get; set; }
        private static List<string> ProgramList { get; set; }

        static void Main(string[] args)
        {
            PrintLogo();
            Console.WriteLine("\n----------- Welcome to jCubeOS! -----------\n");
            GetFileNames();
            bool exit = false;
            while (!exit)
            {
                Console.Write("\nTask program name: ");
                string filePath = FromFileList(Console.ReadLine());

                Input inputHandler = new ConsoleInput();
                Output outputHandler = new ConsoleOutput();

                RealMachine = new RealMachine(inputHandler, outputHandler);

                //FAKE MEMORY ALLOCATION
                FakeMemory(0, 1, 3, 5, 7, 9, 10, 14, 16, 17, 21, 23, 24, 27, 29, 31, 33);

                if (!RealMachine.LoadVirtualMachine(filePath)) continue;

                exit = Execution(exit);
            }
        }

        private static bool Execution(bool exit)
        {
            bool incorrect = false;
            do
            {
                PrintMenu();
                Console.Write("EXECUTION MODE: ");
                string executionMode = Console.ReadLine();
                Console.Write("\n");
                bool working = true;
                switch (executionMode)
                {
                    case "1":
                        working = RealMachine.GetProcessor().Execute();
                        break;
                    case "2":
                        working = RealMachine.GetProcessor().Step();
                        bool done = false;
                        while (!done)
                        {
                            PrintStepMenu();
                            Console.Write("ACTION: ");
                            string action = Console.ReadLine();
                            Console.Write("\n");
                            switch (action)
                            {
                                case "1":
                                    working = RealMachine.GetProcessor().Step();
                                    break;
                                case "2":
                                    working = RealMachine.GetProcessor().Execute();
                                    break;
                                case "3":
                                    RealMachine.GetRealMemory().PrintUserMemory();
                                    break;
                                case "4":
                                    RealMachine.GetVirtualMemory().PrintVirtualMemory();
                                    break;
                                case "5":
                                    RealMachine.GetProcessor().PrintAllRegisters();
                                    break;
                                case "0":
                                    done = true;
                                    break;
                                case "c":
                                    Console.Clear();
                                    break;
                                default:
                                    Console.WriteLine("Incorrect action.");
                                    break;
                            }
                            if (!working) done = true;
                        }
                        break;
                    case "0":
                        exit = true;
                        break;
                    case "c":
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Incorrect execution mode.");
                        incorrect = true;
                        break;
                }
                if (!working) break;
            } while (incorrect);
            return exit;
        }

        private static void GetFileNames()
        {
            ProgramList = new List<string>();
            string[] files = Directory.GetFiles(@".", "*.jcos");
            if (files.Length > 0) Console.WriteLine("jCubeOS program files found in directory:");
            for (int i = 0; i < files.Length; i++)
            {
                ProgramList.Add(files[i]);
                Console.WriteLine(i + ". " + ProgramList[i]);
            }
        }

        private static string FromFileList(string fileName)
        {
            int index = -1;
            if (int.TryParse(fileName, out index) && index >= 0 && index < ProgramList.Count) fileName = ProgramList[index];

            return fileName;
        }

        private static void PrintStepMenu()
        {
            Console.Write("\n");
            Console.WriteLine("Choose action:");
            Console.WriteLine("1. Step again");
            Console.WriteLine("2. Execute to the end");
            Console.WriteLine("3. Print real user memory");
            Console.WriteLine("4. Print virtual memory");
            Console.WriteLine("5. Print Registers");
            Console.WriteLine("----------------------");
            Console.WriteLine("0. Exit");
            Console.WriteLine("c. Clear console");
            Console.Write("\n");
        }

        private static void PrintMenu()
        {
            Console.Write("\n");
            Console.WriteLine("Choose execution mode:");
            Console.WriteLine("1. Execute");
            Console.WriteLine("2. Step");
            Console.WriteLine("----------------------");
            Console.WriteLine("0. Exit");
            Console.WriteLine("c. Clear console");
            Console.Write("\n");
        }

        private static void FakeMemory(params int[] blockIds)
        {
            for (int i = 0; i < blockIds.Length; i++) RealMachine.GetRealMemory().TakeMemoryBlock(blockIds[i]);
        }

        private static void PrintLogo()
        {
            Console.WriteLine("###########################################################################################");
            Console.WriteLine("##                                                                                       ##");
            Console.WriteLine("##                #######              ##                      #######        ######     ##");
            Console.WriteLine("##              ##       ##            ##                    ##       ##    ##      ##   ##");
            Console.WriteLine("##       ##    ##         #            ##                   ##         ##  ##            ##");
            Console.WriteLine("##             ##            ##    ##  ## ####     #####    ##         ##    ####        ##");
            Console.WriteLine("##       ##    ##            ##    ##  ###    ##  ##   ##   ##         ##        ####    ##");
            Console.WriteLine("##       ##    ##         #  ##    ##  ##     ##  #######   ##         ##            ##  ##");
            Console.WriteLine("##       ##     ##       ##  ##    ##  ###    ##  ##         ##       ##    ##      ##   ##");
            Console.WriteLine("##       ##       #######     ##### #  ## ####     #####       #######        ######     ##");
            Console.WriteLine("##      ##                                                                               ##");
            Console.WriteLine("##  #####                                                                                ##");
            Console.WriteLine("###########################################################################################");
        }
    }
}