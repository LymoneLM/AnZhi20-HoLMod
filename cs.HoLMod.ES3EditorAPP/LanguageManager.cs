using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace cs.HoLMod.ES3EditorAPP
{
    internal class LanguageManager
    {
        // 单例模式
        private static readonly LanguageManager instance = new LanguageManager();
        private string currentLanguageCode = "zh-CN"; // 默认中文
        private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>();

        // 私有构造函数
        private LanguageManager()
        {
            // 初始化语言翻译字典
            InitializeTranslations();
        }

        // 获取单例实例
        public static LanguageManager Instance
        {
            get { return instance; }
        }

        // 初始化翻译
        private void InitializeTranslations()
        {
            // 中文翻译
            translations["zh-CN"] = new Dictionary<string, string>
            {
                {"Title", "EasySave3 编辑器"},
                {"Password", "密码"},
                {"DecryptButton", "解密/解压"},
                {"OpenFileButton", "打开文件"},
                {"SaveFileButton", "保存文件"},
                {"Language", "语言"},
                {"Status", "状态: 就绪"},
                {"NoPasswordMessage", "不知道游戏的密码? 请检查下面是否已经知道。"},
                {"DecryptionNote", "一些游戏可能不会加密他们的保存文件，只会使用GZip压缩。在这种情况下，您不需要提供密码。"},
                {"EncryptionType", "加密方式"},
                {"GZip", "GZip"},
                {"FileOpenedSuccess", "文件已成功打开"},
                {"FileSavedSuccess", "文件已成功保存"},
                {"DecryptionSuccess", "解密/解压成功"},
                {"DecryptionFailed", "解密/解压失败"},
                {"InvalidFile", "无效的文件格式"},
                {"FileNotFound", "找不到文件"},
                {"ProcessingFailed", "文件处理失败"}
            };

            // 英文翻译
            translations["en-US"] = new Dictionary<string, string>
            {
                {"Title", "EasySave3 Editor"},
                {"Password", "Password"},
                {"DecryptButton", "Decrypt/Extract"},
                {"OpenFileButton", "Open File"},
                {"SaveFileButton", "Save File"},
                {"Language", "Language"},
                {"Status", "Status: Ready"},
                {"NoPasswordMessage", "Don't know the password for your game? Check if it is already known below."},
                {"DecryptionNote", "Some games might not encrypt their save files and only compress them using GZip. In this case, you don't have to provide a password."},
                {"EncryptionType", "Encryption"},
                {"GZip", "GZip"},
                {"FileOpenedSuccess", "File opened successfully"},
                {"FileSavedSuccess", "File saved successfully"},
                {"DecryptionSuccess", "Decryption/Extraction successful"},
                {"DecryptionFailed", "Decryption/Extraction failed"},
                {"InvalidFile", "Invalid file format"},
                {"FileNotFound", "File not found"},
                {"ProcessingFailed", "File processing failed"}
            };
        }

        // 设置当前语言
        public void SetLanguage(string languageCode)
        {
            if (translations.ContainsKey(languageCode))
            {
                currentLanguageCode = languageCode;
            }
            else
            {
                // 如果请求的语言不存在，使用默认语言
                currentLanguageCode = "zh-CN";
            }
        }

        // 获取翻译
        public string GetTranslation(string key)
        {
            if (translations[currentLanguageCode].ContainsKey(key))
            {
                return translations[currentLanguageCode][key];
            }
            else
            {
                // 如果键不存在，返回键本身
                return key;
            }
        }

        // 获取支持的语言列表
        public List<string> GetSupportedLanguages()
        {
            return translations.Keys.ToList();
        }

        // 获取语言显示名称
        public string GetLanguageDisplayName(string languageCode)
        {
            switch (languageCode)
            {
                case "zh-CN": return "中文简体";
                case "en-US": return "English";
                // 可以在这里添加更多语言的显示名称
                default:
                    return languageCode;
            }
        }
    }
}