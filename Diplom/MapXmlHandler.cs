using Diplom.Models;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace Diplom
{
    static class MapXmlHandler
    {
        public static string LastPath { get; set; }

        private enum Kind { PDH_Relay, PDH_Management }

        private static string MapKey = "MAP";
        private static string NetworkKey = "NETWORK";
        private static string ObjectKey = "OBJECT";
        private static string PortKey = "PORT";
        private static string TimingsKey = "TIMINGS";

        private static string IdAttr = "ID";
        private static string NameAttr = "NAME";
        private static string ColorAttr = "COLOR";
        private static string KindAttr = "KIND";
        private static string RemoteAttr = "REMOTE";
        private static string PolledAttr = "POLLED";
        private static string LeftAttr = "LEFT";
        private static string TopAttr = "TOP";
        private static string ObjectIdAttr = "OBJECTID";

        private static string AskAttr = "ASK";
        private static string NetUpdateAttr = "NETUPDATE";
        private static string TimeoutAttr = "TIMEOUT";
        private static string DelayAttr = "DELAY";

        public static void WriteMap(string path)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.GetEncoding("windows-1251")
            };

            XmlWriter writer = XmlWriter.Create(path, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement(MapKey);
            if (DataNetwork.IsCreated)
            {
                writer.WriteStartElement(NetworkKey);

                writer.WriteAttributeString(IdAttr, DataNetwork.Address.ToString());
                writer.WriteAttributeString(NameAttr, DataNetwork.Name);
                writer.WriteAttributeString(ColorAttr, HexConverter(DataNetwork.CurrentColor));
                writer.WriteAttributeString(KindAttr, DataNetwork.Type);
                writer.WriteAttributeString(RemoteAttr, "NO");
                writer.WriteAttributeString(PolledAttr, "NO");

                foreach (StationControl station in DataNetwork.Stations)
                {
                    writer.WriteStartElement(ObjectKey);
                    writer.WriteAttributeString(NameAttr, station.Data.Name);
                    writer.WriteAttributeString(LeftAttr, Canvas.GetLeft(station).ToString());
                    writer.WriteAttributeString(TopAttr, Canvas.GetTop(station).ToString());
                    writer.WriteAttributeString(IdAttr, station.Data.Number.ToString());
                    writer.WriteAttributeString(KindAttr, Kind.PDH_Relay.ToString());
                    if (station.stationLine != null)
                    {
                        var line = station.stationLine;
                        var objectid = (line.firstControl == station) ?
                            (line.secondControl as StationControl).Data.Number : (line.firstControl as StationControl).Data.Number;

                        writer.WriteStartElement(PortKey);
                        writer.WriteAttributeString(IdAttr, "2");
                        writer.WriteAttributeString(ObjectIdAttr, objectid.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                foreach (ManagerControl manager in DataNetwork.Managers)
                {
                    writer.WriteStartElement(ObjectKey);
                    writer.WriteAttributeString(NameAttr, manager.Data.Name);
                    writer.WriteAttributeString(LeftAttr, Canvas.GetLeft(manager).ToString());
                    writer.WriteAttributeString(TopAttr, Canvas.GetTop(manager).ToString());
                    writer.WriteAttributeString(IdAttr, manager.Data.Number.ToString());
                    writer.WriteAttributeString(KindAttr, Kind.PDH_Management.ToString());

                    writer.WriteStartElement(TimingsKey);
                    writer.WriteAttributeString(AskAttr, "5000");
                    writer.WriteAttributeString(NetUpdateAttr, "3600000");
                    writer.WriteAttributeString(TimeoutAttr, "2000");
                    writer.WriteAttributeString(DelayAttr, "200");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        public static void ReadMap(string path)
        {

        }

        private static string HexConverter(Color c) =>
            "0x" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    }
}
