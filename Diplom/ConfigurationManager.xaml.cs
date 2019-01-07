using Diplom.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Diplom
{
    public partial class ConfigurationManager : Window
    {
        private ManagerControl manager;
        public ConfigurationManager(ManagerControl editedManager = null)
        {
            InitializeComponent();

			List<string> list = new List<string>();
			for (int i = 1; i <= Stock.numberLimit; ++i)
			{
				if (!Stock.workWindow.numbersManagers.Contains(i))
					list.Add(i.ToString());
			}
				
            if (editedManager != null)
            {
                manager = editedManager;
                nameNewManager.Text = manager.Data.Name;
                string item = manager.Data.Number.ToString();
                list.Add(item);
                list.OrderBy(x => int.Parse(x));
                listOfAdress.ItemsSource = list;
                listOfAdress.SelectedItem = item;
            }
            else
            {
                nameNewManager.Text = $"Безымянный";
                listOfAdress.ItemsSource = list;
                listOfAdress.SelectedIndex = listOfAdress.Items.Count - 1;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => Close();

		private void CreateManager(object sender, RoutedEventArgs e)
		{
			int number = int.Parse(listOfAdress.SelectedItem.ToString());
            if (manager == null)
            {
                Stock.workWindow.CreateManager(nameNewManager.Text.Trim(), number);
            }
            else
            {
                Stock.workWindow.numbersStations.Remove(manager.Data.Number);
                Stock.workWindow.numbersStations.Add(number);
                manager.Data.Number = number;
                manager.Data.Name = nameNewManager.Text;
                manager.SetVisibleName();
            }
            Close();
		}
    }
}
