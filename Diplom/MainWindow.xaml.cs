using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

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

        private static readonly string AppdataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MasterLink3");
        private static readonly string ConfigPath = Path.Combine(AppdataFolder, "Config.xml");

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
			Close();
			wind.Show();
		}
	}
}
