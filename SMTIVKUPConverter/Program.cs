using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Threading.Tasks;


namespace KUPConverter
{
    
    class Program
    {
        static void Main(string[] args)
        {
            System.Text.Encoding UTF8 = System.Text.Encoding.UTF8;
            System.Text.Encoding SJIS = System.Text.Encoding.GetEncoding(932);
            System.Text.Encoding Default = System.Text.Encoding.Default;
            List<string> originalList = new List<string>();
            List<string> editedList = new List<string>();
            List<string> editedSJISList = new List<string>();

            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("*  KUPConverter                                                     @TraduSquare  *");
            Console.WriteLine("***********************************************************************************");
            Console.Write("Choose directory: ");
            string dir = Console.ReadLine();
            if (dir != "")
            {
                Console.WriteLine("Checking integrity...");
                    try
                    {
                    foreach (var filefound in Directory.GetFiles(dir, "*.kup", SearchOption.AllDirectories))
                    {
                        XDocument file = XDocument.Load(filefound);
                        string fileName = Path.GetFileName(filefound);
                        Console.WriteLine("Integrity OK");
                        Console.WriteLine("Deserializing content...");
                        XElement fileRoot = file.Root;
                        //LinQ lines by Artuvazro
                        originalList = fileRoot.Descendants("original").Select(x => x.Value).ToList();
                        editedList = fileRoot.Descendants("edited").Select(x => x.Value).ToList();
                        //for (int count = 0; count < original.Capacity; count++)
                        //{
                        //    Console.WriteLine(original[count]);
                        //    Console.WriteLine("");
                        //}
                        var smtivDic = new Dictionary<char, char>
                            {
                                { '\u00F1', '\u30F3' }, //ン from ñ
                                { '\u00E4', '\u30D9' }, //ベ from ä
                                { '\u00E1', '\u30E9' }, //ラ from á
                                { '\u00EF', '\u30DA' }, //ペ from ï
                                { '\u00ED', '\u30EA' }, //リ from í
                                { '\u00FC', '\u30DB' }, //ホ from ü
                                { '\u00FA', '\u30EB' }, //ル from ú
                                { '\u00EB', '\u30DC' }, //ボ from ë
                                { '\u00E9', '\u30EC' }, //レ from é
                                { '\u00F6', '\u30DD' }, //ポ from ö
                                { '\u00F3', '\u30ED' }, //ロ from ó
                                { '\u00C4', '\u30A1' }, //ァ from Ä
                                { '\u00C1', '\u30A2' }, //ア from Á
                                { '\u00CF', '\u30A3' }, //ィ from Ï
                                { '\u00CD', '\u30A4' }, //イ from Í
                                { '\u00DC', '\u30A5' }, //ゥ from Ü
                                { '\u00DA', '\u30A6' }, //ウ from Ú
                                { '\u00CB', '\u30A7' }, //ェ from Ë
                                { '\u00C9', '\u30A8' }, //エ from É
                                { '\u00D6', '\u30A9' }, //ォ from Ö
                                { '\u00D3', '\u30AA' }, //オ from Ó
                                { '\u00A1', '\u30D1' }, //パ from ¡
                                { '\u00BF', '\u30D7' }, //プ from ¿
                            };
                        Console.WriteLine("Replacing characters...");
                        
                            string editedDef_HW = "";
                            foreach (var entry in editedList)
                            {
                                editedDef_HW = "";
                                foreach (char c in entry)
                                {
                                    if (smtivDic.ContainsKey(c)) //If the dic has a char available in the string, replace it
                                    {
                                        editedDef_HW += smtivDic[c];
                                    }
                                    else //If not, just construct the string like normal
                                    {
                                        editedDef_HW += c;
                                    }
                                }



                                Console.WriteLine("Converting to Full-Width...");
                                string regExSearch = "(<.*?>)";
                                string fwConvString = "";
                                string[] regExSplit = Regex.Split(editedDef_HW, regExSearch);
                                foreach (string match in regExSplit)
                                {
                                    Match m = Regex.Match(match, regExSearch);
                                    if (!m.Success)
                                    {
                                        int LocaleID = 0;
                                        string fwConv = Strings.StrConv(match, VbStrConv.Wide, LocaleID = 1041);
                                        fwConvString += fwConv;

                                    }
                                    else
                                    {
                                        fwConvString += match;
                                    }

                                }
                                editedSJISList.Add(fwConvString);
                            }
                            //int LocaleID = 0;
                            //string fwConv = Strings.StrConv(editedDef_HW, VbStrConv.Wide, LocaleID = 1041); //Full-Width conversion
                            //editedSJISList.Add(fwConv); 



                            for (int i = 1; i < editedSJISList.Count; i++)
                            {
                                fileRoot.Elements("entries").Descendants("entry").Where(e => e.Attribute("name").Value.Equals("Text " + i)).Select(e => e.Element("edited")).Single().SetValue(editedSJISList[i - 1]);
                            }
                            Console.WriteLine("¡Archivo convertido con exito!");
                            Console.ReadLine();

                            Directory.CreateDirectory(dir + "\\conv\\");
                            file.Save(dir + "\\conv\\" + fileName, SaveOptions.None);
                            
                            
                            }
                    
                    }   
                 
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine(ex);
                        Console.ReadLine();
                        throw;
                    }
                    
            }
            
        }
          
    }
    
}
