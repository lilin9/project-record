using System.Collections;
using System.Windows;
using System.Windows.Markup;

namespace DynamicWallpaper.Utils {
    public static class ResourceUtil {
        private static readonly ResourceDictionary? CustomResources = Application.Current.Resources.MergedDictionaries[0];

        /// <summary>
        /// 将资源文件持久化到本地文件
        /// </summary>
        /// <returns></returns>
        public static async Task Save() {
            //创建一个字典存储资源键值对
            var resource = new Dictionary<string, string>();
            //遍历所有资源
            foreach (DictionaryEntry entry in Application.Current.Resources.MergedDictionaries[0]) {
                //将字符类型的资源的键值对存入字典
                if (entry.Value is string str) {
                    resource[entry.Key.ToString()!] = str;
                }
            }

            //将资源字典转换为 XAML 字符串
            var resourceDictionary = new ResourceDictionary();
            foreach (var kvp in resource) {
                resourceDictionary.Add(kvp.Key, kvp.Value);
            }
            var xamlString = XamlWriter.Save(resourceDictionary);

            //保存到本地文件
            var resourcesPath = FileUtil.GetRootPath() + "Resources/Resources.xaml";
            await System.IO.File.WriteAllTextAsync(resourcesPath, xamlString);
        }

        /// <summary>
        /// 根据 key 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key) {
            if (CustomResources == null) {
                return string.Empty;
            }
            return (CustomResources[key] as string)!;
        }

        /// <summary>
        /// 根据 key 设置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(string key, string value) {
            if (CustomResources == null) {
                return;
            }

            CustomResources[key] = value;
        }
    }
}
