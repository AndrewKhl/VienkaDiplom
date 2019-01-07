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
    /// Логика взаимодействия для OnOffStation.xaml
    /// </summary>
    public partial class OnOffStation : Window
    {
		private StationControl _currentStation;

        public OnOffStation(StationControl station)
        {
			_currentStation = station;
            InitializeComponent();

			selectType.SelectedIndex = station.Data.State == "включено" ? 0 : 1;
        }

		private void SetNewState(object sender, RoutedEventArgs e)
		{
			string newState = (selectType.SelectedItem as ComboBoxItem).Content.ToString();
			_currentStation.Data.State = newState;

			ParamsWindow pWnd = Owner as ParamsWindow;

			if (newState == "включено")
				pWnd.VisualTree(pWnd.MainTree, Visibility.Visible);
			else
				pWnd.VisualTree(pWnd.MainTree, Visibility.Hidden);

			pWnd.StateStation.Visibility = Visibility.Visible;

			_currentStation.Data.firstRefreshStation = DateTime.MinValue;
			Stock.workWindow.RemoveLocalConnection(_currentStation);
			Close();
		}
	}
}
