using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для ConfigurationNetwork.xaml
    /// </summary>
    public partial class ConfigurationNetwork : Window
    {
        public ConfigurationNetwork()
        {
            InitializeComponent();

			List<string> list = new List<string>();
			for (int i = 1; i <= 55; ++i)
			{
				if (!Stock.workWindow.numberStations.Contains(i))
					list.Add(i.ToString());
			}
				
			listOfAdress.ItemsSource = list;
			listOfAdress.SelectedIndex = 0;
        }

		private void CloseWindow(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void CreateNetwork(object sender, RoutedEventArgs e)
		{
			Stock.workWindow.numberStations.Add(int.Parse(listOfAdress.SelectedItem.ToString()));
			Stock.workWindow.CreateManager();
			Stock.workWindow.CreateStation(nameNewStation.Text.Trim());
			Close();
		}

	}
}
