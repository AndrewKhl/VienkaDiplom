using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Models
{
	public class DataStation: INotifyPropertyChanged
	{
		private int _period = 0;
		private string _main = "Ведомая";
		private string _synchronization = "Внутренняя";
		public bool _onStation = true;

		public DateTime firstRefreshStation = DateTime.MinValue;
		public string Name { get; set; }
		public int Number { get; set; }

		public string State
		{
			get
			{
				return _onStation == true ? "включено" : "выключено";
			}

			set
			{
				_onStation = value == "Включено" ? true : false;
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
