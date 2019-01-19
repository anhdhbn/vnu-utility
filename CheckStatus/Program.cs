using Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CheckStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> listAcc = File.ReadAllLines("00.txt").ToList();
            Parallel.ForEach(listAcc, new ParallelOptions()
            {
                MaxDegreeOfParallelism = 1
            }, (Mssv) =>
            {
                using (CheckStatusRequest vnu = new CheckStatusRequest())
                {
                    vnu.User = Mssv;
                    vnu.Pass = Mssv;
                    vnu.Begin();
                }

            });
            Console.ReadKey();
        }
    }
}
