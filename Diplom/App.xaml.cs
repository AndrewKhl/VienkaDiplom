using System.Globalization;
using System.Threading;
using System.Windows;

namespace Diplom
{
    public partial class App : Application
	{
        protected override void OnStartup(StartupEventArgs e)
        {
            var culture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            base.OnStartup(e);
        }
    }
}
