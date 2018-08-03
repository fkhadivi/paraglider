using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

     /// <summary>
    /// Read CSV-formatted data from a file or TextReader
    /// </summary>
    public class CSVReader : IDisposable
    {
        public const string NEWLINE = "\r\n";

        /// <summary>
        /// This reader will read all of the CSV data
        /// </summary>
        private BinaryReader reader;

        /// <summary>
        /// The number of rows to scan for types when building a DataTable (0 to scan the whole file)
        /// </summary>
        public int ScanRows = 0;

        #region Constructors

        /// <summary>
        /// Read CSV-formatted data from a file
        /// </summary>
        /// <param name="filename">Name of the CSV file</param>
        public CSVReader(FileInfo csvFileInfo)
        {
            if (csvFileInfo == null)
                throw new ArgumentNullException("Null FileInfo passed to CSVReader");

            this.reader = new BinaryReader(File.OpenRead(csvFileInfo.FullName));
        }

        /// <summary>
        /// Read CSV-formatted data from a string
        /// </summary>
        /// <param name="csvData">String containing CSV data</param>
        public CSVReader(string csvData)
        {
            if (csvData == null)
                throw new ArgumentNullException("Null string passed to CSVReader");


            this.reader = new BinaryReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvData)));
        }

        /// <summary>
        /// Read CSV-formatted data from a TextReader
        /// </summary>
        /// <param name="reader">TextReader that's reading CSV-formatted data</param>
        public CSVReader(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("Null TextReader passed to CSVReader");

            this.reader = new BinaryReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd())));
        }

        #endregion



        string currentLine = "";
        /// <summary>
        /// Read the next row from the CSV data
        /// </summary>
        /// <returns>A list of objects read from the row, or null if there is no next row</returns>
        public List<object> ReadRow()
        {
            // ReadLine() will return null if there's no next line
            if (reader.BaseStream.Position >= reader.BaseStream.Length)
                return null;

            StringBuilder builder = new StringBuilder();

            // Read the next line
            while ((reader.BaseStream.Position < reader.BaseStream.Length) && (!builder.ToString().EndsWith(NEWLINE)))
            {
                char c = reader.ReadChar();
                builder.Append(c);
            }

            currentLine = builder.ToString();
            if (currentLine.EndsWith(NEWLINE))
                currentLine = currentLine.Remove(currentLine.IndexOf(NEWLINE), NEWLINE.Length);

            // Build the list of objects in the line
            List<object> objects = new List<object>();
            while (currentLine != "")
                objects.Add(ReadNextObject());
            return objects;
        }

        /// <summary>
        /// Read the next object from the currentLine string
        /// </summary>
        /// <returns>The next object in the currentLine string</returns>
        private object ReadNextObject()
        {
            if (currentLine == null)
                return null;

            // Check to see if the next value is quoted
            bool quoted = false;
            if (currentLine.StartsWith("\""))
                quoted = true;

            // Find the end of the next value
            string nextObjectString = "";
            int i = 0;
            int len = currentLine.Length;
            bool foundEnd = false;
            while (!foundEnd && i <= len)
            {
                // Check if we've hit the end of the string
                if ((!quoted && i == len) // non-quoted strings end with a comma or end of line
                    || (!quoted && currentLine.Substring(i, 1) == ",")
                    // quoted strings end with a quote followed by a comma or end of line
                    || (quoted && i == len - 1 && currentLine.EndsWith("\""))
                    || (quoted && currentLine.Substring(i, 2) == "\","))
                    foundEnd = true;
                else
                    i++;
            }
            if (quoted)
            {
                if (i > len || !currentLine.Substring(i, 1).StartsWith("\""))
                    throw new FormatException("Invalid CSV format: " + currentLine.Substring(0, i));
                i++;
            }
            nextObjectString = currentLine.Substring(0, i).Replace("\"\"", "\"");

            if (i < len)
                currentLine = currentLine.Substring(i + 1);
            else
                currentLine = "";

            if (quoted)
            {
                if (nextObjectString.StartsWith("\""))
                    nextObjectString = nextObjectString.Substring(1);
                if (nextObjectString.EndsWith("\""))
                    nextObjectString = nextObjectString.Substring(0, nextObjectString.Length - 1);
                return nextObjectString;
            }
            else
            {
                object convertedValue;
                StringConverter.ConvertString(nextObjectString, out convertedValue);
                return convertedValue;
            }
        }

        /// <summary>
        /// Read the row data read using repeated ReadRow() calls and build a DataColumnCollection with types and column names
        /// </summary>
        /// <param name="headerRow">True if the first row contains headers</param>
        /// <returns>System.Data.DataTable object populated with the row data</returns>
        public List<List<object>> CreateDataTable()
        {
            // Read the CSV data into rows
            List<List<object>> rows = new List<List<object>>();
            List<object> readRow = null;
            while ((readRow = ReadRow()) != null)
                rows.Add(readRow);

            // The types and names (if headerRow is true) will be stored in these lists
            List<Type> columnTypes = new List<Type>();

            // Read the column types from each row in the list of rows
  
            foreach (List<object> row in rows)
                
                    for (int i = 0; i < row.Count; i++)
                        // If we're adding a new column to the columnTypes list, use its type.
                        // Otherwise, find the common type between the one that's there and the new row.
                        if (columnTypes.Count < i + 1)
                            columnTypes.Add(row[i].GetType());
                        else
                            columnTypes[i] = StringConverter.FindCommonType(columnTypes[i], row[i].GetType());
                

            return rows;
        }

        /// <summary>
        /// Read a CSV file into a table
        /// </summary>
        /// <param name="filename">Filename of CSV file</param>
        /// <param name="headerRow">True if the first row contains column names</param>
        /// <param name="scanRows">The number of rows to scan for types when building a DataTable (0 to scan the whole file)</param>
        /// <returns>System.Data.DataTable object that contains the CSV data</returns>
        public static List<List<object>> ReadCSVFile(string filename, int scanRows)
        {
            using (CSVReader reader = new CSVReader(new FileInfo(filename)))
            {
                reader.ScanRows = scanRows;
                return reader.CreateDataTable();
            }
        }

        /// <summary>
        /// Read a CSV file into a table
        /// </summary>
        /// <param name="filename">Filename of CSV file</param>
        /// <param name="headerRow">True if the first row contains column names</param>
        /// <returns>System.Data.DataTable object that contains the CSV data</returns>
        public static List<List<object>> ReadCSVFile(string filename)
        {
            using (CSVReader reader = new CSVReader(new FileInfo(filename)))
                return reader.CreateDataTable();
        }



        #region IDisposable Members

        public void Dispose()
        {
            if (reader != null)
            {
                try
                {
                    // Can't call BinaryReader.Dispose due to its protection level
                    reader.Close();
                }
                catch { }
            }
        }

        #endregion
    }

    public static class CSVReaderExtension
    {
        /// <summary>
        /// Convert a CSV-formatted string into a list of objects
        /// </summary>
        /// <returns>List of objects that contains the CSV data</returns>
        public static List<object> ReadCSVLine(this string input)
        {
            using (CSVReader reader = new CSVReader(input))
                return reader.ReadRow();
        }

        /// <summary>
        /// Convert a CSV-formatted string into a DataTable
        /// </summary>
        /// <param name="headerRow">True if the first row contains headers</param>
        /// <returns>System.Data.DataTable that contains the CSV data</returns>
        public static List<List<object>> ReadCSVTable(this string input)
        {
            using (CSVReader reader = new CSVReader(input))
                return reader.CreateDataTable();
        }
    }

    /// <summary>
    /// Static class to convert strings to typed values
    /// </summary>
    public static class StringConverter
    {
        public static Type ConvertString(string value, out object convertedValue)
        {
            // First check the whole number types, because floating point types will always parse whole numbers
            // Start with the smallest types
            byte byteResult;
            if (byte.TryParse(value, out byteResult))
            {
                convertedValue = byteResult;
                return typeof(byte);
            }

            short shortResult;
            if (short.TryParse(value, out shortResult))
            {
                convertedValue = shortResult;
                return typeof(short);
            }

            int intResult;
            if (int.TryParse(value, out intResult))
            {
                convertedValue = intResult;
                return typeof(int);
            }

            long longResult;
            if (long.TryParse(value, out longResult))
            {
                convertedValue = longResult;
                return typeof(long);
            }

            ulong ulongResult;
            if (ulong.TryParse(value, out ulongResult))
            {
                convertedValue = ulongResult;
                return typeof(ulong);
            }

            // No need to check the rest of the unsigned types, which will fit into the signed whole number types

            // Next check the floating point types
            float floatResult;
            if (float.TryParse(value, out floatResult))
            {
                convertedValue = floatResult;
                return typeof(float);
            }


            // It's not clear that there's anything that double.TryParse() and decimal.TryParse() will parse 
            // but which float.TryParse() won't
            double doubleResult;
            if (double.TryParse(value, out doubleResult))
            {
                convertedValue = doubleResult;
                return typeof(double);
            }

            decimal decimalResult;
            if (decimal.TryParse(value, out decimalResult))
            {
                convertedValue = decimalResult;
                return typeof(decimal);
            }

            // It's not a number, so it's either a bool, char or string
            bool boolResult;
            if (bool.TryParse(value, out boolResult))
            {
                convertedValue = boolResult;
                return typeof(bool);
            }

            char charResult;
            if (char.TryParse(value, out charResult))
            {
                convertedValue = charResult;
                return typeof(char);
            }

            convertedValue = value;
            return typeof(string);
        }

        /// <summary>
        /// Compare two types and find a type that can fit both of them
        /// </summary>
        /// <param name="typeA">First type to compare</param>
        /// <param name="typeB">Second type to compare</param>
        /// <returns>The type that can fit both types, or string if they're incompatible</returns>
        public static Type FindCommonType(Type typeA, Type typeB)
        {
            // Build the singleton type map (which will rebuild it in a typesafe manner
            // if it's not already built).
            BuildTypeMap();

            if (!typeMap.ContainsKey(typeA))
                return typeof(string);

            if (!typeMap[typeA].ContainsKey(typeB))
                return typeof(string);

            return typeMap[typeA][typeB];
        }


        // Dictionary to map two types to a common type that can hold both of them
        private static Dictionary<Type, Dictionary<Type, Type>> typeMap = null;

        // Locker object to build the singleton typeMap in a typesafe manner
        private static object locker = new object();

        /// <summary>
        /// Build the singleton type map in a typesafe manner.
        /// This map is a dictionary that maps a pair of types to a common type.
        /// So typeMap[typeof(float)][typeof(uint)] will return float, while
        /// typemap[typeof(char)][typeof(bool)] will return string.
        /// </summary>
        private static void BuildTypeMap()
        {
            lock (locker)
            {
                if (typeMap == null)
                {
                    typeMap = new Dictionary<Type, Dictionary<Type, Type>>()
                    {
                        // Comparing byte
                        {typeof(byte), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(byte) },
                            { typeof(short), typeof(short) },
                            { typeof(int), typeof(int) },
                            { typeof(long), typeof(long) },
                            { typeof(ulong), typeof(ulong) },
                            { typeof(float), typeof(float) },
                            { typeof(double), typeof(double) },
                            { typeof(decimal), typeof(decimal) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing short
                        {typeof(short), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(short) },
                            { typeof(short), typeof(short) },
                            { typeof(int), typeof(int) },
                            { typeof(long), typeof(long) },
                            { typeof(ulong), typeof(ulong) },
                            { typeof(float), typeof(float) },
                            { typeof(double), typeof(double) },
                            { typeof(decimal), typeof(decimal) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing int
                        {typeof(int), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(int) },
                            { typeof(short), typeof(int) },
                            { typeof(int), typeof(int) },
                            { typeof(long), typeof(long) },
                            { typeof(ulong), typeof(ulong) },
                            { typeof(float), typeof(float) },
                            { typeof(double), typeof(double) },
                            { typeof(decimal), typeof(decimal) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing long
                        {typeof(long), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(long) },
                            { typeof(short), typeof(long) },
                            { typeof(int), typeof(long) },
                            { typeof(long), typeof(long) },
                            { typeof(ulong), typeof(ulong) },
                            { typeof(float), typeof(float) },
                            { typeof(double), typeof(double) },
                            { typeof(decimal), typeof(decimal) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing ulong
                        {typeof(ulong), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(ulong) },
                            { typeof(short), typeof(ulong) },
                            { typeof(int), typeof(ulong) },
                            { typeof(long), typeof(ulong) },
                            { typeof(ulong), typeof(ulong) },
                            { typeof(float), typeof(float) },
                            { typeof(double), typeof(double) },
                            { typeof(decimal), typeof(decimal) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing float
                        {typeof(float), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(float) },
                            { typeof(short), typeof(float) },
                            { typeof(int), typeof(float) },
                            { typeof(long), typeof(float) },
                            { typeof(ulong), typeof(float) },
                            { typeof(float), typeof(float) },
                            { typeof(double), typeof(double) },
                            { typeof(decimal), typeof(decimal) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing double
                        {typeof(double), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(double) },
                            { typeof(short), typeof(double) },
                            { typeof(int), typeof(double) },
                            { typeof(long), typeof(double) },
                            { typeof(ulong), typeof(double) },
                            { typeof(float), typeof(double) },
                            { typeof(double), typeof(double) },
                            { typeof(decimal), typeof(decimal) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing decimal
                        {typeof(decimal), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(decimal) },
                            { typeof(short), typeof(decimal) },
                            { typeof(int), typeof(decimal) },
                            { typeof(long), typeof(decimal) },
                            { typeof(ulong), typeof(decimal) },
                            { typeof(float), typeof(decimal) },
                            { typeof(double), typeof(decimal) },
                            { typeof(decimal), typeof(decimal) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing bool
                        {typeof(bool), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(string) },
                            { typeof(short), typeof(string) },
                            { typeof(int), typeof(string) },
                            { typeof(long), typeof(string) },
                            { typeof(ulong), typeof(string) },
                            { typeof(float), typeof(string) },
                            { typeof(double), typeof(string) },
                            { typeof(decimal), typeof(string) },
                            { typeof(bool), typeof(bool) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing char
                        {typeof(char), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(string) },
                            { typeof(short), typeof(string) },
                            { typeof(int), typeof(string) },
                            { typeof(long), typeof(string) },
                            { typeof(ulong), typeof(string) },
                            { typeof(float), typeof(string) },
                            { typeof(double), typeof(string) },
                            { typeof(decimal), typeof(string) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(char) },
                            { typeof(string), typeof(string) },
                        }},

                        // Comparing string
                        {typeof(string), new Dictionary<Type, Type>() {
                            { typeof(byte), typeof(string) },
                            { typeof(short), typeof(string) },
                            { typeof(int), typeof(string) },
                            { typeof(long), typeof(string) },
                            { typeof(ulong), typeof(string) },
                            { typeof(float), typeof(string) },
                            { typeof(double), typeof(string) },
                            { typeof(decimal), typeof(string) },
                            { typeof(bool), typeof(string) },
                            { typeof(char), typeof(string) },
                            { typeof(string), typeof(string) },
                        }},

                    };
                }
            }
        }
    }


