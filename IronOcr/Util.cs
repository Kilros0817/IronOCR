using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace IronOcr
{
    public static class Util
    {
        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.CurrentCultureIgnoreCase);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static string getSpecNum(string str, int index)
        {
            int endPos = str.IndexOf(' ', str.IndexOf(':', index + 7));
            int endPos1 = str.IndexOf('\r', str.IndexOf(':', index + 7));
            endPos = endPos > endPos1 ? endPos1 : endPos;
            string specNum = str.Substring(index + 7, endPos - index - 7).Trim();
            if (specNum.IndexOf(' ') != -1)
                specNum = specNum.Substring(0, specNum.IndexOf(' ')) + specNum.Substring(specNum.IndexOf(' ') + 1);
            return specNum;
        }

        public static string getSpecStatus(string str, int index)
        {
            int endPos = str.IndexOf('\r', index);
            string specStatus = str.Substring(index + 8, endPos - index - 8).Trim();
            return specStatus;
        }

        public static string getOrdered(string str, int index)
        {
            int endPos = str.IndexOf('\r', index);
            string ordered = str.Substring(index + 8, endPos - index - 8).Trim();
            return ordered;
        }

        //db
        public static string PATH_CONFIG = string.Empty;
        public static Config CONFIG = new Config();

        public static MySqlConnection con;
        public static MySqlCommand cmd;
        public static MySqlDataAdapter da;
        public static DataTable dt;
        public static void configDB()
        {
            PATH_CONFIG = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

            if (!File.Exists(PATH_CONFIG))
                createConfig();
            
            string envJson = string.Empty;

            envJson = File.ReadAllText(PATH_CONFIG);
            CONFIG = JsonConvert.DeserializeObject<Config>(envJson);

            con = new MySqlConnection($"server = {CONFIG.DB_IP}; user id = {CONFIG.USER_ID}; pwd = {CONFIG.DB_PWD}; database={CONFIG.DB_NAME}");
            DBQuery.createTable();
        }
        public static void createConfig()
        {
            string envJson = JsonConvert.SerializeObject(CONFIG, Formatting.Indented);
            File.WriteAllText(PATH_CONFIG, envJson);
        }
    }
}
