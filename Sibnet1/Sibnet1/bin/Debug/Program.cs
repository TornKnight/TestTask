using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Data.SqlClient;
using System.IO;

namespace Sibnet1
{
    class Program
    {
        static void Main()
        {
            string csv_file_path = @"E:\Users\Torn Knight\Desktop\Sibnet\Sibnet1\Sibnet1\TestData.csv";
            DataTable csvData = ConvertCSVtoDataTable(csv_file_path);
            InsertDataIntoSQLServerUsingSQLBulkCopy(csvData);
            
            Console.ReadLine();
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }


            return dt;
        }
        static void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable csvData)
        {
            using (SqlConnection dbConnection = new SqlConnection("Data Source=.SQLEXPRESS; Initial Catalog=nameDB; Integrated Security=SSPI;"))
            {
                dbConnection.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    
                    foreach (var column in csvData.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());
                    s.DestinationTableName = csvData.TableName;
                    try
                    {
                        s.WriteToServer(csvData);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                }
            }

        }
    }
        
}
