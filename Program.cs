using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GeneticAlgoTestApp {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("-- Application start --");
            Console.WriteLine();
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var worker = new GeneticHelloWorld();
            worker.Run();

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine();
            Console.WriteLine("RunTime: " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10));
            
            Console.WriteLine("-- Application end --");
            Console.WriteLine();
            Console.WriteLine("Press Enter to quit...");
            Console.ReadLine();
        }
    }
}
