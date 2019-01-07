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
	/// Логика взаимодействия для SetPeriodWindow.xaml
	/// </summary>
	public partial class SetPeriodWindow : Window
	{
		private StationControl _currentStation;
		public SetPeriodWindow(StationControl station)
		{
			InitializeComponent();
			_currentStation = station;
			selectPeriod.Text = station.Data.Period.ToString();
		}

		private void SetNewPeriod(object sender, RoutedEventArgs e)
		{
			int minPeriod = 238;
			int maxPeriod = 480;

			if (int.TryParse(selectPeriod.Text, out int newPeriod))
			{
				if (newPeriod >= minPeriod && newPeriod <= maxPeriod)
				{
					if (temporaryChange.IsEnabled == true)
					{
						if (!int.TryParse(temporaryChange.Text, out int timeChaged))
						{
							MessageBox.Show("Время изменения должно быть числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
							return;
						}
					}

					_currentStation.Data.Period = newPeriod;
					Close();
				}
				else
					MessageBox.Show($"Период должен быть в диапазоне от {minPeriod} до {maxPeriod}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
				MessageBox.Show("Период должен быть числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void SetChanged_Click(object sender, RoutedEventArgs e)
		{
			temporaryChange.IsEnabled = temporaryChange.IsEnabled == false ? true : false;
		}
	}
}
