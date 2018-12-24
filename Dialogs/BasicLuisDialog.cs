using System;
using System.Configuration;
using System.Threading.Tasks;

using System.Collections.Generic;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

using MySql.Data.MySqlClient;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("1");
            context.Wait(MessageReceived);
        }

        [LuisIntent("具体产品相关问题")]
        public async Task ProducDetailtIntent(IDialogContext context, LuisResult result)
        {
            string name = result.Entities[1].Type.Split(':')[2];
            string prop = result.Entities[0].Type.Split(':')[2];
            
            string res = "";
            string comp = "齐尾";
            
            if (string.Equals(name, comp)) {
                res = "123";
            }
            
            //  DB_Conn conn = new DB_Conn();
            //  string query_result = conn.ProductDetail(name, prop);
            
            //  string result_str = query_result;
            //  await context.PostAsync(result_str);
            await context.PostAsync(name + prop + res);
            context.Wait(MessageReceived);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Greeting" with the name of your newly created intent in the following handler
        [LuisIntent("产品类别")]
        public async Task ProductTypeIntent(IDialogContext context, LuisResult result)
        {
            
            DB_Conn conn = new DB_Conn();
            List<string> query_lst_result = conn.First_Level_Categories();
            
            string result_str = string.Empty;
            for (int i = 0; i < query_lst_result.Count; ++i) {
                result_str += "/" + query_lst_result[i];
            }
            
            await context.PostAsync(result_str);
            //  await context.PostAsync("产品类别");
            context.Wait(MessageReceived);           
        }

        [LuisIntent("水果种类")]                
        [LuisIntent("蔬菜种类")]
        public async Task VegetableIntent(IDialogContext context, LuisResult result)
        {
            //  string category = result.Entities[0].Type.Split(':')[1];
            
            //  DB_Conn conn = new DB_Conn();
            //  List<string> query_lst_result = conn.Second_Level_Categories(category);
            
            //  string result_str = string.Empty;
            //  for (int i = 0; i < query_lst_result.Count; ++i) {
            //      result_str += "/" + query_lst_result[i];
            //  }
            
            //  await context.PostAsync(result_str);
            await context.PostAsync("zhonglei");
            context.Wait(MessageReceived);
        }
        
        [LuisIntent("番茄问题")]
        [LuisIntent("白菜问题")]
        [LuisIntent("苹果问题")]
        [LuisIntent("香蕉问题")]
        public async Task BananaIntent(IDialogContext context, LuisResult result)
        {
            //  string type = result.Entities[1].Type;
            //  string prop = result.Entities[0].Type.Split(':')[1];
            
            //  DB_Conn conn = new DB_Conn();
            //  List<string> query_lst_result = conn.Third_Level_Categories(type, prop);
            
            //  string result_str = string.Empty;
            //  for (int i = 0; i < query_lst_result.Count; ++i) {
            //      result_str += "/" + query_lst_result[i];
            //  }
            
            //  await context.PostAsync(result_str);
             await context.PostAsync("wenti");
            context.Wait(MessageReceived);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            //string query_result = string.Empty;
            //DB_Conn conn = new DB_Conn();
            //List<string> query_lst_result = conn.Categories("");
            //query_result = conn.ProductDetail("apple", "quantity");
            //await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            await context.PostAsync($"{result.Entities[0].Type},{result.Entities[1].Type}");
            //for (int i = 0; i < query_lst_result.Count; ++i) {
                //query_result += "/" + query_lst_result[i];
            //}
            //await context.PostAsync(query_result);
            context.Wait(MessageReceived);
        }
    }
    
    public class DB_Conn
    {
       string conn_str = String.Empty;

        public DB_Conn()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "industryqnamysql.mysql.database.azure.com",
                Database = "fruit",
                UserID = "user@industryqnamysql",
                Password = "Admin!!!"
            };

            this.conn_str = builder.ConnectionString;
        }

        public string ProductDetail(string name, string prop)
        {
            string result = String.Empty;

            try {
                var conn = new MySqlConnection(conn_str);
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT" + prop + "FROM detail WHERE name = @name;";
                command.Parameters.AddWithValue("@name", name);

                var reader = command.ExecuteReader();
                while (reader.Read()) {
                    result = reader.GetString(0);
                }
            } catch (Exception e) {
                 result = e.Message;   
            }

            return result;
        }
        
        public List<string> First_Level_Categories()
        {
            List<string> result = new List<String>();
            
            try {
                var conn = new MySqlConnection(conn_str);
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT category FROM fruit.category;";

                var reader = command.ExecuteReader();
                while (reader.Read()) {
                    result.Add(reader.GetString(0));
                }
            } catch (Exception e) {
                 result.Add(e.Message);   
            }
            
            return result;
        }
        
        public List<string> Second_Level_Categories(string category)
        {
            List<string> result = new List<String>();
            
            using (var conn = new MySqlConnection(conn_str))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {                   
                    command.CommandText = "SELECT type FROM " + category + ";";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0));
                        }
                    }
                }
                
                return result;
            }
        }
        
        public List<string> Third_Level_Categories(string type, string prop)
        {
            List<string> result = new List<String>();
            
            using (var conn = new MySqlConnection(conn_str))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {                    
                    command.CommandText = "SELECT name FROM detail WHERE type = @type;";
                    command.Parameters.AddWithValue("@type", type);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0));
                        }
                    }
                }
                
                return result;
            }
        }
    }
}