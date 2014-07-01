using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace freight_api_csharp_sample
{
    class Program
    {

        static void Main(string[] args)
        {
            Sample sample = new Sample();
            sample.RunAll();



            Console.WriteLine("Ended.... Press any key to exit...");
            Console.ReadKey();
        }


    }
}
