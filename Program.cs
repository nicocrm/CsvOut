using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;

namespace CsvOut
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.Error.WriteLine("Usage: CsvOut <server> <database> <table1> <table2> ...");
                return;
            }
            using (SqlConnection connection = new SqlConnection("server=" + args[0] + ";Initial Catalog=" + args[1] + ";Integrated Security=true"))
            {
                connection.Open();
                for (int i = 2; i < args.Length; i++)
                {
                    ExportTable(connection, args[i]);                    
                }
            }
        }

        private static void ExportTable(SqlConnection connection, string tableName)
        {
            Console.WriteLine("Writing " + tableName + ".csv");
            using (var output = new StreamWriter(tableName + ".csv"))
            {
                using (var cmd = connection.CreateCommand())
                {                    
                    cmd.CommandText = "select * from [" + tableName + "]";
                    using (var reader = cmd.ExecuteReader())
                    {
                        WriteHeader(reader, output);
                        while (reader.Read())
                        {
                            WriteData(reader, output);
                        }
                    }
                }
            }
        }

        
        private static void WriteHeader(SqlDataReader reader, TextWriter output)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0)
                    output.Write(',');
                output.Write(reader.GetName(i));
            }
            output.WriteLine();
        }

        private static void WriteData(SqlDataReader reader, TextWriter output)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0)
                    output.Write(',');
                String v = reader[i].ToString();
                if (v.Contains(',') || v.Contains('\n') || v.Contains('\r') || v.Contains('"'))
                {
                    output.Write('"');
                    output.Write(v.Replace("\"", "\"\""));
                    output.Write('"');
                }
                else
                {
                    output.Write(v);
                }
            }
            output.WriteLine();
        }
    }
}
