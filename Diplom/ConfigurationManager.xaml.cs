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
    /// Interaction logic for ConfigurationManager.xaml
    /// </summary>
    public partial class ConfigurationManager : Window
    {
        public ConfigurationManager()
        {
            InitializeComponent();

			List<string> list = new List<string>();
			for (int i = 1; i <= Stock.numberLimit; ++i)
			{
				if (!Stock.workWindow.numbersManagers.Contains(i))
					list.Add(i.ToString());
			}
				
			listOfAdress.ItemsSource = list;
            listOfAdress.SelectedIndex = listOfAdress.Items.Count - 1;

            nameNewManager.Text = $"Безымянный";
        }


        private void CloseWindow(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void CreateManager(object sender, RoutedEventArgs e)
		{
			int number = int.Parse(listOfAdress.SelectedItem.ToString());
			Stock.workWindow.CreateManager(nameNewManager.Text.Trim(), number);
			Close();
		}
    }
}
