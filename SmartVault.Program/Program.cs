using System.Data.SQLite;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;

namespace SmartVault.Program
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            if (args.Length == 0)
            {
                return;
            }

            WriteEveryThirdFileToFile(configuration, Convert.ToInt32(args[0]));
            GetAllFileSizes();
        }

        private static void GetAllFileSizes()
        {
            // TODO: Implement functionality
        }

        private static void WriteEveryThirdFileToFile(IConfiguration configuration, int accountId)
        {
            Console.WriteLine(Environment.CurrentDirectory);
            var con = new SQLiteConnection(string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", configuration?["DatabaseFileName"]));
            con.Open();

            string stm = $"SELECT * FROM Document WHERE AccountId = {accountId}";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            using (Stream output = new FileStream("output.txt", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int count = 0;
                while (rdr.Read())
                {
                    if (++count == 3)
                    {
                        count = 0;
                        var file = rdr.GetString(3);
                        Console.WriteLine($"Appending {file}");

                        /* Assuming each file content is different */

                        // Write file name for debugging.
                        var bytes = Encoding.ASCII.GetBytes(Environment.NewLine + file + Environment.NewLine);
                        output.Write(bytes);

                        using (Stream input = File.OpenRead(rdr.GetString(4)))
                        {
                            input.CopyTo(output);
                        }
                    }
                }
            }
        }
    }
}