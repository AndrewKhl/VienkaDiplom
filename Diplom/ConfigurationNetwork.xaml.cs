using Diplom.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Diplom
{
    public partial class ConfigurationNetwork : Window
    {
        public bool IsEditing { get; set; } = false;
        public ConfigurationNetwork()
        {
            InitializeComponent();

			List<string> list = new List<string>();
			for (int i = 1; i <= Stock.numberLimit; ++i)
                list.Add(i.ToString());
				
			listOfAdress.ItemsSource = list;
			listOfAdress.SelectedIndex = 0;

            nameNewNetwork.Text = $"Безымянный";
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => Close();

		private void CreateNetwork(object sender, RoutedEventArgs e)
		{
			int number = int.Parse(listOfAdress.SelectedItem.ToString());
            DataNetwork.CurrentColor = colorCanvas.SelectedColor ?? Colors.Green;
            if (!IsEditing)
                Stock.workWindow.CreateManager(nameNewNetwork.Text.Trim(), number);
			DataNetwork.Name = nameNewNetwork.Text.Trim();
			DataNetwork.Type = (typeNetwork.SelectedItem as ComboBoxItem).Content.ToString();
            DataNetwork.Address = number;
			DataNetwork.IsCreated = true;
			Stock.workWindow.EnabledButton(true);
			Close();
            if (!IsEditing)
                Stock.workWindow.CreateStation_Click(sender, e);
            if (IsEditing)
                Stock.workWindow.UpdateColors();

            Stock.workWindow.MapChanged();
		}
	}
}
