using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Diplom.Models
{
    public static class DataNetwork
	{
		public static string Name { get; set; }
		public static bool IsCreated { get; set; } = false;
		public static string Type { get; set; }
        public static int Address { get; set; }
        public static Color CurrentColor { get; set; }
		public static ObservableCollection<ManagerControl> Managers { get; set; }
		public static ObservableCollection<StationControl> Stations { get; set; }

		static DataNetwork()
		{
			Managers = new ObservableCollection<ManagerControl>();
			Stations = new ObservableCollection<StationControl>();
		}
	}
}
