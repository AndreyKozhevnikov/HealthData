using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HealthDataExport {
    class Program {
        static void Main(string[] args) {
            //  if (args)
            flPath = args[0].ToString();//.Split('\\').LastOrDefault();

            flDirectory = Directory.GetParent(args[0].ToString()).ToString();
            OpenFile();
            CalculateData();
            Console.Read();
        }
        static List<HData> ListData;
        static string flDirectory;// = @"f:\dropbox\common\hd\";
        static string flPath;
        public static void OpenFile() {

            // string flPath = @"f:\dropbox\common\hd\export.xml";
            ListData = new List<HData>();
            XmlTextReader reader = new XmlTextReader(flPath);
            while (reader.Read()) {

                // Обработка данных.
                switch (reader.NodeType) {
                    case XmlNodeType.Element: // Узел является элементом.
                        //Console.Write("<" + reader.Name + reader.Value);
                        //Console.WriteLine(">");
                        if (reader.Name == "Record") {
                            HData dt = new HData();
                            dt.Type = reader.GetAttribute("type");
                            dt.Source = reader.GetAttribute("source");
                            dt.StartDate = reader.GetAttribute("endDate");
                            double tmpValue = -1;
                            string dblString = reader.GetAttribute("value");
                            var b = double.TryParse(dblString, out tmpValue);
                            if (!b) {
                                dblString = dblString.Replace(".", ",");
                                double.TryParse(dblString, out tmpValue);
                            }
                            if (tmpValue == -1) {//? удалить
                                new NullReferenceException();
                            }
                            else {
                                dt.Value = tmpValue;
                            }
                            //dt.Value = double.Parse(reader.GetAttribute("value"));
                            dt.CalculateDate();
                            ListData.Add(dt);
                            //   Console.WriteLine(dt.GetString());
                        }
                        break;
                }
            }


        }
        public static void CalculateData() {
            var v = ListData.Where(x => x.Source == "Andrey's iPhone" && x.Type == "HKQuantityTypeIdentifierDistanceWalkingRunning").ToList();
            var v2 = v.GroupBy(x => x.Date).Select(y => new { dt = y.Key, val = (int)y.Sum(x => x.Value) }).OrderBy(z=>z.dt).ToList();
          
            string flName = flDirectory + @"\exp" + DateTime.Now.ToString("MMddyy") + ".txt";
            StreamWriter sw = new StreamWriter(flName);
            foreach (var el in v2) {
                string dts = el.dt.ToShortDateString();
                Console.WriteLine(dts + " " + el.val);
                sw.WriteLine(dts + ";" + el.val);
            }
            sw.Close();

        }
    }

    public class HData {
        public string Type { get; set; }
        public string Source { get; set; }
        public string StartDate { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }
        public void CalculateDate() {
            Date = new DateTime(int.Parse(StartDate.Substring(0, 4)), int.Parse(StartDate.Substring(4, 2)), int.Parse(StartDate.Substring(6, 2)));
        }
        public string GetString() {
            return String.Format("{0} {1} {2} {3}", Type, Source, StartDate, Value);

        }
    }
}
