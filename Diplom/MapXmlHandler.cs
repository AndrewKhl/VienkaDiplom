using Diplom.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

        private static readonly string MapKey = "MAP";
        private static readonly string NetworkKey = "NETWORK";
        private static readonly string ObjectKey = "OBJECT";
        private static readonly string PortKey = "PORT";
        private static readonly string TimingsKey = "TIMINGS";

        private static readonly string IdAttr = "ID";
        private static readonly string NameAttr = "NAME";
        private static readonly string ColorAttr = "COLOR";
        private static readonly string KindAttr = "KIND";
        private static readonly string RemoteAttr = "REMOTE";
        private static readonly string PolledAttr = "POLLED";
        private static readonly string LeftAttr = "LEFT";
        private static readonly string TopAttr = "TOP";
        private static readonly string ObjectIdAttr = "OBJECTID";

        private static readonly string AskAttr = "ASK";
        private static readonly string NetUpdateAttr = "NETUPDATE";
        private static readonly string TimeoutAttr = "TIMEOUT";
        private static readonly string DelayAttr = "DELAY";

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
                    writer.WriteAttributeString(LeftAttr, ((int)Canvas.GetLeft(station)).ToString());
                    writer.WriteAttributeString(TopAttr, ((int)Canvas.GetTop(station)).ToString());
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
                    writer.WriteAttributeString(LeftAttr, ((int)Canvas.GetLeft(manager)).ToString());
                    writer.WriteAttributeString(TopAttr, ((int)Canvas.GetTop(manager)).ToString());
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
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlNode map = document.DocumentElement.SelectSingleNode($"/{MapKey}");
            XmlNode network = map.SelectSingleNode($"//{NetworkKey}");
            if (network != null)
            {
                var colorValue = network.Attributes[ColorAttr].Value;
                if (colorValue.Substring(0, 2) == "0x")
                {
                    colorValue = colorValue.Remove(0, 2);
                    colorValue = "#" + colorValue;
                }
                var color = (Color)ColorConverter.ConvertFromString(colorValue);
                LoadNetwork(
                    network.Attributes[NameAttr].Value,
                    int.Parse(network.Attributes[IdAttr].Value),
                    network.Attributes[KindAttr].Value,
                    color);

                var controls = network.SelectNodes(ObjectKey);
                Dictionary<int, int> connections = new Dictionary<int, int>();
                foreach (XmlNode control in controls)
                {
                    Kind kind = (Kind)Enum.Parse(typeof(Kind), control.Attributes[KindAttr].Value);
                    switch (kind)
                    {
                        case Kind.PDH_Relay:
                            string name = control.Attributes[NameAttr].Value;
                            int controlId = int.Parse(control.Attributes[IdAttr].Value);
                            int top = int.Parse(control.Attributes[TopAttr].Value);
                            int left = int.Parse(control.Attributes[LeftAttr].Value);
                            Stock.workWindow.CreateStation(name, controlId, top, left);

                            foreach (XmlNode port in control.ChildNodes)
                            {
                                int connectedWith = int.Parse(port.Attributes[ObjectIdAttr].Value);
                                int min = Math.Min(controlId, connectedWith);
                                int max = Math.Max(controlId, connectedWith);
                                if (connections.ContainsKey(min) && connections[min] == max)
                                {
                                    connections.Remove(min);
                                    Stock.workWindow.LoadStationConnection(min, max);
                                }
                                else
                                {
                                    connections.Add(min, max);
                                }
                            }
                            break;
                        case Kind.PDH_Management:
                            Stock.workWindow.CreateManager(
                                control.Attributes[NameAttr].Value,
                                int.Parse(control.Attributes[IdAttr].Value),
                                int.Parse(control.Attributes[TopAttr].Value),
                                int.Parse(control.Attributes[LeftAttr].Value));
                            break;
                    }
                }
            }
        }

        private static string HexConverter(Color c) =>
            "0x" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        private static void LoadNetwork(string name, int number, string type, Color color)
        {
            DataNetwork.CurrentColor = color;
            DataNetwork.Name = name;
            DataNetwork.Type = type;
            DataNetwork.Address = number;
			DataNetwork.IsCreated = true;
			Stock.workWindow.EnabledButton(true);
        }

        public static bool ReadLastPath()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey("Micran", true);
            key = key.OpenSubKey("Magapp", true);
            object path = key.GetValue("LastMap");
            if (path != null && !string.IsNullOrEmpty(path as string))
            {
                LastPath = path as string;
                return true;
            }
            return false;
        }

        public static void WriteLastPath()
        {
            if (!string.IsNullOrEmpty(LastPath))
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

                try { key = key.OpenSubKey("Micran", true); }
                catch (Exception) { key = key.CreateSubKey("Micran", true); }

                try { key = key.OpenSubKey("Magapp", true); }
                catch (Exception) { key = key.CreateSubKey("Magapp", true); }

                key.SetValue("LastMap", LastPath);
            }
        }
    }
}
