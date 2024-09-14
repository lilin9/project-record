using DynamicWallpaper.Utils;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace DynamicWallpaper {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    ///
    /// 最后一步需要完成的：将项目打包发布成可执行文件，但是目前的问题是无法连同资源文件一起打包
    /// 可执行文件启动时找不到资源文件
    /// </summary>
    public partial class MainWindow: MainWindowNotifyIcon {
        private Wallpaper? _wallpaper;
        private const string IsAutoRunningKey = "IsAutoRunning";
        private const string ShortcutNameKey = "ShortcutName";

        public MainWindow() {
            InitializeComponent();
            InitializeNotifyIcon();
            SetCheckBox();  //页面加载时，设置 开机自启动复选框 的选中状态
        }

        /// <summary>
        /// 确认按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SureBtn_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(textBlock.Text) || !System.IO.File.Exists(textBlock.Text)) {
                return;
            }

            //点击确认按钮的时候，检查一下 开机自启动复选框 的选中状态
            CheckBox_Click(sender, e);

            _wallpaper = Wallpaper.GetInstance(textBlock.Text);
            _wallpaper.Show();
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, CancelEventArgs e) {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 文件选择按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedFileBtn_Click(object sender, RoutedEventArgs e) {
            //创建文件选择弹出框
            var openFileDialog = new OpenFileDialog {
                Filter = "HTML Files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*"
            };

            //显示对话框，处理选择结果
            if (openFileDialog.ShowDialog() != true) {
                return;
            }
            //获取文件路径
            var filePath = openFileDialog.FileName;
            ShowTextBlockText(filePath);
        }

        /// <summary>
        /// 文件被拖拽到文本框时，获取文件路径，
        /// 读取内容，显示到文本框内
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_Drop(object sender, DragEventArgs e) {
            //检查拖拽的数据是否包含文件
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                return;
            }

            //获取拖拽的文件路径
            var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (files is not { Length: > 0 }) {
                return;
            }

            if (!FileUtil.IsHtmlFile(files[0])) {
                return;
            }
            //将文件路径显示到文本框内
            ShowTextBlockText(files[0]);
        }

        /// <summary>
        /// 在拖拽过程中检测拖拽的数据是否包含文件，并显示拖拽效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_PreviewDragOver(object sender, DragEventArgs e) {
            //检查拖拽的文件是否包含数据
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ?
                DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// CheckBox开机自启复选框点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            var isChecked = checkBox.IsChecked is true;
            //复选框没有选中
            if (!isChecked) {
                //删除系统中软件自启动的快捷键
                DeleteShortcut();
            } else {    //复选框选中
                //创建软件的快捷方式，在系统启动文件夹中
                CreateShortcut();
            }
            //将用户的选择结果持久化
            var checkedStr = isChecked ? "true" : "false";
            ResourceUtil.SetValue(IsAutoRunningKey, checkedStr);
        }

        /// <summary>
        /// 在系统启动文件夹中创建软件的快捷方式
        /// </summary>
        private void CreateShortcut() {
            var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var shortcutPath = Path.Combine(startupFolderPath, ResourceUtil.GetValue(ShortcutNameKey));
            var targetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            //如果快捷方式已经存在就退出程序
            if (FileUtil.IsExists(shortcutPath)) {
                return;
            }

            var shell = new WshShell();
            var shortCut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortCut.TargetPath = targetPath;
            shortCut.Save();
        }

        /// <summary>
        /// 删除快捷方式
        /// </summary>
        private void DeleteShortcut() {
            var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var shortcutPath = Path.Combine(startupFolderPath, ResourceUtil.GetValue(ShortcutNameKey));
            FileUtil.DeleteFile(shortcutPath);
        }

        /// <summary>
        /// 显示文本框的文本内容
        /// </summary>
        /// <param name="filePath"></param>
        private void ShowTextBlockText(string filePath) {
            textBlock.Text = FileUtil.FormatFilePath(filePath);
        }

        /// <summary>
        /// 根据资源文件的IsAutoRunning的情况，设置 开机自启动复选框 的选中情况
        /// </summary>
        private void SetCheckBox() {
            var isChecked = ResourceUtil.GetValue(IsAutoRunningKey) == "true";
            checkBox.IsChecked = isChecked;
        }

    }
}