using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Compilo
{
    // change to different project so we have a lib and a running file 
    public class RunCompilo
    {
        static bool hadError = false;
        public static void main(String[] args) 
        {
            try
            {
                if (args.Length > 1)
                {
                    Console.WriteLine("Usage: Compilo [script]");
                    Environment.Exit(64);
                }
                else if (args.Length == 1)
                {
                    RunFile(args[0]);
                }
                else
                {
                    RunPrompt();
                }
            }
            catch (IOException e) 
            {
                throw;
            }
         
        }
        public static void RunPrompt()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Console.OpenStandardInput()))
                {
                    while (true)
                    {
                        Console.Write("> ");
                        string line = reader.ReadLine();

                        if (line == null)
                            break;

                        Run(line);
                        hadError = false;
                    }
                }
            }
            catch (IOException e) 
            {
                throw;
            }
        }

        public static void RunFile(String path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                Run(Encoding.UTF8.GetString(bytes));

                if (hadError)
                {
                    Environment.Exit(65);
                }
            }
            catch(IOException e) 
            {
                throw;
            }
        }
        private static void Run(string source)
        {
            throw new NotImplementedException();
        }

        public static void Error(int line, string message)
        {
            Report(line,"", message);
        }
        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"Line {line},  Error {where} : {message}");
            hadError = true;
        }
      
    }
}
