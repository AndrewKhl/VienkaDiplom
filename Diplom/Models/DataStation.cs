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

		public string Name { get; set; }
		public int Number { get; set; }
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

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName]string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
