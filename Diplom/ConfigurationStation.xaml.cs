using Diplom.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Diplom
{
    public partial class ConfigurationStation : Window
    {
        private StationControl station;
        public ConfigurationStation(StationControl editedStation = null)
        {
            InitializeComponent();

			List<string> list = new List<string>();
			for (int i = 1; i <= Stock.numberLimit; ++i)
			{
                //if (!Stock.workWindow.numbersStations.Contains(i))
                if (!Stock.workWindow.numbersControls.Contains(i))
                    list.Add(i.ToString());
			}

            if (editedStation != null)
            {
                station = editedStation;
                nameNewStation.Text = station.Data.Name;
                string item = station.Data.Number.ToString();
                list.Add(item);
                list.OrderBy(x => int.Parse(x)).ToList();
                listOfAdress.ItemsSource = list;
                listOfAdress.SelectedItem = item;
            }
            else
            {
                nameNewStation.Text = $"Безымянная";
                listOfAdress.ItemsSource = list;
                listOfAdress.SelectedIndex = 0;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => Close();

		private void CreateNetwork(object sender, RoutedEventArgs e)
		{
			int number = int.Parse(listOfAdress.SelectedItem.ToString());
            if (station == null)
            {
                Stock.workWindow.CreateStation(nameNewStation.Text.Trim(), number);
            }
            else
            {
                //Stock.workWindow.numbersStations.Remove(station.Data.Number);
                //Stock.workWindow.numbersStations.Add(number);
                Stock.workWindow.numbersControls.Remove(station.Data.Number);
                Stock.workWindow.numbersControls.Add(number);

                station.Data.Number = number;
                station.Data.Name = nameNewStation.Text;
                station.SetVisibleName();
            }
            Close();
		}
	}
}
