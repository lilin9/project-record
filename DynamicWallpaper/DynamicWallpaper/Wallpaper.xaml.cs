using System.IO;
using System.Windows;
using DynamicWallpaper.Utils;
using WindowState = System.Windows.WindowState;

namespace DynamicWallpaper {
    /// <summary>
    /// Wallpaper.xaml 的交互逻辑
    /// </summary>
    public partial class Wallpaper: Window {
        //动态背景路径 
        private readonly string _filePath;
        //单例模式，保证全局唯一实例
        private static Wallpaper? _instance;

        private Wallpaper(string filePath) {
            _filePath = filePath;
            InitializeComponent();
        }

        /// <summary>
        /// 获取全局唯一单例
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Wallpaper GetInstance(string filePath) {
            return _instance ??= new Wallpaper(filePath);
        }

        /// <summary>
        /// 窗体初始化
        /// </summary>
        private async void Wallpaper_Loaded(object sender, RoutedEventArgs e) {
            //将传入的动态壁纸进行一个缓存
            await FileUtil.SaveHtmlAsync(_filePath);
            //设置窗体样式
            ResetWindowStyle();
            //设置动态壁纸
            await LoadDynamicWallpaper();
            //将窗体设置为桌面壁纸
            SetFullScreen();
        }

        /// <summary>
        /// 重置窗体样式
        /// </summary>
        private void ResetWindowStyle() {
            //将窗体设置为全屏，无边框
            Top = 0;
            Left = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// 加载动态壁纸
        /// </summary>
        /// <returns></returns>
        private async Task LoadDynamicWallpaper() {
            //加载动态壁纸
            var htmlStr = await File.ReadAllTextAsync(FileUtil.GetDefaultHtmlPath());
            await WebView.EnsureCoreWebView2Async(null);
            WebView.CoreWebView2.NavigateToString(htmlStr);
        }

        /// <summary>
        /// 将当前窗口设置成桌面
        /// </summary>
        private void SetFullScreen() {
            //通过类名查找一个窗口，返回窗口句柄
            var programIntPtr = Win32.FindWindow("Progman", null);

            //判断句柄是否有效
            if (programIntPtr == IntPtr.Zero) {
                return;
            }

            var result = IntPtr.Zero;
            //项 Program Manager 窗口发送 0x52c 的一个消息，超时设置为 0x3e8（1秒）
            Win32.SendMessageTimeout(programIntPtr, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 0x3e8, result);

            //遍历顶级窗口
            Win32.EnumWindows((hwnd, lParam) => {
                //找到包含 SHELLDLL_DefView 窗口的句柄 WorkerW
                if (Win32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero) {
                    //找到当前 WorkerW 窗口的后一个 WorkerW 窗口
                    var tempHwnd = Win32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
                    //隐藏窗口
                    Win32.ShowWindow(tempHwnd, 0);
                }

                return true;
            }, IntPtr.Zero);
            Win32.SetParent(new System.Windows.Interop.WindowInteropHelper(this).Handle, programIntPtr);
        }


    }
}
