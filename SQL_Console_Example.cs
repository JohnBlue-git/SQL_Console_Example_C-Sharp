/*
Auther: John Blue
Time: 2022/5
Platform: VS2017
Object: to use SQL



How to create DataBase.mdf and construct it in VS GUI
https://www.youtube.com/watch?v=GVV-LUcmCOE



if Table name is "Table":
since "Table" is a really reserved keyword,
so try using SELECT * FROM dbo.[Table] ... to coding in this case



using / try catch finally:
using
1. word for namespace
2. is a block for excuting Dispose method
雖然 .NET 有內建強大的記憶體管理機制(GC)，但開發人員還是不能完全依賴 .NET 來處理一些無法釋放的資源，例如：Handles, Unmanaged Resources, …。
而使用 using 最主要的目的是為了讓物件建立的同時能確保該物件所佔用的資源一定會被完整釋放。
使用 using 陳述式有一個最基本的條件，就是該物件必須有實做 IDisposable 介面，才能確保在 using 的結尾數時自動執行 Dispose() 方法。
try catch
is a Transact that handling the exception
finally
is the block that will excute under any condition
https://blog.miniasp.com/post/2009/10/12/About-CSharp-using-Statement-misunderstanding-on-try-catch-finally
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;// for Directory.GetCurrentDirectory()
using System.Data.SqlClient;

namespace SQL_Example
{
    class SQL_Console_Example
    {
        //private static SqlConnection connectionString = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\Downloads\SQL_Console_Example\SQL_Example\Database.mdf;Integrated Security=True");
        
        // Directory.GetCurrentDirectory() is the path of the Debug folder
        // AppDomain.CurrentDomain.BaseDirectory.Length - 10 since \bin\Debug is ten
        private static SqlConnection connectionString = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Directory.GetCurrentDirectory().Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\Database.mdf;Integrated Security=True");
        
        // if this project is deployed as .exe
        //private static SqlConnection connectionString = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Directory.GetCurrentDirectory() + @"\Database.mdf;Integrated Security=True");

        private static void InsertCommand(string Id = "", string Name = "")
        {
            string queryString = "IF NOT EXISTS (SELECT Id FROM dbo.[Table] WHERE Id = " + Id + ") BEGIN INSERT INTO dbo.[Table] (Id, Name) VALUES ('" + Id + "','" + Name + "') END;";

            // method 1
            //using (SqlCommand command = new SqlCommand(queryString, connectionString))
            // method 2
            connectionString.Open();
            using (SqlCommand command = new SqlCommand())
            {
                try
                {
                    // method 1
                    //command.Connection.Open();

                    // method 2
                    command.CommandText = queryString;
                    command.Connection = connectionString;

                    // Excuting
                    command.ExecuteNonQuery();

                    // method 1
                    //command.Connection.Close();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }/*
                finally
                {
                    if (command != null)
                    {
                        ((IDisposable)command).Dispose();
                    }
                }*/
            }
            connectionString.Close();
        }

        private static void ShowCommand()
        {
            string queryString = "SELECT * FROM dbo.[Table];";

            connectionString.Open();
            using (SqlCommand command = new SqlCommand(queryString, connectionString))
            {
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read())
                        {
                            Console.WriteLine(String.Format("{0}, {1}",
                                reader[0], reader[1]));
                        }
                    }

                    command.Parameters.Clear();
                    command.CommandText = "sp_spaceused";
                    using (SqlDataReader reader = command.ExecuteReader()) {
                         while (reader.Read()) {
                            Console.WriteLine("Name: " + reader["database_name"]);
                            Console.WriteLine("Size: " + reader["database_size"]);
                            string[] s = reader["database_size"].ToString().Split('.');
                            Console.WriteLine("Size: {0}", Int32.Parse(s[0]));
                        }
                    }

                    command.Parameters.Clear();
                    command.CommandText = "SELECT COUNT(Id) FROM dbo.[Table];";
                    Console.WriteLine("Element: {0}", (Int32)command.ExecuteScalar());
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }/*
                finally
                {
                    if (command != null)
                    {
                        ((IDisposable)command).Dispose();
                    }
                }*/
            }
            connectionString.Close();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Insert ...");
            InsertCommand("2", "two");
            InsertCommand("3", "three");
            InsertCommand("4", "");
            InsertCommand("7", "any");
            InsertCommand("8", "168");
            //InsertCommand("", "666");// not allow
            Console.WriteLine();
            Console.WriteLine("Show ...");
            ShowCommand();
            Console.WriteLine();
            Console.WriteLine("Finish");
            Console.ReadKey(true);
        }
    }
}
