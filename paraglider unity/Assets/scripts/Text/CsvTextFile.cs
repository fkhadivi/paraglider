using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class CsvTextFile : ITextFile
    {
        private Dictionary<string, string> dict = new Dictionary<string, string>();
        public string File;

        public CsvTextFile(string file)
        {
            this.File = file;

            ReadCsv();
        }

        private bool ReadCsv()
        {
            if (System.IO.File.Exists(File))
            {
                List<List<object>> table;
                try
                {
                    table = CSVReader.ReadCSVFile(File);
                }
                catch (Exception e)
                {
                    Debug.LogError("Could not open text.csv. Open in another program? " + e.Message);
                    return false;
                }

                string[] lang = new string[table[0].Count];
                for (int a = 0; a < table[0].Count; a++)
                    lang[a] = table[0][a].ToString();

                string theKey = "";
                string theValue = "";

                for (int row = 1; row < table.Count; row++)
                {
                    for (int column = 1; column < table[row].Count; column++)
                    {
                        theKey = (string)table[row][0];
                        if (theKey.Length > 0 && column < table[0].Count && lang[column].Length > 0)
                        {
                            theKey += "." + lang[column];
                            theValue = table[row][column].ToString();
                            //Debug.Log(theKey + " -> " + theValue);
                            try
                            {
                                dict.Add(theKey, theValue);
                            }
                            catch (Exception e)
                            {
                                dict[theKey] = theValue;
                                Debug.LogWarning(theKey + ":  " +e.Message);
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
    


