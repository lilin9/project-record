using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using DynamicWallpaper.Utils;

namespace DynamicWallpaper {
    /// <summary>
    /// 点击最小化将主窗口隐藏到系统托盘里面
    /// </summary>
    public class MainWindowNotifyIcon: Window {
        private NotifyIcon notifyIcon;

        protected void InitializeNotifyIcon() {
            var icoPath = FileUtil.GetRootPath() + "Resources/logo.ico";
            notifyIcon = new NotifyIcon {
                BalloonTipText = "应用程序已最小化到托盘",
                Text = "DynamicWallpaper",
                Icon = new Icon(icoPath),
                Visible = true
            };
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            var exitMenuItem = new ToolStripMenuItem("退出", null, ExitMenuItem_Click);
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(exitMenuItem);
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) {
                return;
            }
            Show();
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e) {
            notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e) {
            base.OnStateChanged(e);

            if (WindowState != WindowState.Minimized)
                return;
            Hide();
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(1000);
        }

        protected override void OnClosed(EventArgs e) {
            notifyIcon.Dispose();
            base.OnClosed(e);
        }
    }
}
