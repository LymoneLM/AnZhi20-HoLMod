using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using UnityEngine;

namespace cs.HoLMod.AutoArchive
{
    internal class Setting
    {
        // 自动存档间隔的枚举类型
        public enum AutoSaveInterval
        {
            ThreeMonths = 3,    // 每3个月
            SixMonths = 6,      // 每6个月
            TwelveMonths = 12   // 每12个月
        }

        // 配置项
        public static ConfigEntry<bool> EnableAutoSave { get; private set; }
        public static ConfigEntry<AutoSaveInterval> SaveInterval { get; private set; }
        private static float lastTimeChecked = 0f; // 上次检查的时间（游戏时间）
        private const float CHECK_INTERVAL_SECONDS = 30f; // 检查间隔（秒）- 实际项目中可以调整为更大的值

        // 初始化配置
        public static void Init(ConfigFile config)
        {
            EnableAutoSave = config.Bind(
                "AutoSave",
                "EnableAutoSave",
                false,
                "是否启用自动存档功能"
            );

            SaveInterval = config.Bind(
                "AutoSave",
                "SaveInterval",
                AutoSaveInterval.SixMonths,
                "自动存档间隔时间（3、6、12个月）"
            );
        }

        // 检查是否应该执行自动存档
        public static bool ShouldAutoSave()
        {
            // 如果未启用自动存档，则不执行
            if (!EnableAutoSave.Value)
            {
                return false;
            }

            // 获取当前游戏时间
            if (Mainload.Time_now == null || Mainload.Time_now.Count < 3)
            {
                return false;
            }

            int currentMonth = Mainload.Time_now[1]; // 索引1对应月份
            AutoSaveInterval interval = SaveInterval.Value;

            // 根据不同的间隔设置判断是否应该存档
            switch (interval)
            {
                case AutoSaveInterval.ThreeMonths:
                    // 每3个月存档：3月、6月、9月、12月
                    return currentMonth == 3 || currentMonth == 6 || currentMonth == 9 || currentMonth == 12;
                case AutoSaveInterval.SixMonths:
                    // 每6个月存档：6月、12月
                    return currentMonth == 6 || currentMonth == 12;
                case AutoSaveInterval.TwelveMonths:
                    // 每12个月存档：12月
                    return currentMonth == 12;
                default:
                    return false;
            }
        }

        // 定期检查函数（由游戏主循环调用）
        public static void CheckTimePeriodically()
        {
            // 如果未启用自动存档，不需要检查
            if (!EnableAutoSave.Value)
                return;

            // 使用Unity的Time.time来跟踪检查间隔
            float currentTime = Time.time;
            if (currentTime - lastTimeChecked >= CHECK_INTERVAL_SECONDS)
            {
                lastTimeChecked = currentTime;
                
                // 检查Mainload.Time_now[1]的值（月份）
                if (Mainload.Time_now != null && Mainload.Time_now.Count >= 3)
                {
                    int currentMonth = Mainload.Time_now[1];
                    Debug.Log("自动存档定期检查 - 当前月份: " + currentMonth);
                    
                    // 这里可以添加额外的日志或调试信息
                }
            }
        }
    }
}
