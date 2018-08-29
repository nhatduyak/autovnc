namespace NPNDAutoVNC
{
    using System;
    using System.Data;
    using System.Data.OleDb;
    using System.Drawing;

    public class CTLImportExcel
    {
        public string ChuoiKetNoi = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={sourcefile}; Jet OLEDB:Engine Type=5;Extended Properties=Excel 8.0;";
        private string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;\r\n               Data Source={sourcefile};Extended Properties=\"Excel 12.0;HDR=YES;\"";

        public DataTable getDataFromXLS(string strFilePath)
        {
            DataTable table;
            try
            {
                OleDbConnection connection = new OleDbConnection(this.ChuoiKetNoi.Replace("{sourcefile}", strFilePath));
                connection.Open();
                OleDbCommand command = new OleDbCommand("SELECT * FROM [Sheet1$]", connection);
                OleDbDataAdapter adapter = new OleDbDataAdapter {
                    SelectCommand = command
                };
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                connection.Close();
                adapter = null;
                table = dataTable;
                table.Columns.Add("NgayModify");
                table.Columns.Add("Image", typeof(Image));
                table.Columns.Add("Loai");
                connection.Close();
            }
            catch (Exception exception)
            {
                CtlError.WriteError("CTLImportExcel GetDataFromXLS", exception.Message);
                return null;
            }
            return table;
        }

        public DataTable getDataFromXLS2007(string strFilePath)
        {
            DataTable table;
            try
            {
                OleDbConnection connection = new OleDbConnection(this.connectionString.Replace("{sourcefile}", strFilePath));
                connection.Open();
                OleDbCommand command = new OleDbCommand("SELECT * FROM [Sheet1$]", connection);
                OleDbDataAdapter adapter = new OleDbDataAdapter {
                    SelectCommand = command
                };
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                connection.Close();
                adapter = null;
                table = dataTable;
                table.Columns.Add("NgayModify");
                table.Columns.Add("Image", typeof(byte[]));
            }
            catch (Exception exception)
            {
                CtlError.WriteError("CTLImportExcel GetDataFromXLS2007", exception.Message);
                return null;
            }
            return table;
        }
    }
}

