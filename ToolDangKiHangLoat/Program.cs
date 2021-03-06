﻿using Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ToolDangKiHangLoat
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
                using (StartRequest vnu = new StartRequest())
                {
                    vnu.User = Mssv;
                    vnu.Pass = Mssv;
                    vnu.LoginType = 1;
                    vnu.ListDataRowIndex = new List<string>()
                    {
                        "136", "141", "137"
                    };

                    vnu.Login();
                }

            });
            Console.WriteLine($"{DateTime.Now} Done");
        }
    }
}
