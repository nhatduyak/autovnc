namespace NPNDAutoVNC
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    internal class CTLConfig
    {
        public static string _ControlIDIP;
        public static string _ControlIDPass;
        public static int _hightApp;
        public static Point _IDFA;
        public static Point _IDFAGen;
        public static Point _IDFAIden;
        public static Point _IDFASett;
        public static Point _Link;
        public static string _PathVNC;
        public static Point _PointAD;
        public static Point _PointAppCl;
        public static Point _PointFirtApp;
        public static Point _PointOK;
        public static Point _pointRightClick;
        public static Point _PointStartApp;
        public static Point _PointVNCConnect;
        public static string _ProcessName;
        public static Point _ReLink;
        public static Point _SetLink;
        public static int _SLApp;
        public static string _Sleep;
        public static int _SoluongReset = 5;
        public static string _Time1Connectvnc;
        public static string _Time1to24;
        public static string _Time2inputPass;
        public static string _Time3Connect;
        public static string _Time4rightClick;
        public static string _Time5closeapp;
        public static string _Time6StartApp;
        public static int _WidthApp;

        public static void GetConfiguration()
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(Environment.CurrentDirectory + @"\Config.xml");
                _ProcessName = document.SelectSingleNode("//ProcessName").Attributes["Value"].Value;
                _ControlIDIP = document.SelectSingleNode("//ControlIDIP").Attributes["Value"].Value;
                _ControlIDPass = document.SelectSingleNode("//ControlIDPass").Attributes["Value"].Value;
                _PathVNC = document.SelectSingleNode("//Path").Attributes["Value"].Value;
                _Sleep = document.SelectSingleNode("//Sleep").Attributes["Value"].Value;
                _Time1to24 = document.SelectSingleNode("//Time1to24").Attributes["Value"].Value;
                string[] strArray = document.SelectSingleNode("//PointADAPP").Attributes["Value"].Value.Split(new char[] { ',' });
                _PointAD = new Point(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1]));
                string[] strArray2 = document.SelectSingleNode("//PointAppClo").Attributes["Value"].Value.Split(new char[] { ',' });
                _PointAppCl = new Point(Convert.ToInt32(strArray2[0]), Convert.ToInt32(strArray2[1]));
                string[] strArray3 = document.SelectSingleNode("//PointRightClick").Attributes["Value"].Value.Split(new char[] { ',' });
                _pointRightClick = new Point(Convert.ToInt32(strArray3[0]), Convert.ToInt32(strArray3[1]));
                string str4 = document.SelectSingleNode("//PointStartApp").Attributes["Value"].Value;
                string[] strArray4 = str4.Split(new char[] { ',' });
                _PointStartApp = new Point(Convert.ToInt32(str4[0]), Convert.ToInt32(str4[1]));
                string[] strArray5 = document.SelectSingleNode("//PointFirtApp").Attributes["Value"].Value.Split(new char[] { ',' });
                _PointFirtApp = new Point(Convert.ToInt32(strArray5[0]), Convert.ToInt32(strArray5[1]));
                string[] strArray6 = document.SelectSingleNode("//PointVNCConnect").Attributes["Value"].Value.Split(new char[] { ',' });
                _PointVNCConnect = new Point(Convert.ToInt32(strArray6[0]), Convert.ToInt32(strArray6[1]));
                string[] strArray7 = document.SelectSingleNode("//PointOK").Attributes["Value"].Value.Split(new char[] { ',' });
                _PointOK = new Point(Convert.ToInt32(strArray7[0]), Convert.ToInt32(strArray7[1]));
                string[] strArray8 = document.SelectSingleNode("//Link").Attributes["Value"].Value.Split(new char[] { ',' });
                _Link = new Point(Convert.ToInt32(strArray8[0]), Convert.ToInt32(strArray8[1]));
                string[] strArray9 = document.SelectSingleNode("//ReLink").Attributes["Value"].Value.Split(new char[] { ',' });
                _ReLink = new Point(Convert.ToInt32(strArray9[0]), Convert.ToInt32(strArray9[1]));
                string[] strArray10 = document.SelectSingleNode("//SetLink").Attributes["Value"].Value.Split(new char[] { ',' });
                _SetLink = new Point(Convert.ToInt32(strArray10[0]), Convert.ToInt32(strArray10[1]));
                _SLApp = Convert.ToInt32("0" + document.SelectSingleNode("//SLApp").Attributes["Value"].Value);
                _WidthApp = Convert.ToInt32("0" + document.SelectSingleNode("//WidthApp").Attributes["Value"].Value);
                _hightApp = Convert.ToInt32("0" + document.SelectSingleNode("//HightApp").Attributes["Value"].Value);
                _Time1Connectvnc = document.SelectSingleNode("//time1Connectvnc").Attributes["Value"].Value;
                _Time2inputPass = document.SelectSingleNode("//time2inputPass").Attributes["Value"].Value;
                _Time3Connect = document.SelectSingleNode("//time3Connect").Attributes["Value"].Value;
                _Time4rightClick = document.SelectSingleNode("//time4rightclick").Attributes["Value"].Value;
                _Time5closeapp = document.SelectSingleNode("//time5closeapp").Attributes["Value"].Value;
                _Time6StartApp = document.SelectSingleNode("//time6startapp").Attributes["Value"].Value;
                string[] strArray11 = document.SelectSingleNode("//IDFA").Attributes["Value"].Value.Split(new char[] { ',' });
                _IDFA = new Point(Convert.ToInt32(strArray11[0]), Convert.ToInt32(strArray11[1]));
                string[] strArray12 = document.SelectSingleNode("//IDFASett").Attributes["Value"].Value.Split(new char[] { ',' });
                _IDFASett = new Point(Convert.ToInt32(strArray12[0]), Convert.ToInt32(strArray12[1]));
                string[] strArray13 = document.SelectSingleNode("//IDFAIden").Attributes["Value"].Value.Split(new char[] { ',' });
                _IDFAIden = new Point(Convert.ToInt32(strArray13[0]), Convert.ToInt32(strArray13[1]));
                string[] strArray14 = document.SelectSingleNode("//IDFAGen").Attributes["Value"].Value.Split(new char[] { ',' });
                _IDFAGen = new Point(Convert.ToInt32(strArray14[0]), Convert.ToInt32(strArray14[1]));
                _SoluongReset = Convert.ToInt32("0" + document.SelectSingleNode("//SoluongReset").Attributes["Value"].Value);
            }
            catch (Exception exception)
            {
                CtlError.WriteError("CTLConfig getconfig", exception.Message);
            }
        }

        public static void SetHightApp(string hight)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(Environment.CurrentDirectory + @"\Config.xml");
                document.SelectSingleNode("//HightApp").Attributes["Value"].Value = hight;
                document.Save(Path.Combine(Application.StartupPath, "Config.xml"));
            }
            catch (Exception exception)
            {
                CtlError.WriteError("Loi Setvalue Hight", exception.Message);
            }
        }

        public static void SetSLApp(string slapp)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(Environment.CurrentDirectory + @"\Config.xml");
                document.SelectSingleNode("//SLApp").Attributes["Value"].Value = slapp;
                document.Save(Path.Combine(Application.StartupPath, "Config.xml"));
            }
            catch (Exception exception)
            {
                CtlError.WriteError("Loi Setvalue SLApp", exception.Message);
            }
        }

        public static void Setvalue(string Time1to24)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(Environment.CurrentDirectory + @"\Config.xml");
                document.SelectSingleNode("//Time1to24").Attributes["Value"].Value = Time1to24;
                document.Save(Path.Combine(Application.StartupPath, "Config.xml"));
            }
            catch (Exception exception)
            {
                CtlError.WriteError("Loi Setvalue Time1to24", exception.Message);
            }
        }

        public static void SetWidthApp(string width)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(Environment.CurrentDirectory + @"\Config.xml");
                document.SelectSingleNode("//WidthApp").Attributes["Value"].Value = width;
                document.Save(Path.Combine(Application.StartupPath, "Config.xml"));
            }
            catch (Exception exception)
            {
                CtlError.WriteError("Loi Setvalue width", exception.Message);
            }
        }
    }
}

