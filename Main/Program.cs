// Main/Program.cs
using System;
using Compilo;

namespace MainApp
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                RunCompilo.main(args);
                return 0;
            }
            catch (IOException e)
            {
                Console.Error.WriteLine($"IO Error: {e.Message}");
                return 1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Unexpected Error: {e.Message}");
                return 1;
            }
        }
    }
}
