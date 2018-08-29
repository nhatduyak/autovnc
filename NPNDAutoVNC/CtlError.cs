namespace NPNDAutoVNC
{
    using System;
    using System.IO;

    internal class CtlError
    {
        public static string _path = Environment.CurrentDirectory;

        public static bool WriteError(string title, string ex)
        {
            try
            {
                FileInfo info = new FileInfo(Path.Combine(_path, "ErrorLog"));
                if (!info.Exists)
                {
                    info.Create();
                }
                FileStream stream = new FileStream(info.FullName, FileMode.Append);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine("");
                writer.WriteLine("");
                writer.WriteLine("");
                writer.WriteLine("");
                writer.WriteLine("-------------------------" + DateTime.Now + "---------------------------------");
                writer.WriteLine("C\x00f3 vấn đề sảy ra tại " + title);
                writer.WriteLine(ex);
                writer.Dispose();
                writer.Close();
                stream.Dispose();
                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool WriteError(string file_name, string Title, string ex)
        {
            try
            {
                FileInfo info = new FileInfo(Path.Combine(_path, file_name));
                if (!info.Exists)
                {
                    info.Create();
                }
                FileStream stream = new FileStream(info.FullName, FileMode.Append);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine("");
                writer.WriteLine("");
                writer.WriteLine("");
                writer.WriteLine("");
                writer.WriteLine("-------------------------" + DateTime.Now + "---------------------------------");
                writer.WriteLine(Title);
                writer.WriteLine(ex);
                writer.Dispose();
                writer.Close();
                stream.Dispose();
                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

