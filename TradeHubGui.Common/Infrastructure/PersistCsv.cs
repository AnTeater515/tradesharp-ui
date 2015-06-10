﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeHubGui.Common.Models;

namespace TradeHubGui.Common.Infrastructure
{
    /// <summary>
    /// Saves data in a CSV file
    /// </summary>
    public static class PersistCsv
    {
        /// <summary>
        /// Saves incoming data in csv file
        /// </summary>
        /// <param name="folderPath">The folder in which to save the file</param>
        /// <param name="dataList">Data to be saved</param>
        /// <param name="instanceDescription">Brief Strategy instance Description</param>
        public static void SaveData(string folderPath, IReadOnlyList<string> dataList, string instanceDescription)
        {
            // Create file path
            string path = folderPath + "\\" + instanceDescription + "-" + DateTime.Now.ToString("yyMMddHmsfff") + ".csv";

            // Write data
            File.WriteAllLines(path, dataList);
        }

        /// <summary>
        /// Saves incoming order executions data in csv file
        /// </summary>
        /// <param name="folderPath">The folder in which to save the file</param>
        /// <param name="dataList">Data to be saved</param>
        /// <param name="instanceDescription">Brief Strategy instance Description</param>
        public static void SaveData(string folderPath, IReadOnlyList<OrderDetails> dataList, string instanceDescription)
        {
            // Create file path
            string path = folderPath + "\\" + instanceDescription + "-" + DateTime.Now.ToString("yyMMddHmsfff") + ".csv";

            if (!File.Exists(path))
            {
                StreamWriter outputFile = new StreamWriter(path);
                outputFile.WriteLine("Symbol,OrderId,Side,Quantity,Price,Time");
                foreach (var execution in dataList)
                {
                    outputFile.WriteLine(execution.BasicExecutionInfo());
                }
                outputFile.Close();
            }
        }
    }
}
