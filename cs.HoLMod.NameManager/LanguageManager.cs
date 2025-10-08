using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs.HoLMod.NameManager
{
    internal class LanguageManager
    {
        // 当前语言类型
        public static int CurrentLanguageType = 0; // 0: 中文, 1: 英文
        
        // 设置当前语言
        public static void SetLanguage(int languageType)
        {
            CurrentLanguageType = languageType;
        }
        
        // 获取当前语言的文本
        public static string GetText(string chineseText, string englishText)
        {
            return CurrentLanguageType == 0 ? chineseText : englishText;
        }
        
        // 根据分隔符获取当前语言的文本（用于名称等格式为"中文|英文"的字符串）
        public static string GetTextBySeparator(string textWithSeparator, char separator = '|')
        {
            if (string.IsNullOrEmpty(textWithSeparator))
            {
                return string.Empty;
            }
            
            string[] parts = textWithSeparator.Split(separator);
            if (parts.Length >= 2)
            {
                return CurrentLanguageType == 0 ? parts[0] : parts[1];
            }
            
            return textWithSeparator;
        }
    }
}
