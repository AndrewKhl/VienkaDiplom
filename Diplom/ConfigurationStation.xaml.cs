using System.Collections.Generic;
using System.Windows;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для ConfigurationStation.xaml
    /// </summary>
    public partial class ConfigurationStation : Window
    {
        public ConfigurationStation()
        {
            InitializeComponent();

			List<string> list = new List<string>();
			for (int i = 1; i <= Stock.numberLimit; ++i)
			{
                if (!Stock.workWindow.numbersStations.Contains(i))
                    list.Add(i.ToString());
			}

			listOfAdress.ItemsSource = list;
			listOfAdress.SelectedIndex = 0;

            nameNewStation.Text = $"Безымянная";
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void CreateNetwork(object sender, RoutedEventArgs e)
		{
			int number = int.Parse(listOfAdress.SelectedItem.ToString());
			Stock.workWindow.CreateStation(nameNewStation.Text.Trim(), number);
			Close();
		}
	}
}
