using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartVault.Library;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace SmartVault.DataGeneration
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            SQLiteConnection.CreateFile(configuration["DatabaseFileName"]);
            File.WriteAllText("TestDoc.txt", $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}");

            /*
             * Note by Tushar on 2023-05-08.
             * It is possible that Sqlite is not a truely concurrent DB
             * and therefore writing to a table concurrently may not achive
             * the desired outcome and therefore this has been avoided.
             * Refer to https://www.sqlite.org/lockingv3.html for more info.
             */

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.WriteLine("DataGeneration started");

            using (var connection = new SQLiteConnection(string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", configuration?["DatabaseFileName"])))
            {
                var files = Directory.GetFiles(@"..\..\..\..\BusinessObjectSchema");

                for (int i = 0; i <= 2; i++)
                {
                    var serializer = new XmlSerializer(typeof(BusinessObject));
                    var businessObject = serializer.Deserialize(new StreamReader(files[i])) as BusinessObject;
                    connection.Execute(businessObject?.Script);

                }

                Console.WriteLine("Scripts executed");

                var documentPathInfo = new FileInfo("TestDoc.txt");

                connection.Open();

                /*
                 * Note by Tushar on 2023-05-08.
                 * Create prepared statement so that SQL does not need to be parsed by the runtime on each iteration.
                 */

                using var detailCmd = new SQLiteCommand(connection);
                detailCmd.CommandText = "INSERT INTO Document (Id, Name, FilePath, Length, AccountId) VALUES(@Id, @Name, @FilePath, @Length, @AccountId)";
                var detailId = detailCmd.Parameters.Add("@Id", System.Data.DbType.Int32);
                var detailName = detailCmd.Parameters.Add("@Name", System.Data.DbType.String);
                var detailFilePath = detailCmd.Parameters.AddWithValue("@FilePath", documentPathInfo.FullName);
                var detailLength = detailCmd.Parameters.AddWithValue("@Length", documentPathInfo.Length);
                var detailAccountUId = detailCmd.Parameters.Add("@AccountId", System.Data.DbType.Int32);
                detailCmd.Prepare();

                var documentNumber = 0;
                for (int i = 0; i < 100; i++)
                {
                    var randomDayIterator = RandomDay().GetEnumerator();
                    randomDayIterator.MoveNext();
                    connection.Execute($"INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password) VALUES('{i}','FName{i}','LName{i}','{randomDayIterator.Current.ToString("yyyy-MM-dd")}','{i}','UserName-{i}','e10adc3949ba59abbe56e057f20f883e')");
                    connection.Execute($"INSERT INTO Account (Id, Name) VALUES('{i}','Account{i}')");

                    /*
                     * Note by Tushar on 2023-05-08.
                     * Commit entire batch of inserts in one go. This is the most efficient way to insert
                     * many rows however it does also require, on most db servers, a reasonably sized log-file.
                     */

                    using (var tx = connection.BeginTransaction())
                    {
                        detailCmd.Transaction = tx;

                        for (int d = 0; d < 10000; d++, documentNumber++)
                        {
                            if (d % 1000 == 0)
                                Console.WriteLine($"  Details inserted {d * 100 / 10000}%");

                            detailId.Value = documentNumber;
                            detailName.Value = $"Document{i}-{d}.txt";
                            detailAccountUId.Value = i;
                            detailCmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }

                    Console.WriteLine($"Inserted { i + 1 }%");
                }

                connection.Close();

                var accountData = connection.Query("SELECT COUNT(*) FROM Account;");
                Console.WriteLine($"AccountCount: {JsonConvert.SerializeObject(accountData)}");
                var documentData = connection.Query("SELECT COUNT(*) FROM Document;");
                Console.WriteLine($"DocumentCount: {JsonConvert.SerializeObject(documentData)}");
                var userData = connection.Query("SELECT COUNT(*) FROM User;");
                Console.WriteLine($"UserCount: {JsonConvert.SerializeObject(userData)}");
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            var elapsedTime = String.Format(
                "{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours,
                ts.Minutes,
                ts.Seconds,
                ts.Milliseconds / 10
            );

            Console.WriteLine("DataGeneration finished");
            Console.WriteLine("RunTime " + elapsedTime);
        }

        static IEnumerable<DateTime> RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }
    }
}