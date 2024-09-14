using System.IO;

namespace DynamicWallpaper.Utils {
    public static class FileUtil {

        /// <summary>
        /// 将Html文件保存到程序运行目录下
        /// </summary>
        /// <param name="filePath">新背景文件路径</param>
        public static async Task SaveHtmlAsync(string filePath) {
            if (!File.Exists(filePath)) {
                return;
            }

            var defaultHtmlPath = GetDefaultHtmlPath();

            var directory = defaultHtmlPath[..defaultHtmlPath.LastIndexOf('/')];
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            var file = await File.ReadAllTextAsync(filePath);
            File.Delete(defaultHtmlPath);
            await File.WriteAllTextAsync(defaultHtmlPath, file);
        }

        /// <summary>
        /// 判断是否是HTML类型的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsHtmlFile(string filePath) {
            if (!File.Exists(filePath)) {
                return false;
            }

            var extensions = Path.GetExtension(filePath).ToLower();
            return extensions is ".html" or ".htm";
        }

        /// <summary>
        /// 对文件路径进行格式化
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FormatFilePath(string filePath) {
            return !filePath.Contains("\\") ? filePath : filePath.Replace("\\", "/");
        }

        /// <summary>
        /// 获取默认html背景文件的位置
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultHtmlPath() {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var projectRootPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDirectory)!.FullName)!.FullName)!
                .FullName;
            var defaultPath = projectRootPath + "/Resources/Default.html";
            return FormatFilePath(defaultPath);
        }

        /// <summary>
        /// 获取项目根目录路径
        /// </summary>
        /// <returns></returns>
        public static string GetRootPath() {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            // var rootPath = path[..path.LastIndexOf("bin", StringComparison.Ordinal)];
            return FormatFilePath(path);
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsExists(string filePath) {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath) {
            if (!IsExists(filePath)) {
                return;
            }

            File.Delete(filePath);
        }
    }
}
