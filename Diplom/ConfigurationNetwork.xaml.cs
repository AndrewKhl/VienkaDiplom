using Diplom.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

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
			for (int i = 1; i <= Stock.numberLimit; ++i)
			{
				if (!Stock.workWindow.numbersManagers.Contains(i))
					list.Add(i.ToString());
			}
				
			listOfAdress.ItemsSource = list;
			listOfAdress.SelectedIndex = 0;

            nameNewNetwork.Text = $"Безымянный";
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void CreateNetwork(object sender, RoutedEventArgs e)
		{
			int number = int.Parse(listOfAdress.SelectedItem.ToString());
            Stock.workWindow.currentColor = colorCanvas.SelectedColor ?? Colors.Green;
			Stock.workWindow.CreateManager(nameNewNetwork.Text.Trim(), number);
			DataNetwork.Name = nameNewNetwork.Text.Trim();
			DataNetwork.Type = typeNetwork.SelectedItem.ToString();
			DataNetwork.IsCreate = true;
			Stock.workWindow.EnabledButton(true);
			Close();
			Stock.workWindow.CreateStation_Click(sender, e);
		}
	}
}
