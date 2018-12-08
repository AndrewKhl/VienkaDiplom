using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Diplom
{
	public partial class MainWindow : Window
	{

		[DllImport("user32.dll")]
		static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

		private const int GWL_STYLE = -16;

		private const uint WS_SYSMENU = 0x80000;

		protected override void OnSourceInitialized(EventArgs e)
		{
			IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_STYLE,
				GetWindowLong(hwnd, GWL_STYLE) & (0xFFFFFFFF ^ WS_SYSMENU));

			base.OnSourceInitialized(e);
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Close_Window(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Open_Work_Window(object sender, RoutedEventArgs e)
		{
			WorkWindow wind = new WorkWindow();
			wind.Show();
			Close();
		}
	}
}
