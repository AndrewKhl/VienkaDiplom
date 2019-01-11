using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Diplom.Models
{
    public class DataStation: INotifyPropertyChanged
	{
		private int _period = 238;
		private string _main = "Ведомая";
		private string _synchronization = "Внутренняя";
		public bool _onStation = true;

		public DateTime firstRefreshStation = DateTime.MinValue;
		public string Name { get; set; }
		public int Number { get; set; }

		public string State
		{
			get => _onStation ? "включено" : "выключено";
			set
			{
				_onStation = value == "Включено";
				OnPropertyChanged("State");
			}
		} 

		public int Period
		{
			get { return _period; }
			set
			{
				_period = value;
				OnPropertyChanged("Period");
			}
		}

		public string Main
		{
			get { return _main; }
			set
			{
				_main = value;
				OnPropertyChanged("Main");
			}
		}

		public string Synhronization
		{
			get { return _synchronization; }
			set
			{
				_synchronization = value;
				OnPropertyChanged("Synhronization");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName]string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
