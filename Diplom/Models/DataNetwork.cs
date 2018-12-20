using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Models
{
	public static class DataNetwork
	{
		public static string Name { get; set; }
		public static bool IsCreate { get; set; } = false;
		public static string Type { get; set; }
		public static ObservableCollection<ManagerControl> Managers { get; set; }
		public static ObservableCollection<StationControl> Stations { get; set; }

		static DataNetwork()
		{
			Managers = new ObservableCollection<ManagerControl>();
			Stations = new ObservableCollection<StationControl>();
		}
	}
}
