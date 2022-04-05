using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronOcr;
using System.IO;

namespace IronOcr
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Util.configDB();
                string[] pdfL = Directory.GetFiles(Util.CONFIG.PDF_PATH, "*.pdf");
                Console.WriteLine($"Total Count of Files: {pdfL.Length}\n");
                if (pdfL.Length == 0)
                {
                    Console.WriteLine("Please set source folder in config.json file!");
                    return;
                }
                Console.WriteLine("Start Converting...\n");
                foreach (string file in pdfL)
                {
                    string result = new IronOcr.IronTesseract().Read(file).Text;
                    string name = result.Substring(result.IndexOf("Name:") + 6, result.IndexOf("Sex:") - result.IndexOf("Name") - 6);
                    string[] nameL = name.Split(',');
                    string firstName = nameL[0].Trim();
                    string middleName = string.Empty;
                    string lastName = string.Empty;
                    if (nameL.Length == 3)
                    {
                        middleName = nameL[1].Trim();
                        lastName = nameL[2].Trim();
                    }
                    else
                    {
                        lastName = nameL[1].Trim();
                    }

                    string sex = result.Substring(result.IndexOf("Sex:") + 5, 1);
                    string orderingMD = result.Substring(result.IndexOf("Ordering MD:") + 13, result.IndexOf("Acct#:") - result.IndexOf("Ordering MD:") - 13).Trim();
                    string acctNumber = result.Substring(result.IndexOf("Acct#:") + 7, result.IndexOf("MR #:") - result.IndexOf("Acct#:") - 7);
                    string mr = result.Substring(result.IndexOf("MR #:") + 5, result.IndexOf("DOB:") - result.IndexOf("MR #:") - 5).Trim();
                    string dob = result.Substring(result.IndexOf("DOB:") + 5, result.IndexOf("Loc:") - result.IndexOf("DOB:") - 5).Trim();
                    string loc = result.Substring(result.IndexOf("Loc:") + 5, result.IndexOf("Tel:") - result.IndexOf("Loc:") - 5).Trim();
                    string tel = result.Substring(result.IndexOf("Tel:") + 5, result.IndexOf("Coll:") - result.IndexOf("Tel:") - 5).Trim();
                    string coll = result.Substring(result.IndexOf("Coll:") + 6, result.IndexOf("Spec #:") - result.IndexOf("Coll:") - 6).Trim();
                    string collDate = coll.Split('-')[0];
                    string collTime = coll.Split('-')[1];
                    collTime = collTime.Substring(0, 2) + ":" + collTime.Substring(2, 2);
                    string receive = result.Substring(result.IndexOf("Recd:") + 6, result.IndexOf("Spec Status:") - result.IndexOf("Recd:") - 6).Trim();
                    string recDate = receive.Split('-')[0];
                    string recTime = receive.Split('-')[1];
                    recTime = recTime.Substring(0, 2) + ":" + recTime.Substring(2, 2);

                    List<int> specNumIL = Util.AllIndexesOf(result, "SPEC #:");
                    List<string> specNumL = new List<string>(Enumerable.Repeat("", 4).ToArray());
                    for (int i = 0; i < specNumIL.Count; i++)
                        specNumL[i] = Util.getSpecNum(result, specNumIL[i]);

                    List<int> specStatusIL = Util.AllIndexesOf(result, "Status:");
                    List<string> specStatusL = new List<string>(Enumerable.Repeat("", 4).ToArray());
                    for (int i = 0; i < specNumIL.Count; i++)
                        specStatusL[i] = Util.getSpecStatus(result, specStatusIL[i]);

                    List<int> orderedIL = Util.AllIndexesOf(result, "Ordered:");
                    List<string> orderedL = new List<string>(Enumerable.Repeat("", 4).ToArray());
                    for (int i = 0; i < orderedIL.Count; i++)
                        orderedL[i] = Util.getOrdered(result, orderedIL[i]);


                    int id = DBQuery.getCurrentID() + 1;
                    string query = $"INSERT INTO patient values({id}, '{firstName}', '{middleName}', '{lastName}', '{sex}', "
                                   + $"'{orderingMD}', '{acctNumber}', '{mr}', '{dob}', '{loc}', '{tel}', '{collDate}', '{collTime}', "
                                   + $"'{recDate}', '{recTime}', '{specNumL[0]}', '{specStatusL[0]}', '{orderedL[0]}', "
                                   + $"'{specNumL[1]}', '{specStatusL[1]}', '{orderedL[1]}', "
                                   + $"'{specNumL[2]}', '{specStatusL[2]}', '{orderedL[2]}', "
                                   + $"'{specNumL[3]}', '{specStatusL[3]}', '{orderedL[3]}');";

                    DBQuery.Execute_Query(query);
                }
                Console.WriteLine("Complete!\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }

}
