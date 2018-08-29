namespace NPNDAutoVNC
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Odbc;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    internal class CTLImportFileCSV
    {
        public static string sqlConnString = "server=(local);\r\n\tdatabase=Test_CSV_impex;Trusted_Connection=True";

        public DataSet LoadCSV(int numberOfRows)
        {
            DataSet dataSet = new DataSet();
            try
            {
                string str2;
                OdbcConnection selectConnection = new OdbcConnection(("Driver={Microsoft Text Driver (*.txt; *.csv)};\r\n\t\t\tDbq=" + Path.Combine(Application.StartupPath, "DS Folder.csv") + ";\r\n\t\t\tExtensions=asc,csv,tab,txt;Persist Security Info=False").Trim());
                selectConnection.Open();
                if (numberOfRows == -1)
                {
                    str2 = "select * from [list.csv]";
                }
                else
                {
                    str2 = "select top " + numberOfRows + " * from [list.csv]";
                }
                new OdbcDataAdapter(str2, selectConnection).Fill(dataSet, "csv");
                selectConnection.Close();
            }
            catch (Exception exception)
            {
                CtlError.WriteError("LoadCSV ", exception.Message);
                return null;
            }
            return dataSet;
        }

        public static DataTable ReadFromCsv(string fileName, Encoding encoding, char separator)
        {
            DataTable table = null;
            if (((fileName == null) || fileName.Equals(string.Empty)) || !File.Exists(fileName))
            {
                throw new FileNotFoundException("Error in ReadFromCsv: the file path could not be found.");
            }
            try
            {
                FileInfo info = new FileInfo(fileName);
                string name = info.Name;
                List<string> list = new List<string>();
                StreamReader reader = new StreamReader(fileName, encoding);
                for (string str2 = reader.ReadLine(); str2 != null; str2 = reader.ReadLine())
                {
                    list.Add(str2);
                }
                List<string[]> list2 = new List<string[]>();
                int length = 0;
                foreach (string str3 in list)
                {
                    string[] item = str3.Split(new char[] { separator });
                    if (item.Length > length)
                    {
                        length = item.Length;
                    }
                    list2.Add(item);
                }
                table = new DataTable(name);
                foreach (string[] strArray2 in list2)
                {
                    table.Rows.Add(strArray2);
                }
                table.AcceptChanges();
            }
            catch (Exception)
            {
                throw new Exception("Error in ReadFromCsv: IO error.");
            }
            return table;
        }
    }
}

