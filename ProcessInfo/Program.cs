using System;
using System.Threading;

namespace ProcessInfoProj
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ProcessInfo processInfo = new();

            while (true)
            {
                //Console.Clear();
                foreach (var process in processInfo.GetProcessDataList())
                {
                    Console.WriteLine("{0} {1} {2} {3} {4}", process.Name, process.PID, process.UserName, process.CPU, process.Memory);
                }
                break;
                //Thread.Sleep(30);
            }
        }
    }
}
