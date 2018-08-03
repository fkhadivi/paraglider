using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace IGP
{
    class Text
    {
        public new string ToString()
        {
            return text;
        }

        public string key = "";
        public string lang = "";
        public string text = "";
    }

    /// <summary>
    /// Global (static) storage for application texts in different languages.
    /// </summary>
    class TextProvider
    {
        /// <summary> Dictionary of text key/value pairs. This is the prefered source for text, alternatively the INI file is searched. </summary>
        protected static Dictionary<string, string> dict = new Dictionary<string, string>();
        
        /// <summary> Always set the correct language before getting or setting text! </summary>
        public static string lang = "en";

        /// <summary> Alternative language - used if the current language is not found. </summary>
        public static string altLang = "en";

        /// <summary> The file to read text from. The CSV file has priority over ini file. </summary>
        public static ITextFile textFile = null;

        /// <summary> Load an INI text file. </summary>
        public static bool Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                if (fileName.IndexOf("csv") != -1)
                    textFile = new CsvTextFile(Env.GetFullFileName(fileName));
                else if (fileName.IndexOf("xls") != -1 || fileName.IndexOf("xlsx") != -1)
                    textFile = new ExcelTextFile(Env.GetFullFileName(fileName));
                else 
                    textFile = new IniFile(Env.GetFullFileName(fileName));

                Log.Message("Text file loaded from '" + fileName + ".");
                return true;
            }
            Log.Warning("Text file '" + fileName + "' not found.");
            return false;
        }

        public static string GetText(string key)
        {
            return GetText(key, lang);
        }

        /// <summary> Get a text by its key name - using the currently set language in "lang". Do not specify the language in the key here. </summary>
        public static string GetText(string key, string _lang)
        {
            key = key.Replace("{", "");
            key = key.Replace("}", "");
            string keyLang = key+"."+ _lang;
            if (dict.ContainsKey(keyLang))
            {
                return FormatText(dict[keyLang]);
            }
            else
            {
                if (textFile != null)
                {
                    string r = textFile.ReadValue("text", keyLang);
                    if (r == null || r.Length == 0)
                    {
                        string keyAltLang = key + "." + altLang;
                        if (dict.ContainsKey(keyAltLang))
                        {
                            return FormatText(dict[keyAltLang]);
                        }
                        else
                        {
                            r = textFile.ReadValue("text", keyAltLang);
                            if (r == null || r.Length == 0)
                            {
                                return "_[" + keyLang + "]";
                            }
                        }
                    }

                    return FormatText(r);
                }
            }
            return "_[" + keyLang + "]";
        }

        public static string GetTextWithLanguage(string key, string _lang)
        {
            return GetText(key, _lang);
        }

            ///
            public static bool HasText(string key)
        {
            string keyLang = key + "." + lang;
            if (GetText(key) == "_[" + keyLang + "]") return false;
            return true;
        }
        
        /// <summary> Set a text by its key name, same rules as in "GetText()". </summary>
        public static void SetText(string key, string value)
        {
            string keyLang = key/*+"."+lang*/;
            dict[keyLang] = value;
        }

        /// <summary> Method for post- editing texts, used by "GetText()". Here, the line-break is made compatible with WPF. </summary>
        public static string FormatText(string t)
        {
            if (t == null) return null;
            string text = t.Replace("\\n", "\n");
            //text = text.Replace("\n", "{0}");
            //text = string.Format(text, Environment.NewLine);
            return text;
        }

    }
}
