using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IronOcr
{
    public class Config
    {
        public Config()
        {
            this.PDF_PATH = Directory.GetCurrentDirectory();
            this.DB_IP = "127.0.0.1";
            this.USER_ID = "root";
            this.DB_NAME = "ocr_db";
            this.DB_PWD = "2102559";
        }
        public string PDF_PATH { get; set; }
        public string DB_IP { get; set; }
        public string USER_ID { get; set; }
        public string DB_NAME { get; set; }
        public string DB_PWD { get; set; }
    }
}
