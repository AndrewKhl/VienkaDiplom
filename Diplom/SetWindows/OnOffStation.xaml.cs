using Diplom.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Diplom
{
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
			{
				pWnd.VisualTree(pWnd.MainTree, Visibility.Visible);
				Close();
			}
			else
			{
				pWnd.VisualTree(pWnd.MainTree, Visibility.Hidden);
				pWnd.StateStation.Visibility = Visibility.Visible;
				_currentStation.Data.firstRefreshStation = DateTime.MinValue;
				Stock.workWindow.RemoveLocalConnection(_currentStation);
				pWnd.Close();
			}
		}
	}
}
