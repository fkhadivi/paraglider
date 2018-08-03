using System;
using System.Collections.Generic;
using Excel;

namespace IGP
{
    public class ExcelTextFile : ITextFile
    {
        private Dictionary<string, string> dict = new Dictionary<string, string>();
        public string file;

        public ExcelTextFile(string _file)
        {
            this.file = _file;

            ReadExcel();
        }

        private bool ReadExcel()
        {
            if (System.IO.File.Exists(file))
            {
                List<List<string>> table = new List<List<string>>();
                try
                {
                    foreach (worksheet worksheet in Workbook.Worksheets(file))
                    {
                        foreach (Row row in worksheet.Rows)
                        {
                            List<string> r = new List<string>();
                            table.Add(r);
                            foreach (Cell cell in row.Cells)
                            {
                                if(cell != null)
                                    r.Add(cell.Text);
                                else 
                                    r.Add("");
                            }
                        }
                        break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Could not open text.xlsx. Open in another program? " + e.Message);
                    return false;
                }

                string[] lang = new string[table[0].Count];
                for (int a = 0; a < table[0].Count; a++)
                    lang[a] = table[0][a].ToString();

                string theKey = "";
                string theValue = "";

                int numlanguages = table[0].Count - 1;

                for (int row = 1; row < table.Count; row++)
                {
                    for (int column = 1; column < table[row].Count; column++)
                    {
                        theKey = table[row][0].ToString();
                        if (theKey.Length > 0  && column < table[0].Count)
                        {
                            if (lang[column].Length > 0)
                            {
                                theKey += "." + lang[column];
                                theValue = table[row][column].ToString();
                                //Log.Message(theKey + " -> " + theValue);
                                try
                                {
                                    dict.Add(theKey, theValue);
                                }
                                catch (Exception e)
                                {
                                    dict[theKey] = theValue;
                                    Log.Error(theKey + ":  " + e.Message);
                                }
                            }
                        }


                    }
                }
                return true;
            }
            return false;
        }




        public string ReadValue(string section, string key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return null;

        }
    }
    
}

