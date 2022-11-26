using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ConsoleApp1
{
    public class DbHelper
    {
        static SqlConnection connection = new SqlConnection(@"Data Source=.;Initial Catalog=english;Integrated Security=True");
        public static void Open()
        {
            if (connection?.State == ConnectionState.Open)
            {
                return;
            }

            connection.Open();
        }
        public static List<ExampleEntity> ReadWord(string word)
        {
           

            using SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = $@"SELECT    a.[ExampleType] 
      ,[Example]  ,a.[Translate]  ,[Audio]  FROM[wordexample] a inner join[englishdictionary] b on b.id = a.WordId
  where
   b.[Word] = '{word}'";
            sqlCommand.Connection = connection;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);
       
            List<ExampleEntity> examples = new List<ExampleEntity>();
            var expamplq1s = dataTable.Select("ExampleType=1 ","    Audio  desc");
            if (expamplq1s.Length>0)
            {
                examples.Add(new ExampleEntity(expamplq1s[0]));
            }
            expamplq1s = dataTable.Select("ExampleType=2 ", "   Audio  desc");
            if (expamplq1s.Length > 0)
            {
                examples.Add(new ExampleEntity(expamplq1s[0]));
            }
            expamplq1s = dataTable.Select("ExampleType=3 ", "   Audio  desc");
            if (expamplq1s.Length > 0)
            {
                examples.Add(new ExampleEntity(expamplq1s[0]));
            }
            
            return examples;
        }


        public class ExampleEntity
        {
            public ExampleEntity(string type,string text,string trans,string audio) {
                ExampleType = type;
                Example = text;
                Translate = trans;
                Audio = audio;
            
            
            }
            public ExampleEntity(DataRow dataRow)
            {
                var type=int. Parse( dataRow["ExampleType"].ToString());
                if (type==1)
                {
                    ExampleType = "双语例句";
                }else if (type == 2)
                {
                    ExampleType = "原声例句";
                }
                else  
                {
                    ExampleType = "权威例句";
                }
                Example = dataRow["Example"] as string;
                Translate = dataRow["Translate"] as string;
                Audio = dataRow["Audio"] as string;
            }

            public string ExampleType { get; set; }

            public string Example  { get; set; }

            public string Translate { get; set; }

            public string Audio { get; set; }
        }
    }
}
