using Diplom.Models;
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
	/// Логика взаимодействия для SetSynhronizationStation.xaml
	/// </summary>
	public partial class SetSynhronizationStation : Window
	{
		private StationControl _currentStation;
		public SetSynhronizationStation(StationControl station)
		{
			_currentStation = station;
			InitializeComponent();
			selectType.SelectedIndex = station.Data.Synhronization == "Внутренняя" ? 0 : 1;

		}

		private void SetSynhroniztionStation(object sender, RoutedEventArgs e)
		{
			_currentStation.Data.Synhronization = (selectType.SelectedItem as ComboBoxItem).Content.ToString();
			Close();
		}
	}
}
