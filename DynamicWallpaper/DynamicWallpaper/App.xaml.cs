using System.Windows;
using DynamicWallpaper.Utils;

namespace DynamicWallpaper {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App: Application {
        protected override async void OnExit(ExitEventArgs e) {
            //将资源字典文件持久化到本地
            await ResourceUtil.Save();
            base.OnExit(e);
        }
    }

}
