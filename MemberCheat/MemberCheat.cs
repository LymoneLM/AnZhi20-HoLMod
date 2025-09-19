using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace cs.HoLMod.MenberCheat
{
    [BepInPlugin("cs.HoLMod.MemberCheat.AnZhi20", "HoLMod.MemberCheat", "1.1.0")]
    public class MenberCheat : BaseUnityPlugin
    {
        // 窗口设置
        private ConfigEntry<float> menuWidth;
        private ConfigEntry<float> menuHeight;
        private static Rect windowRect;
        private static bool showMenu = false;
        private static Vector2 scrollPosition = Vector2.zero;
        private static bool blockGameInput = false;
        private static bool hasSelectedCategory = false; // 控制是否已经选择了分类的变量，初始为false
        private int currentMode = 0; // 0: 族人修改, 1: 门客修改
        private string searchText = "";
        private string selectedMemberName = "";
        private int currentMemberSubMode = 0; // 0: 所有族人, 1: 族人家族, 2: 族人妻妾
        
        // 用于显示工具提示的变量
        private bool showTooltip = false;
        private string tooltipText = "";
        private Vector2 tooltipPosition = Vector2.zero;

        // 定义族人家族模式变量 (前缀A_)
        private string A_reputation = "0"; // 声誉 
        private string A_reputationOriginal = "0"; // 原始声誉
        private string A_title = "0"; // 功名
        private string A_titleOriginal = "0"; // 原始功名
        private string A_power = "0"; // 体力
        private string A_powerOriginal = "0"; // 原始体力
        private string A_age = "0"; // 年龄
        private string A_ageOriginal = "0"; // 原始年龄
        private string A_health = "0"; // 健康
        private string A_healthOriginal = "0"; // 原始健康
        private string A_mood = "0"; // 心情
        private string A_moodOriginal = "0"; // 原始心情
        private string A_charm = "0"; // 魅力
        private string A_charmOriginal = "0"; // 原始魅力
        private string A_luck = "0"; // 幸运
        private string A_luckOriginal = "0"; // 原始幸运
        private string A_preference = "0"; // 喜好
        private string A_preferenceOriginal = "0"; // 原始喜好
        private string A_character = "0"; // 品性
        private string A_characterOriginal = "0"; // 原始品性
        private string A_talent = "0"; // 天赋
        private string A_talentOriginal = "0"; // 原始天赋
        private string A_talentPoint = "0"; // 天赋点
        private string A_talentPointOriginal = "0"; // 原始天赋点
        private string A_skill = "0"; // 技能
        private string A_skillOriginal = "0"; // 原始技能
        private string A_skillPoint = "0"; // 技能点
        private string A_skillPointOriginal = "0"; // 原始技能点
        private string A_intelligence = "0"; // 文才
        private string A_intelligenceOriginal = "0"; // 原始文才
        private string A_weapon = "0"; // 武才
        private string A_weaponOriginal = "0"; // 原始武才
        private string A_business = "0"; // 商才
        private string A_businessOriginal = "0"; // 原始商才
        private string A_art = "0"; // 艺才
        private string A_artOriginal = "0"; // 原始艺才
        private string A_strategy = "0"; // 计谋
        private string A_strategyOriginal = "0"; // 原始计谋
        private string A_JueWei = "0"; // 爵位
        private string A_JueWeiOriginal = "0"; // 原始爵位
        private string A_FengDi = "0"; // 封地
        private string A_FengDiOriginal = "0"; // 原始封地
        
        // 定义族人妻妾模式变量 (前缀B_)
        private string B_reputation = "0"; // 声誉 
        private string B_reputationOriginal = "0"; // 原始声誉
        private string B_power = "0"; // 体力
        private string B_powerOriginal = "0"; // 原始体力
        private string B_age = "0"; // 年龄
        private string B_ageOriginal = "0"; // 原始年龄
        private string B_health = "0"; // 健康
        private string B_healthOriginal = "0"; // 原始健康
        private string B_mood = "0"; // 心情
        private string B_moodOriginal = "0"; // 原始心情
        private string B_charm = "0"; // 魅力
        private string B_charmOriginal = "0"; // 原始魅力
        private string B_luck = "0"; // 幸运
        private string B_luckOriginal = "0"; // 原始幸运
        private string B_preference = "0"; // 喜好
        private string B_preferenceOriginal = "0"; // 原始喜好
        private string B_character = "0"; // 品性
        private string B_characterOriginal = "0"; // 原始品性
        private string B_talent = "0"; // 天赋
        private string B_talentOriginal = "0"; // 原始天赋
        private string B_talentPoint = "0"; // 天赋点
        private string B_talentPointOriginal = "0"; // 原始天赋点
        private string B_skill = "0"; // 技能
        private string B_skillOriginal = "0"; // 原始技能
        private string B_skillPoint = "0"; // 技能点
        private string B_skillPointOriginal = "0"; // 原始技能点
        private string B_intelligence = "0"; // 文才
        private string B_intelligenceOriginal = "0"; // 原始文才
        private string B_weapon = "0"; // 武才
        private string B_weaponOriginal = "0"; // 原始武才
        private string B_business = "0"; // 商才
        private string B_businessOriginal = "0"; // 原始商才
        private string B_art = "0"; // 艺才
        private string B_artOriginal = "0"; // 原始艺才
        private string B_strategy = "0"; // 计谋
        private string B_strategyOriginal = "0"; // 原始计谋
        
        // 定义门客模式变量 (前缀C_)
        private string C_reputation = "0"; // 声誉 
        private string C_reputationOriginal = "0"; // 原始声誉
        private string C_power = "0"; // 体力
        private string C_powerOriginal = "0"; // 原始体力
        private string C_age = "0"; // 年龄
        private string C_ageOriginal = "0"; // 原始年龄
        private string C_health = "0"; // 健康
        private string C_healthOriginal = "0"; // 原始健康
        private string C_mood = "0"; // 心情
        private string C_moodOriginal = "0"; // 原始心情
        private string C_charm = "0"; // 魅力
        private string C_charmOriginal = "0"; // 原始魅力
        private string C_luck = "0"; // 幸运
        private string C_luckOriginal = "0"; // 原始幸运
        private string C_character = "0"; // 品性
        private string C_characterOriginal = "0"; // 原始品性
        private string C_talent = "0"; // 天赋
        private string C_talentOriginal = "0"; // 原始天赋
        private string C_talentPoint = "0"; // 天赋点
        private string C_talentPointOriginal = "0"; // 原始天赋点
        private string C_skill = "0"; // 技能
        private string C_skillOriginal = "0"; // 原始技能
        private string C_skillPoint = "0"; // 技能点
        private string C_skillPointOriginal = "0"; // 原始技能点
        private string C_intelligence = "0"; // 文才
        private string C_intelligenceOriginal = "0"; // 原始文才
        private string C_weapon = "0"; // 武才
        private string C_weaponOriginal = "0"; // 原始武才
        private string C_business = "0"; // 商才
        private string C_businessOriginal = "0"; // 原始商才
        private string C_art = "0"; // 艺才
        private string C_artOriginal = "0"; // 原始艺才
        private string C_strategy = "0"; // 计谋
        private string C_strategyOriginal = "0"; // 原始计谋
        private string C_MenKe_Reward = "0"; // 门客酬劳
        private string C_MenKe_RewardOriginal = "0"; // 原始门客酬劳
        
        // 从游戏中获取实际数据
        private List<string> memberList = new List<string>(); // 初始化空列表，实际数据在Update或OnGUI中加载
        private List<string> filteredMembers = new List<string>();

        private void Awake()
        {
            Logger.LogInfo("族人门客修改器已加载！");
            
            // 窗口配置
            menuWidth = Config.Bind("界面设置", "窗口宽度", 1000f, "族人门客修改器窗口的宽度");
            menuHeight = Config.Bind("界面设置", "窗口高度", 1400f, "族人门客修改器窗口的高度");
            
            windowRect = new Rect(100, 200, menuWidth.Value, menuHeight.Value);
            filteredMembers = memberList;
        }        
        private void Update()
        {
            // 按F3键切换窗口显示
            if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
            {
                showMenu = !showMenu;
                blockGameInput = showMenu;
                // 当打开窗口时，重置分类选择状态
                if (showMenu)
                {
                    hasSelectedCategory = false;
                }
                Logger.LogInfo(showMenu ? "族人门客修改器窗口已打开" : "族人门客修改器窗口已关闭");
            }
            
            // 阻止游戏输入当窗口显示时
            if (blockGameInput)
            {
                // 阻止鼠标滚轮
                if (UnityEngine.Input.mouseScrollDelta.y != 0)
                {
                    UnityEngine.Input.ResetInputAxes();
                }
                
                // 阻止鼠标点击
                if (UnityEngine.Input.GetMouseButton(0) || UnityEngine.Input.GetMouseButton(1) || UnityEngine.Input.GetMouseButton(2))
                {
                    UnityEngine.Input.ResetInputAxes();
                }
                
                // 阻止键盘输入（保留F3键用于关闭窗口）
                if (UnityEngine.Input.anyKeyDown && !UnityEngine.Input.GetKeyDown(KeyCode.F3))
                {
                    UnityEngine.Input.ResetInputAxes();
                }
            }
        }
        
        private void OnGUI()
        {
            // 重置工具提示变量
            showTooltip = false;
            
            if (!showMenu)
                return;
            
            // 允许窗口拖拽出游戏窗口 - 移除边界限制
            
            // 设置GUI样式
            GUI.skin.window.fontSize = 12;
            GUI.skin.window.padding = new RectOffset(20, 20, 10, 10);
            
            GUI.skin.label.fontSize = 12;
            GUI.skin.button.fontSize = 12;
            GUI.skin.textField.fontSize = 12;
            
            // 保存窗口背景色并设置为半透明
            Color originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.95f);
            
            // 显示一个半透明的背景遮罩，防止操作游戏界面
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
            GUI.color = new Color(0, 0, 0, 0.1f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.EndGroup();
            
            // 创建窗口（无标题栏）
            windowRect = GUI.Window(0, windowRect, DrawWindow, "", UnityEngine.GUI.skin.window);
            windowRect.width = menuWidth.Value;
            windowRect.height = menuHeight.Value;
            
            // 显示工具提示（在主窗口之外绘制）
            if (showTooltip && !string.IsNullOrEmpty(tooltipText))
            {
                // 确保工具提示不会超出屏幕边界
                Vector2 adjustedPosition = tooltipPosition;
                adjustedPosition.y -= 30; // 窗口显示在鼠标上方
                
                // 设置工具提示样式
                GUI.backgroundColor = new Color(0.95f, 0.95f, 0.95f, 0.95f);
                GUI.Window(999, new Rect(adjustedPosition, new Vector2(100, 30)), (id) => 
                {
                    GUI.Label(new Rect(5, 5, 90, 20), tooltipText);
                }, "");
            }
            
            // 恢复原始背景色
            GUI.backgroundColor = originalBackgroundColor;
        }
        
        private void DrawWindow(int windowID)
        {
            // 设置统一的字体大小
            GUI.skin.label.fontSize = 14;
            GUI.skin.button.fontSize = 14;
            GUI.skin.textField.fontSize = 14;
            GUI.skin.window.fontSize = 14;
            
            GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) });
            
            // 标题
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("成员修改器", new GUILayoutOption[] { GUILayout.ExpandWidth(false) });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            
            // 分类按钮
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            if (GUILayout.Button("族人修改", new GUILayoutOption[] { GUILayout.Width(150f), GUILayout.Height(30f) }))
            {
                currentMode = 0;
                hasSelectedCategory = true;
                // 清空属性值，确保显示与当前模式一致
                ResetAttributeValues();
                // 清空搜索文本框
                ClearSearchText();
                // 过滤显示族人员列表
                FilterMembers();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("门客修改", new GUILayoutOption[] { GUILayout.Width(150f), GUILayout.Height(30f) }))
            {
                currentMode = 1;
                hasSelectedCategory = true;
                // 清空属性值，确保显示与当前模式一致
                ResetAttributeValues();
                // 清空搜索文本框
                ClearSearchText();
                // 过滤显示门客列表
                FilterMembers();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            
            // 族人模式支项选择（只有选择族人模式时才显示）
                if (currentMode == 0)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
                    if (GUILayout.Button("族人家族", new GUILayoutOption[] { GUILayout.Width(150f), GUILayout.Height(30f) }))
                    {
                        currentMemberSubMode = 1;
                        hasSelectedCategory = true;
                        // 清空属性值，确保显示与当前子模式一致
                        ResetAttributeValues();
                        // 清空搜索文本框
                        ClearSearchText();
                        FilterMembers();
                    }
                    if (GUILayout.Button("族人妻妾", new GUILayoutOption[] { GUILayout.Width(150f), GUILayout.Height(30f) }))
                    {
                        currentMemberSubMode = 2;
                        hasSelectedCategory = true;
                        // 清空属性值，确保显示与当前子模式一致
                        ResetAttributeValues();
                        // 清空搜索文本框
                        ClearSearchText();
                        FilterMembers();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10f);
                }
            
            // 根据是否选择了分类显示不同内容
            if (hasSelectedCategory)
            {
                // 选择分类后显示：搜索框、成员列表、属性编辑区域、修改按钮
                
                // 搜索文本框和清空按钮
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
                GUILayout.Label("名字搜索:", new GUILayoutOption[] { GUILayout.Width(80f) });
                string newSearchText = GUILayout.TextField(searchText, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
                if (newSearchText != searchText)
                {
                    searchText = newSearchText;
                    FilterMembers();
                }
                if (GUILayout.Button("清空", new GUILayoutOption[] { GUILayout.Width(60f), GUILayout.Height(20f) }))
                {
                    ClearSearchText();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10f);
                
                // 搜索结果列表
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(150f) });
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) });
                
                foreach (string member in filteredMembers)
                {
                    if (GUILayout.Button(member, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(30f) }))
                    {
                        selectedMemberName = member;
                        LoadMemberData(member);
                    }
                }
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.Space(10f);
                
                // 属性编辑区域 - 根据选择的分类显示不同的属性
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
                Vector2 scrollPos = GUILayout.BeginScrollView(Vector2.zero, new GUILayoutOption[] { GUILayout.Height(750f) });
                
                // 根据不同分类显示对应的属性
                if (currentMode == 0) // 族人模式
                {
                    if (currentMemberSubMode == 1) // 族人家族
                    {
                        // 族人家族基本属性
                        DrawAttributeRow("声誉:", ref A_reputationOriginal, ref A_reputation);
                        DrawAttributeRow("体力:", ref A_powerOriginal, ref A_power);
                        DrawAttributeRow("年龄:", ref A_ageOriginal, ref A_age);
                        DrawAttributeRow("健康:", ref A_healthOriginal, ref A_health);
                        DrawAttributeRow("心情:", ref A_moodOriginal, ref A_mood);
                        DrawAttributeRow("魅力:", ref A_charmOriginal, ref A_charm);
                        DrawAttributeRow("幸运:", ref A_luckOriginal, ref A_luck);
                        DrawAttributeRow("品性:", ref A_characterOriginal, ref A_character);
                        DrawAttributeRow("天赋:", ref A_talentOriginal, ref A_talent);
                        DrawAttributeRow("天赋点:", ref A_talentPointOriginal, ref A_talentPoint);
                        DrawAttributeRow("技能:", ref A_skillOriginal, ref A_skill);
                        DrawAttributeRow("技能点:", ref A_skillPointOriginal, ref A_skillPoint);
                        DrawAttributeRow("喜好:", ref A_preferenceOriginal, ref A_preference);
                        DrawAttributeRow("文才:", ref A_intelligenceOriginal, ref A_intelligence);
                        DrawAttributeRow("武才:", ref A_weaponOriginal, ref A_weapon);
                        DrawAttributeRow("商才:", ref A_businessOriginal, ref A_business);
                        DrawAttributeRow("艺才:", ref A_artOriginal, ref A_art);
                        DrawAttributeRow("计谋:", ref A_strategyOriginal, ref A_strategy);
                        // 族人家族特有属性：爵位和封地
                        DrawAttributeRow("爵位:", ref A_JueWeiOriginal, ref A_JueWei);
                        DrawAttributeRow("封地:", ref A_FengDiOriginal, ref A_FengDi);
                    }
                    else if (currentMemberSubMode == 2) // 族人妻妾
                    {
                        // 族人妻妾基本属性
                        DrawAttributeRow("声誉:", ref B_reputationOriginal, ref B_reputation);
                        DrawAttributeRow("体力:", ref B_powerOriginal, ref B_power);
                        DrawAttributeRow("年龄:", ref B_ageOriginal, ref B_age);
                        DrawAttributeRow("健康:", ref B_healthOriginal, ref B_health);
                        DrawAttributeRow("心情:", ref B_moodOriginal, ref B_mood);
                        DrawAttributeRow("魅力:", ref B_charmOriginal, ref B_charm);
                        DrawAttributeRow("幸运:", ref B_luckOriginal, ref B_luck);
                        DrawAttributeRow("品性:", ref B_characterOriginal, ref B_character);
                        DrawAttributeRow("天赋:", ref B_talentOriginal, ref B_talent);
                        DrawAttributeRow("天赋点:", ref B_talentPointOriginal, ref B_talentPoint);
                        DrawAttributeRow("技能:", ref B_skillOriginal, ref B_skill);
                        DrawAttributeRow("技能点:", ref B_skillPointOriginal, ref B_skillPoint);
                        DrawAttributeRow("喜好:", ref B_preferenceOriginal, ref B_preference);
                        DrawAttributeRow("文才:", ref B_intelligenceOriginal, ref B_intelligence);
                        DrawAttributeRow("武才:", ref B_weaponOriginal, ref B_weapon);
                        DrawAttributeRow("商才:", ref B_businessOriginal, ref B_business);
                        DrawAttributeRow("艺才:", ref B_artOriginal, ref B_art);
                        DrawAttributeRow("计谋:", ref B_strategyOriginal, ref B_strategy);
                        
                        // 族人妻妾特有属性提示
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(100f);
                        GUILayout.Label("族人妻妾特有属性", new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
                        GUILayout.EndHorizontal();
                    }
                }
                else if (currentMode == 1) // 门客模式
                {
                    // 门客基本属性
                    DrawAttributeRow("声誉:", ref C_reputationOriginal, ref C_reputation);
                    DrawAttributeRow("体力:", ref C_powerOriginal, ref C_power);
                    DrawAttributeRow("年龄:", ref C_ageOriginal, ref C_age);
                    DrawAttributeRow("健康:", ref C_healthOriginal, ref C_health);
                    DrawAttributeRow("心情:", ref C_moodOriginal, ref C_mood);
                    DrawAttributeRow("魅力:", ref C_charmOriginal, ref C_charm);
                    DrawAttributeRow("幸运:", ref C_luckOriginal, ref C_luck);
                    DrawAttributeRow("品性:", ref C_characterOriginal, ref C_character);
                    DrawAttributeRow("天赋:", ref C_talentOriginal, ref C_talent);
                    DrawAttributeRow("天赋点:", ref C_talentPointOriginal, ref C_talentPoint);
                    DrawAttributeRow("技能:", ref C_skillOriginal, ref C_skill);
                    DrawAttributeRow("技能点:", ref C_skillPointOriginal, ref C_skillPoint);
                    DrawAttributeRow("文才:", ref C_intelligenceOriginal, ref C_intelligence);
                    DrawAttributeRow("武才:", ref C_weaponOriginal, ref C_weapon);
                    DrawAttributeRow("商才:", ref C_businessOriginal, ref C_business);
                    DrawAttributeRow("艺才:", ref C_artOriginal, ref C_art);
                    DrawAttributeRow("计谋:", ref C_strategyOriginal, ref C_strategy);
                    DrawAttributeRow("门客酬劳:", ref C_MenKe_RewardOriginal, ref C_MenKe_Reward);
                }
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.Space(10f);

                // 修改按钮
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("修改按钮", new GUILayoutOption[] { GUILayout.Width(200f), GUILayout.Height(50f) }))
                {
                    ApplyChanges();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10f);
            }
            else
            {
                // 未选择分类时显示：使用说明
                
                // 说明区域
                GUILayout.Space(20f);
                GUILayout.BeginVertical();
                
                // 创建大字体样式
                GUIStyle largeFontStyle = new GUIStyle(UnityEngine.GUI.skin.label);
                largeFontStyle.fontSize = largeFontStyle.fontSize * 2; // 设置字体大小为原来的2倍
                
                GUIStyle largeBoxStyle = new GUIStyle(UnityEngine.GUI.skin.box);
                largeBoxStyle.fontSize = largeBoxStyle.fontSize * 2; // 设置字体大小为原来的2倍
                
                // 使用说明标题
                GUILayout.Label("使用说明:", largeBoxStyle);
                
                // 使用说明
                GUILayout.Label("1. 请在点击修改前先保存游戏，以便回档", largeFontStyle);
                GUILayout.Label("2. 按F3键显示/隐藏窗口", largeFontStyle);
                GUILayout.Label("3. 选择修改类型：族人修改/门客修改", largeFontStyle);
                GUILayout.Label("4. 族人模式下可选择：族人家族/族人妻妾", largeFontStyle);
                GUILayout.Label("5. 输入部分字符可搜索角色", largeFontStyle);
                GUILayout.Label("6. 选择角色后可以通过点击按钮或者直接在文本框中输入来修改对应的属性值", largeFontStyle);
                GUILayout.Label("7. 点击修改按钮应用更改", largeFontStyle);
                GUILayout.Label("", largeFontStyle);
                
                // MOD作者及版本号说明
                GUILayout.Label("Mod作者：AnZhi20", largeFontStyle);
                GUILayout.Label("Mod版本：1.1.0", largeFontStyle);
                GUILayout.EndVertical();
            }
            
            GUILayout.EndVertical();
            
            // 允许拖动窗口
            GUI.DragWindow();
        }
        
        // 技能代码转中文对照表
        private string SkillCodeToChinese(string skillCode)
        {
            if (string.IsNullOrEmpty(skillCode))
                return "";
            
            string result = "";
            char[] codes = skillCode.ToCharArray();
            
            foreach (char code in codes)
            {
                switch (code)
                {
                    case '0': result += "空";
                        break;
                    case '1': result += "巫";
                        break;
                    case '2': result += "医";
                        break;
                    case '3': result += "相";
                        break;
                    case '4': result += "卜";
                        break;
                    case '5': result += "魅";
                        break;
                    case '6': result += "工";
                        break;
                    default: result += code;
                        break;
                }
            }
            
            return result;
        }
        
        // 天赋代码转中文对照表
        private string TalentCodeToChinese(string talentCode)
        {
            if (string.IsNullOrEmpty(talentCode))
                return "";
            
            string result = "";
            char[] codes = talentCode.ToCharArray();
            
            foreach (char code in codes)
            {
                switch (code)
                {
                    case '0': result += "空";
                        break;
                    case '1': result += "文";
                        break;
                    case '2': result += "武";
                        break;
                    case '3': result += "商";
                        break;
                    case '4': result += "艺";
                        break;
                    default: result += code;
                        break;
                }
            }
            
            return result;
        }
        
        // 喜好代码转中文对照表
        private string PreferenceCodeToChinese(string preferenceCode)
        {
            if (string.IsNullOrEmpty(preferenceCode))
                return "";
            
            string result = "";
            char[] codes = preferenceCode.ToCharArray();
            
            foreach (char code in codes)
            {
                switch (code)
                {
                    case '0': result += "香粉、";
                        break;
                    case '1': result += "书法、";
                        break;
                    case '2': result += "丹青、";
                        break;
                    case '3': result += "文玩、";
                        break;
                    case '4': result += "茶具、";
                        break;
                    case '5': result += "香具、";
                        break;
                    case '6': result += "瓷器、";
                        break;
                    case '7': result += "美酒、";
                        break;
                    case '8': result += "琴瑟、";
                        break;
                    case '9': result += "皮毛、";
                        break;
                    default: result += code + "、";
                        break;
                }
            }
            
            // 移除最后一个顿号
            if (result.EndsWith("、"))
            {
                result = result.Substring(0, result.Length - 1);
            }
            
            return result;
        }
        
        // 品性代码转中文对照表
        private string CharacterCodeToChinese(string characterCode)
        {
            if (string.IsNullOrEmpty(characterCode))
                return "";
            
            // 品性代码0-14对应中文
            switch (characterCode)
            {
                case "0": return "不明";
                case "1": return "傲娇";
                case "2": return "刚正";
                case "3": return "活泼";
                case "4": return "善良";
                case "5": return "真诚";
                case "6": return "洒脱";
                case "7": return "高冷";
                case "8": return "自卑";
                case "9": return "怯懦";
                case "10": return "腼腆";
                case "11": return "凶狠";
                case "12": return "善变";
                case "13": return "忧郁";
                case "14": return "多疑";
                default: return characterCode;
            }
        }
        
        // 爵位代码转中文对照表
        private string JueWeiCodeToChinese(string jueWeiCode)
        {
            if (string.IsNullOrEmpty(jueWeiCode))
                return "";
            
            // 爵位代码0-4对应中文
            switch (jueWeiCode)
            {
                case "0": return "空";
                case "1": return "伯爵";
                case "2": return "侯爵";
                case "3": return "公爵";
                case "4": return "王爵";
                default: return jueWeiCode;
            }
        }
        
        // 封地代码转中文对照表
        private string FengDiCodeToChinese(string fengDiCode)
        {
            if (string.IsNullOrEmpty(fengDiCode))
                return "";
            
            // 封地代码1-12对应中文
            switch (fengDiCode)
            {
                case "0": return "无封地";
                case "1": return "南郡-汉阳";
                case "2": return "三川郡-左亭";
                case "3": return "蜀郡-华阳";
                case "4": return "丹阳郡-宛陵";
                case "5": return "陈留郡-长罗";
                case "6": return "长沙郡-安成";
                case "7": return "会稽郡-太末";
                case "8": return "广陵郡-盐渎";
                case "9": return "太原郡-霍人";
                case "10": return "益州郡-比苏";
                case "11": return "南海郡-新会";
                case "12": return "云南郡-越隽";
                default: return fengDiCode;
            }
        }
        
        private void DrawAttributeRow(string label, ref string originalValue, ref string currentValue)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            GUILayout.Label(label, new GUILayoutOption[] { GUILayout.Width(100f) });
            
            // 特殊处理喜好显示和修改
                if (label == "喜好:")
                {
                    // 创建居中显示的样式
                    GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                    centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                    
                    // 第二列显示中文
                    GUILayout.Label(PreferenceCodeToChinese(originalValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                    
                    // 第二列和第三列之间添加至少4个字符缩进
                    GUILayout.Space(32f);
                    
                    // 第三列显示固定文本：→
                    GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                    
                    // 第三列和第四列之间添加5个字符缩进
                    GUILayout.Space(40f);
                    
                    // 第四列显示待修改值
                    GUILayout.Label(PreferenceCodeToChinese(currentValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                
                // 第四列和第五列之间添加至少4个字符缩进
                GUILayout.Space(32f);
                
                // 第五列改为按钮形式
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(350f) });
                
                // 第一行按钮 (0-4)
                GUILayout.BeginHorizontal();
                string[] preferenceOptions1 = new string[] { "0", "1", "2", "3", "4" };
                string[] preferenceLabels1 = new string[] { "香粉", "书法", "丹青", "文玩", "茶具" };
                
                for (int i = 0; i < preferenceOptions1.Length; i++)
                {
                    Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(preferenceLabels1[i]), GUI.skin.button, new GUILayoutOption[] { GUILayout.Width(60f), GUILayout.Height(20f) });
                    if (GUI.Button(buttonRect, preferenceLabels1[i]))
                    {
                        currentValue = preferenceOptions1[i];
                    }
                    
                    // 检查鼠标是否悬停在按钮上
                    if (buttonRect.Contains(Event.current.mousePosition))
                    {
                        // 设置工具提示变量
                        showTooltip = true;
                        tooltipText = PreferenceCodeToChinese(preferenceOptions1[i]);
                        tooltipPosition = Event.current.mousePosition;
                    }
                }
                GUILayout.EndHorizontal();
                
                // 第二行按钮 (5-9)
                GUILayout.BeginHorizontal();
                string[] preferenceOptions2 = new string[] { "5", "6", "7", "8", "9" };
                string[] preferenceLabels2 = new string[] { "香具", "瓷器", "美酒", "琴瑟", "皮毛" };
                
                for (int i = 0; i < preferenceOptions2.Length; i++)
                {
                    Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(preferenceLabels2[i]), GUI.skin.button, new GUILayoutOption[] { GUILayout.Width(60f), GUILayout.Height(20f) });
                    if (GUI.Button(buttonRect, preferenceLabels2[i]))
                    {
                        currentValue = preferenceOptions2[i];
                    }
                    
                    // 检查鼠标是否悬停在按钮上
                    if (buttonRect.Contains(Event.current.mousePosition))
                    {
                        // 设置工具提示变量
                        showTooltip = true;
                        tooltipText = PreferenceCodeToChinese(preferenceOptions2[i]);
                        tooltipPosition = Event.current.mousePosition;
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.EndVertical();
            }
            // 特殊处理爵位显示和修改
            else if (label == "爵位:")
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 第二列显示中文
                GUILayout.Label(JueWeiCodeToChinese(originalValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                
                // 第二列和第三列之间添加至少4个字符缩进
                GUILayout.Space(32f);
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                
                // 第三列和第四列之间添加5个字符缩进
                GUILayout.Space(40f);
                
                // 第四列显示待修改值
                GUILayout.Label(JueWeiCodeToChinese(currentValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                
                // 第四列和第五列之间添加至少4个字符缩进
                GUILayout.Space(32f);
                
                // 第五列改为按钮形式
                GUILayout.BeginHorizontal();
                string[] jueWeiOptions = new string[] { "0", "1", "2", "3", "4" };
                string[] jueWeiLabels = new string[] { "空", "伯爵", "侯爵", "公爵", "王爵" };
                
                for (int i = 0; i < jueWeiOptions.Length; i++)
                {
                    if (GUILayout.Button(jueWeiLabels[i], new GUILayoutOption[] { GUILayout.Width(60f), GUILayout.Height(20f) }))
                    {
                        currentValue = jueWeiOptions[i];
                    }
                }
                GUILayout.EndHorizontal();
                
            }
            // 特殊处理封地显示和修改
            else if (label == "封地:")
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 第二列显示中文
                GUILayout.Label(FengDiCodeToChinese(originalValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                
                // 第二列和第三列之间添加至少4个字符缩进
                GUILayout.Space(32f);
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                
                // 第三列和第四列之间添加5个字符缩进
                GUILayout.Space(40f);
                
                // 第四列显示待修改值
                GUILayout.Label(FengDiCodeToChinese(currentValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                
                // 第五列改为按钮形式
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(380f) });
                
                // 空按钮单独成行
                GUILayout.BeginHorizontal();
                Rect emptyButtonRect = GUILayoutUtility.GetRect(new GUIContent("空"), GUI.skin.button, new GUILayoutOption[] { GUILayout.Width(60f), GUILayout.Height(20f) });
                if (GUI.Button(emptyButtonRect, "空"))
                {
                    currentValue = "0";
                }
                
                // 检查鼠标是否悬停在空按钮上
                if (emptyButtonRect.Contains(Event.current.mousePosition))
                {
                    // 设置工具提示变量
                    showTooltip = true;
                    tooltipText = FengDiCodeToChinese("0");
                    tooltipPosition = Event.current.mousePosition;
                }
                GUILayout.EndHorizontal();
                
                // 第一行按钮 (1-6)
                GUILayout.BeginHorizontal();
                string[] fengDiOptions1 = new string[] { "1", "2", "3", "4", "5", "6" };
                string[] fengDiLabels1 = new string[] { "汉阳", "左亭", "华阳", "宛陵", "长罗", "安成" };
                
                for (int i = 0; i < fengDiOptions1.Length; i++)
                {
                    Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(fengDiLabels1[i]), GUI.skin.button, new GUILayoutOption[] { GUILayout.Width(60f), GUILayout.Height(20f) });
                    if (GUI.Button(buttonRect, fengDiLabels1[i]))
                    {
                        currentValue = fengDiOptions1[i];
                    }
                    
                    // 检查鼠标是否悬停在按钮上
                    if (buttonRect.Contains(Event.current.mousePosition))
                    {
                        // 设置工具提示变量
                        showTooltip = true;
                        tooltipText = FengDiCodeToChinese(fengDiOptions1[i]);
                        tooltipPosition = Event.current.mousePosition;
                    }
                }
                GUILayout.EndHorizontal();
                
                // 第二行按钮 (7-12)
                GUILayout.BeginHorizontal();
                string[] fengDiOptions2 = new string[] { "7", "8", "9", "10", "11", "12" };
                string[] fengDiLabels2 = new string[] { "太末", "盐渎", "霍人", "比苏", "新会", "越隽" };
                
                for (int i = 0; i < fengDiOptions2.Length; i++)
                {
                    Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(fengDiLabels2[i]), GUI.skin.button, new GUILayoutOption[] { GUILayout.Width(60f), GUILayout.Height(20f) });
                    if (GUI.Button(buttonRect, fengDiLabels2[i]))
                    {
                        currentValue = fengDiOptions2[i];
                    }
                    
                    // 检查鼠标是否悬停在按钮上
                    if (buttonRect.Contains(Event.current.mousePosition))
                    {
                        // 设置工具提示变量
                        showTooltip = true;
                        tooltipText = FengDiCodeToChinese(fengDiOptions2[i]);
                        tooltipPosition = Event.current.mousePosition;
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.EndVertical();
            }
            // 特殊处理品性显示
            else if (label == "品性:")
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 第二列显示中文
                GUILayout.Label(CharacterCodeToChinese(originalValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                
                // 第二列和第三列之间添加至少4个字符缩进
                GUILayout.Space(32f);
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                
                // 第三列和第四列之间添加5个字符缩进
                GUILayout.Space(40f);
                
                // 第四列显示待修改值
                GUILayout.Label(CharacterCodeToChinese(currentValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                
                // 第五列改为按钮形式（分两行显示，每行7个）
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(300f) });
                
                // 第一行按钮 (1-7)
                GUILayout.BeginHorizontal();
                string[] characterOptions1 = new string[] { "1", "2", "3", "4", "5", "6", "7" };
                string[] characterLabels1 = new string[] { "傲娇", "刚正", "活泼", "善良", "真诚", "洒脱", "高冷" };
                
                for (int i = 0; i < characterOptions1.Length; i++)
                {
                    if (GUILayout.Button(characterLabels1[i], new GUILayoutOption[] { GUILayout.Width(40f), GUILayout.Height(20f) }))
                    {
                        currentValue = characterOptions1[i];
                        // 直接更新对应的模式特定变量
                        if (label == "品性:")
                        {
                            if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                            {
                                A_character = characterOptions1[i];
                            }
                            else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                            {
                                B_character = characterOptions1[i];
                            }
                            else if (currentMode == 1) // 门客模式
                            {
                                C_character = characterOptions1[i];
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
                
                // 第二行按钮 (8-14)
                GUILayout.BeginHorizontal();
                string[] characterOptions2 = new string[] { "8", "9", "10", "11", "12", "13", "14" };
                string[] characterLabels2 = new string[] { "自卑", "怯懦", "腼腆", "凶狠", "善变", "忧郁", "多疑" };
                
                for (int i = 0; i < characterOptions2.Length; i++)
                {
                    if (GUILayout.Button(characterLabels2[i], new GUILayoutOption[] { GUILayout.Width(40f), GUILayout.Height(20f) }))
                    {
                        currentValue = characterOptions2[i];
                        // 直接更新对应的模式特定变量
                        if (label == "品性:")
                        {
                            if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                            {
                                A_character = characterOptions2[i];
                            }
                            else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                            {
                                B_character = characterOptions2[i];
                            }
                            else if (currentMode == 1) // 门客模式
                            {
                                C_character = characterOptions2[i];
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.EndVertical();
            }
            // 特殊处理技能显示
            else if (label == "技能:")
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 第二列显示中文
                GUILayout.Label(SkillCodeToChinese(originalValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                
                // 第二列和第三列之间添加至少4个字符缩进
                GUILayout.Space(32f);
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                
                // 第三列和第四列之间添加5个字符缩进
                GUILayout.Space(40f);
                
                // 第四列显示待修改值
                GUILayout.Label(SkillCodeToChinese(currentValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                
                // 第五列改为按钮形式
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(200f) });
                string[] skillOptions = new string[] { "0", "1", "2", "3", "4", "5", "6" };
                string[] skillLabels = new string[] { "空", "巫", "医", "相", "卜", "魅", "工" };
                
                for (int i = 0; i < skillOptions.Length; i++)
                {
                    if (GUILayout.Button(skillLabels[i], new GUILayoutOption[] { GUILayout.Width(30f), GUILayout.Height(25f) }))
                    {
                        // 直接赋值选中的技能值
                        currentValue = skillOptions[i];
                        
                        // 直接更新对应的模式特定变量
                        if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                        {
                            A_skill = skillOptions[i];
                        }
                        else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                        {
                            B_skill = skillOptions[i];
                        }
                        else if (currentMode == 1) // 门客模式
                        {
                            C_skill = skillOptions[i];
                        }
                        
                        // 如果技能为空，重置技能点为0
                        if (string.IsNullOrEmpty(currentValue) || currentValue == "0")
                        {
                            if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                            {
                                A_skillPoint = "0";
                                A_skillPointOriginal = "0";
                            }
                            else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                            {
                                B_skillPoint = "0";
                                B_skillPointOriginal = "0";
                            }
                            else if (currentMode == 1) // 门客模式
                            {
                                C_skillPoint = "0";
                                C_skillPointOriginal = "0";
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            // 特殊处理技能点显示
            else if (label == "技能点:")
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 第二列显示原始值
                GUILayout.Label(originalValue, centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                
                // 第二列和第三列之间添加至少4个字符缩进
                GUILayout.Space(32f);
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                
                // 第三列和第四列之间添加5个字符缩进
                GUILayout.Space(40f);
                
                // 第四列显示待修改值
                GUILayout.Label(currentValue, centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                
                // 第五列改为输入框
                // 如果技能为空或选择空，技能点默认为0且不可修改
                bool isSkillEmpty = false;
                if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                {
                    isSkillEmpty = string.IsNullOrEmpty(A_skill) || A_skill == "0";
                }
                else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                {
                    isSkillEmpty = string.IsNullOrEmpty(B_skill) || B_skill == "0";
                }
                else if (currentMode == 1) // 门客模式
                {
                    isSkillEmpty = string.IsNullOrEmpty(C_skill) || C_skill == "0";
                }
                
                if (isSkillEmpty)
                {
                    GUI.enabled = false;
                    GUILayout.TextField("0", new GUILayoutOption[] { GUILayout.Width(200f) });
                    GUI.enabled = true;
                }
                else
                {
                    string newValue = GUILayout.TextField(currentValue, new GUILayoutOption[] { GUILayout.Width(200f) });
                    if (newValue != currentValue)
                    {
                        currentValue = newValue;
                        // 更新对应的模式特定变量
                        if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                        {
                            A_skillPoint = newValue;
                        }
                        else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                        {
                            B_skillPoint = newValue;
                        }
                        else if (currentMode == 1) // 门客模式
                        {
                            C_skillPoint = newValue;
                        }
                    }
                }
            }
            // 特殊处理天赋显示
            else if (label == "天赋:")
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 第二列显示中文
                GUILayout.Label(TalentCodeToChinese(originalValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                GUILayout.Space(32f); // 第二列和第三列之间至少4个字符缩进
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                GUILayout.Space(40f); // 第三列和第四列之间添加5个字符缩进
                
                // 第四列显示待修改值
                GUILayout.Label(TalentCodeToChinese(currentValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                GUILayout.Space(32f); // 第四列和第五列之间至少4个字符缩进
                
                // 第五列改为按钮形式
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(200f) });
                string[] talentOptions = new string[] { "0", "1", "2", "3", "4" };
                string[] talentLabels = new string[] { "空", "文", "武", "商", "艺" };
                
                for (int i = 0; i < talentOptions.Length; i++)
                {
                    if (GUILayout.Button(talentLabels[i], new GUILayoutOption[] { GUILayout.Width(40f), GUILayout.Height(25f) }))
                    {
                        // 直接赋值选中的天赋值
                        currentValue = talentOptions[i];
                        
                        // 直接更新对应的模式特定变量
                        if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                        {
                            A_talent = talentOptions[i];
                        }
                        else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                        {
                            B_talent = talentOptions[i];
                        }
                        else if (currentMode == 1) // 门客模式
                        {
                            C_talent = talentOptions[i];
                        }
                        
                        // 如果天赋为空，重置天赋点为0
                        if (string.IsNullOrEmpty(currentValue) || currentValue == "0")
                        {
                            if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                            {
                                A_talentPoint = "0";
                                A_talentPointOriginal = "0";
                            }
                            else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                            {
                                B_talentPoint = "0";
                                B_talentPointOriginal = "0";
                            }
                            else if (currentMode == 1) // 门客模式
                            {
                                C_talentPoint = "0";
                                C_talentPointOriginal = "0";
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            // 特殊处理天赋点显示
            else if (label == "天赋点:")
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 第二列显示原始值
                GUILayout.Label(originalValue, centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                GUILayout.Space(32f); // 第二列和第三列之间至少4个字符缩进
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                GUILayout.Space(40f); // 第三列和第四列之间添加5个字符缩进
                
                // 第四列显示待修改值
                GUILayout.Label(currentValue, centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                GUILayout.Space(32f); // 第四列和第五列之间至少4个字符缩进
                
                // 第五列改为输入框
                // 如果天赋为空或选择空，天赋点默认为0且不可修改
                bool isTalentEmpty = false;
                if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                {
                    isTalentEmpty = string.IsNullOrEmpty(A_talent) || A_talent == "0";
                }
                else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                {
                    isTalentEmpty = string.IsNullOrEmpty(B_talent) || B_talent == "0";
                }
                else if (currentMode == 1) // 门客模式
                {
                    isTalentEmpty = string.IsNullOrEmpty(C_talent) || C_talent == "0";
                }
                
                if (isTalentEmpty)
                {
                    GUI.enabled = false;
                    GUILayout.TextField("0", new GUILayoutOption[] { GUILayout.Width(200f) });
                    GUI.enabled = true;
                }
                else
                {
                    string newValue = GUILayout.TextField(currentValue, new GUILayoutOption[] { GUILayout.Width(200f) });
                    if (newValue != currentValue)
                    {
                        currentValue = newValue;
                        // 更新对应的模式特定变量
                        if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                        {
                            A_talentPoint = newValue;
                        }
                        else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                        {
                            B_talentPoint = newValue;
                        }
                        else if (currentMode == 1) // 门客模式
                        {
                            C_talentPoint = newValue;
                        }
                    }
                }
            }
            // 特殊处理喜好点显示
            else if (label == "喜好点:")
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 第二列显示原始值
                GUILayout.Label(originalValue, centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                GUILayout.Space(32f); // 第二列和第三列之间至少4个字符缩进
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                GUILayout.Space(40f); // 第三列和第四列之间添加5个字符缩进
                
                // 第四列显示待修改值
                GUILayout.Label(currentValue, centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                GUILayout.Space(32f); // 第四列和第五列之间至少4个字符缩进
                
                // 第五列改为输入框
                // 如果喜好为空或选择空，喜好点默认为0且不可修改
                if (currentMode == 0) // 只在族人模式下处理喜好点
                {
                    bool isPreferenceEmpty = false;
                    if (currentMemberSubMode == 1) // 族人家族模式
                    {
                        isPreferenceEmpty = string.IsNullOrEmpty(A_preference);
                    }
                    else if (currentMemberSubMode == 2) // 族人妻妾模式
                    {
                        isPreferenceEmpty = string.IsNullOrEmpty(B_preference);
                    }
                    
                    if (isPreferenceEmpty)
                    {
                        GUI.enabled = false;
                        GUILayout.TextField("0", new GUILayoutOption[] { GUILayout.Width(200f) });
                        GUI.enabled = true;
                    }
                }
                else
                {
                    string newValue = GUILayout.TextField(currentValue, new GUILayoutOption[] { GUILayout.Width(200f) });
                    if (newValue != currentValue)
                    {
                        currentValue = newValue;
                        // 更新对应的模式特定变量
                        if (currentMode == 0)
                        {
                            if (currentMemberSubMode == 1) // 族人家族模式
                            {
                                A_preference = newValue;
                            }
                            else if (currentMemberSubMode == 2) // 族人妻妾模式
                            {
                                B_preference = newValue;
                            }
                        }
                    }
                }
            }
            // 特殊处理喜好显示
            else if (label == "喜好:")
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 第二列显示中文
                GUILayout.Label(PreferenceCodeToChinese(originalValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                GUILayout.Space(32f); // 第二列和第三列之间至少4个字符缩进
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                GUILayout.Space(40f); // 第三列和第四列之间添加5个字符缩进
                
                // 第四列显示待修改值
                GUILayout.Label(PreferenceCodeToChinese(currentValue), centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                
                // 第五列改为按钮形式（分两行显示）
                GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(200f) });
                
                // 第一行按钮
                GUILayout.BeginHorizontal();
                string[] preferenceOptions1 = new string[] { "0", "1", "2", "3", "4" };
                string[] preferenceLabels1 = new string[] { "香粉", "书法", "丹青", "文玩", "茶具" };
                
                for (int i = 0; i < preferenceOptions1.Length; i++)
                {
                    if (GUILayout.Button(preferenceLabels1[i], new GUILayoutOption[] { GUILayout.Width(35f), GUILayout.Height(20f) }))
                    {
                        if (string.IsNullOrEmpty(currentValue) || !currentValue.Contains(preferenceOptions1[i]))
                        {
                            currentValue += preferenceOptions1[i];
                        }
                        else
                        {
                            // 移除已存在的喜好
                            currentValue = currentValue.Replace(preferenceOptions1[i], "");
                        }
                        
                        // 直接更新对应的模式特定变量
                        if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                        {
                            A_preference = currentValue;
                        }
                        else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                        {
                            B_preference = currentValue;
                        }
                    }
                }
                GUILayout.EndHorizontal();
                
                // 第二行按钮
                GUILayout.BeginHorizontal();
                string[] preferenceOptions2 = new string[] { "5", "6", "7", "8", "9" };
                string[] preferenceLabels2 = new string[] { "香具", "瓷器", "美酒", "琴瑟", "皮毛" };
                
                for (int i = 0; i < preferenceOptions2.Length; i++)
                {
                    if (GUILayout.Button(preferenceLabels2[i], new GUILayoutOption[] { GUILayout.Width(35f), GUILayout.Height(20f) }))
                    {
                        if (string.IsNullOrEmpty(currentValue) || !currentValue.Contains(preferenceOptions2[i]))
                        {
                            currentValue += preferenceOptions2[i];
                        }
                        else
                        {
                            // 移除已存在的喜好
                            currentValue = currentValue.Replace(preferenceOptions2[i], "");
                        }
                        
                        // 直接更新对应的模式特定变量
                        if (currentMode == 0 && currentMemberSubMode == 1) // 族人家族模式
                        {
                            A_preference = currentValue;
                        }
                        else if (currentMode == 0 && currentMemberSubMode == 2) // 族人妻妾模式
                        {
                            B_preference = currentValue;
                        }
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.EndVertical();
            }
            else
            {
                // 创建居中显示的样式
                GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.label);
                centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
                
                // 其他属性修改为新的布局方式
                GUILayout.Label(originalValue, centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(60f) }); // 第二列显示宽度改为6个字符，并居中显示
                GUILayout.Space(32f); // 第二列和第三列之间至少4个字符缩进
                
                // 第三列显示固定文本：→
                GUILayout.Label("→", new GUILayoutOption[] { GUILayout.Width(20f) });
                GUILayout.Space(40f); // 第三列和第四列之间改为5个字符缩进
                
                // 第四列显示待修改值
                GUILayout.Label(currentValue, centeredLabelStyle, new GUILayoutOption[] { GUILayout.Width(80f) }); // 第四列显示宽度改为8个字符，并居中显示
                GUILayout.Space(32f); // 第四列和第五列之间至少4个字符缩进
                
                // 第五列改为输入框
                string newValue = GUILayout.TextField(currentValue, new GUILayoutOption[] { GUILayout.Width(200f) });
                if (newValue != currentValue)
                {
                    currentValue = newValue;
                    
                    // 根据当前模式和标签更新对应的模式特定变量
                    if (currentMode == 0)
                    {
                        if (currentMemberSubMode == 1) // 族人家族模式
                        {
                            if (label == "幸运:") A_luck = currentValue;
                            else if (label == "天赋:") A_talent = currentValue;
                            else if (label == "技能:") A_skill = currentValue;
                            else if (label == "喜好:") A_preference = currentValue;
                            else if (label == "品性:") A_character = currentValue;
                            else if (label == "天赋点:") A_talentPoint = currentValue;
                            else if (label == "技能点:") A_skillPoint = currentValue;
                        }
                        else if (currentMemberSubMode == 2) // 族人妻妾模式
                        {
                            if (label == "幸运:") B_luck = currentValue;
                            else if (label == "天赋:") B_talent = currentValue;
                            else if (label == "技能:") B_skill = currentValue;
                            else if (label == "喜好:") B_preference = currentValue;
                            else if (label == "品性:") B_character = currentValue;
                            else if (label == "天赋点:") B_talentPoint = currentValue;
                            else if (label == "技能点:") B_skillPoint = currentValue;
                        }
                    }
                    else if (currentMode == 1) // 门客模式
                    {
                        if (label == "幸运:") C_luck = currentValue;
                        else if (label == "天赋:") C_talent = currentValue;
                        else if (label == "技能:") C_skill = currentValue;
                        else if (label == "品性:") C_character = currentValue;
                        else if (label == "天赋点:") C_talentPoint = currentValue;
                        else if (label == "技能点:") C_skillPoint = currentValue;
                    }
                }
            }
            
            GUILayout.EndHorizontal();
        }
        
        private void FilterMembers()
        {
            try
            {
                filteredMembers.Clear();
                
                if (currentMode == 0) // 族人模式
                {
                    // 只有当选择了具体的子模式（族人家族或族人妻妾）时才显示列表
                    if (currentMemberSubMode == 0)
                    {
                        return; // 不显示任何列表
                    }
                    
                    // 检查数据是否可用
                    bool hasMemberNowData = Mainload.Member_now != null && Mainload.Member_now.Count > 0;
                    bool hasMemberQuData = Mainload.Member_qu != null && Mainload.Member_qu.Count > 0;
                    
                    if (!hasMemberNowData && !hasMemberQuData)
                    {
                        Logger.LogWarning("没有找到任何族人数据");
                        return;
                    }
                    
                    // 搜索族人家族
                    if (currentMemberSubMode == 1)
                    {
                        SearchMemberData(Mainload.Member_now, 4);
                    }
                    
                    // 搜索族人妻妾
                    if (currentMemberSubMode == 2)
                    {
                        SearchMemberData(Mainload.Member_qu, 2); 
                    }
                }
                else if (currentMode == 1) // 门客模式
                {
                    SearchMemberData(Mainload.MenKe_Now, 2);
                }
                
                // 按名称排序
                filteredMembers.Sort();
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("筛选成员时出错: {0}", ex.Message));
                filteredMembers.Clear();
            }
        }
        
        // 通用的成员搜索方法，提高代码复用性
        private void SearchMemberData(List<List<string>> dataList, int nameFieldIndex)
        {
            if (dataList == null || dataList.Count == 0)
                return;
            
            foreach (var memberData in dataList)
            {
                try
                {
                    if (memberData != null && memberData.Count > nameFieldIndex)
                    {
                        string nameField = memberData[nameFieldIndex];
                        if (!string.IsNullOrEmpty(nameField))
                        {
                            string[] memberInfo = nameField.Split('|');
                            if (memberInfo.Length > 0 && !string.IsNullOrEmpty(memberInfo[0]))
                            {
                                AddMemberIfMatches(memberInfo[0]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(string.Format("处理成员数据时出错: {0}", ex.Message));
                    // 继续处理下一个成员，不中断搜索
                }
            }
        }
        
        private void AddMemberIfMatches(string name)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                filteredMembers.Add(name);
            }
            else
            {
                // 不区分大小写的模糊搜索
                if (name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    filteredMembers.Add(name);
                }
            }
        }
        
        // 清空搜索文本
        private void ClearSearchText()
        {
            searchText = "";
            FilterMembers();
        }

        private void ResetAttributeValues()
        {
            // 重置族人家族属性
            A_reputation = "0";
            A_reputationOriginal = "0";
            A_title = "0";
            A_titleOriginal = "0";
            A_power = "0";
            A_powerOriginal = "0";
            A_age = "0";
            A_ageOriginal = "0";
            A_health = "0";
            A_healthOriginal = "0";
            A_mood = "0";
            A_moodOriginal = "0";
            A_charm = "0";
            A_charmOriginal = "0";
            A_luck = "0";
            A_luckOriginal = "0";
            A_preference = "0";
            A_preferenceOriginal = "0";
            A_character = "0";
            A_characterOriginal = "0";
            A_talent = "0";
            A_talentOriginal = "0";
            A_talentPoint = "0";
            A_talentPointOriginal = "0";
            A_skill = "0";
            A_skillOriginal = "0";
            A_skillPoint = "0";
            A_skillPointOriginal = "0";
            A_JueWei = "0";
            A_JueWeiOriginal = "0";
            A_FengDi = "0";
            A_FengDiOriginal = "0";
            
            // 重置族人妻妾属性
            B_reputation = "0";
            B_reputationOriginal = "0";
            B_power = "0";
            B_powerOriginal = "0";
            B_age = "0";
            B_ageOriginal = "0";
            B_health = "0";
            B_healthOriginal = "0";
            B_mood = "0";
            B_moodOriginal = "0";
            B_charm = "0";
            B_charmOriginal = "0";
            B_luck = "0";
            B_luckOriginal = "0";
            B_preference = "0";
            B_preferenceOriginal = "0";
            B_character = "0";
            B_characterOriginal = "0";
            B_talent = "0";
            B_talentOriginal = "0";
            B_talentPoint = "0";
            B_talentPointOriginal = "0";
            B_skill = "0";
            B_skillOriginal = "0";
            B_skillPoint = "0";
            B_skillPointOriginal = "0";
            
            // 重置门客属性
            C_reputation = "0";
            C_reputationOriginal = "0";
            C_power = "0";
            C_powerOriginal = "0";
            C_age = "0";
            C_ageOriginal = "0";
            C_health = "0";
            C_healthOriginal = "0";
            C_mood = "0";
            C_moodOriginal = "0";
            C_charm = "0";
            C_charmOriginal = "0";
            C_luck = "0";
            C_luckOriginal = "0";
            C_character = "0";
            C_characterOriginal = "0";
            C_talent = "0";
            C_talentOriginal = "0";
            C_talentPoint = "0";
            C_talentPointOriginal = "0";
            C_skill = "0";
            C_skillOriginal = "0";
            C_skillPoint = "0";
            C_skillPointOriginal = "0";
            C_intelligence = "0";
            C_intelligenceOriginal = "0";
            C_weapon = "0";
            C_weaponOriginal = "0";
            C_business = "0";
            C_businessOriginal = "0";
            C_art = "0";
            C_artOriginal = "0";
            C_strategy = "0";
            C_strategyOriginal = "0";
            C_MenKe_Reward = "0";
            C_MenKe_RewardOriginal = "0";
            
            // 重置选择状态
            selectedMemberName = "";
            currentDataIndex = -1;
            
            // 重置数据索引变量
            Member_nowData = null;
            Member_quData = null;
            Member_MenKeData = null;
        }
        
        // 为不同模式定义独立的数据索引变量
        private List<string> Member_nowData = null; // 族人家族数据
        private List<string> Member_quData = null; // 族人妻妾数据
        private List<string> Member_MenKeData = null; // 门客数据
        private int currentDataIndex = -1; // 当前选中成员的索引

        private void LoadMemberData(string memberName)
        {
            try
            {
                if (currentMode == 0) // 族人模式
                {
                    // 根据子模式选择正确的数据结构
                    bool found = false;
                    currentDataIndex = -1;
                        
                    if (currentMemberSubMode == 0 || currentMemberSubMode == 1)
                    {
                        if (Mainload.Member_now != null)
                        {
                            for (int i = 0; i < Mainload.Member_now.Count; i++)
                            {
                                Member_nowData = Mainload.Member_now[i];
                                string[] memberInfo = Member_nowData[4].Split('|'); 
                                string name = memberInfo[0];
                                     
                                if (name == memberName)
                                {
                                    // 加载族人家族成员数据
                                    A_reputation = Member_nowData[16]; // 声誉
                                    A_reputationOriginal = A_reputation; // 保存原始值
                                    A_power = Member_nowData[30]; // 体力
                                    A_powerOriginal = A_power; // 保存原始值
                                    A_age = Member_nowData[6]; // 年龄
                                    A_ageOriginal = A_age; // 保存原始值
                                    A_health = Member_nowData[21]; // 健康
                                    A_healthOriginal = A_health; // 保存原始值
                                    A_mood = Member_nowData[11]; // 心情
                                    A_moodOriginal = A_mood; // 保存原始值
                                    A_charm = Member_nowData[20]; // 魅力
                                    A_charmOriginal = A_charm; // 保存原始值
                                    A_luck = memberInfo.Length > 7 ? memberInfo[7] : "0"; // 幸运
                                    A_luckOriginal = A_luck; // 保存原始值
                                    A_character = Member_nowData[5]; // 品性
                                    A_characterOriginal = A_character; // 保存原始值
                                    A_talent = memberInfo.Length > 2 ? memberInfo[2] : "0"; // 天赋
                                    A_talentOriginal = A_talent; // 保存原始值
                                    A_talentPoint = memberInfo.Length > 3 ? memberInfo[3] : "0"; // 天赋点
                                    A_talentPointOriginal = A_talentPoint; // 保存原始值
                                    A_skill = memberInfo.Length > 6 ? memberInfo[6] : "0"; // 技能
                                    A_skillOriginal = A_skill; // 保存原始值
                                    A_skillPoint = Member_nowData[33]; // 技能点
                                    A_skillPointOriginal = A_skillPoint; // 保存原始值
                                    A_preference = memberInfo.Length > 9 ? memberInfo[9] : "0"; // 喜好
                                    A_preferenceOriginal = A_preference; // 保存原始值
                                    A_intelligence = Member_nowData[7]; // 文才
                                    A_intelligenceOriginal = A_intelligence; // 保存原始值
                                    A_weapon = Member_nowData[8]; // 武才
                                    A_weaponOriginal = A_weapon; // 保存原始值
                                    A_business = Member_nowData[9]; // 商才
                                    A_businessOriginal = A_business; // 保存原始值
                                    A_art = Member_nowData[10]; // 艺才
                                    A_artOriginal = A_art; // 保存原始值
                                    A_strategy = Member_nowData[27]; // 计谋
                                    A_strategyOriginal = A_strategy; // 保存原始值
                                    string[] jueWeiFengDi = Member_nowData[14].Split('|');
                                    A_JueWei = jueWeiFengDi.Length > 0 ? jueWeiFengDi[0] : "0"; // 爵位
                                    A_JueWeiOriginal = A_JueWei; // 保存原始值
                                    A_FengDi = jueWeiFengDi.Length > 1 ? jueWeiFengDi[1] : "0"; // 封地
                                    A_FengDiOriginal = A_FengDi; // 保存原始值
                                    
                                    found = true;
                                    currentDataIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                        
                    // 加载族人妻妾数据
                    if (!found && currentMode == 0 && currentMemberSubMode == 2)
                    {
                        if (Mainload.Member_qu != null)
                        {
                            for (int i = 0; i < Mainload.Member_qu.Count; i++)
                            {
                                Member_quData = Mainload.Member_qu[i];
                                string[] memberInfo = Member_quData[2].Split('|'); 
                                string name = memberInfo[0];
                                    
                                if (name == memberName)
                                {
                                    // 加载族人妻妾成员数据
                                    B_reputation = Member_quData[12]; // 声誉
                                    B_reputationOriginal = B_reputation; // 保存原始值
                                    B_power = Member_quData[20]; // 体力
                                    B_powerOriginal = B_power; // 保存原始值
                                    B_age = Member_quData[5]; // 年龄
                                    B_ageOriginal = B_age; // 保存原始值
                                    B_health = Member_quData[16]; // 健康
                                    B_healthOriginal = B_health; // 保存原始值
                                    B_mood = Member_quData[10]; // 心情
                                    B_moodOriginal = B_mood; // 保存原始值
                                    B_charm = Member_quData[15]; // 魅力
                                    B_charmOriginal = B_charm; // 保存原始值
                                    B_luck = memberInfo.Length > 7 ? memberInfo[7] : "0"; // 幸运
                                    B_luckOriginal = B_luck; // 保存原始值
                                    B_character = memberInfo.Length > 8 ? memberInfo[8] : "0"; // 品性
                                    B_characterOriginal = B_character; // 保存原始值
                                    B_preference = memberInfo.Length > 10 ? memberInfo[10] : "0"; // 喜好
                                    B_preferenceOriginal = B_preference; // 保存原始值
                                    B_talent = memberInfo.Length > 2 ? memberInfo[2] : "0"; // 天赋
                                    B_talentOriginal = B_talent; // 保存原始值
                                    B_talentPoint = memberInfo.Length > 3 ? memberInfo[3] : "0"; // 天赋点
                                    B_talentPointOriginal = B_talentPoint; // 保存原始值
                                    B_skill = memberInfo.Length > 6 ? memberInfo[6] : "0"; // 技能
                                    B_skillOriginal = B_skill; // 保存原始值
                                    B_skillPoint = Member_quData[23]; // 技能点
                                    B_skillPointOriginal = B_skillPoint; // 保存原始值
                                    B_intelligence = Member_quData[6]; // 文才
                                    B_intelligenceOriginal = B_intelligence; // 保存原始值
                                    B_weapon = Member_quData[7]; // 武才
                                    B_weaponOriginal = B_weapon; // 保存原始值
                                    B_business = Member_quData[8]; // 商才
                                    B_businessOriginal = B_business; // 保存原始值
                                    B_art = Member_quData[9]; // 艺才
                                    B_artOriginal = B_art; // 保存原始值
                                    B_strategy = Member_quData[19]; // 计谋
                                    B_strategyOriginal = B_strategy; // 保存原始值
                                        
                                    found = true;
                                    currentDataIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (currentMode == 1) // 门客模式
                {
                    // 现有的门客模式加载逻辑保持不变
                    if (Mainload.MenKe_Now != null)
                    {
                        for (int i = 0; i < Mainload.MenKe_Now.Count; i++)
                        {
                            Member_MenKeData = Mainload.MenKe_Now[i];
                            string[] menkeInfo = Member_MenKeData[2].Split('|'); // 名字
                            string name = menkeInfo[0];
                                
                            if (name == memberName)
                                {
                                    C_reputation = Member_MenKeData[11]; // 声誉
                                    C_reputationOriginal = C_reputation; // 保存原始值
                                    C_power = Member_MenKeData[19]; // 体力
                                    C_powerOriginal = C_power; // 保存原始值
                                    C_age = Member_MenKeData[3]; // 年龄
                                    C_ageOriginal = C_age; // 保存原始值
                                    C_health = Member_MenKeData[14]; // 健康
                                    C_healthOriginal = C_health; // 保存原始值
                                    C_mood = Member_MenKeData[8]; // 心情
                                    C_moodOriginal = C_mood; // 保存原始值
                                    C_charm = Member_MenKeData[13]; // 魅力
                                    C_charmOriginal = C_charm; // 保存原始值
                                    string[] menkeDataArray = Member_MenKeData[2].Split('|');
                                    C_luck = menkeDataArray.Length > 7 ? menkeDataArray[7] : "0"; // 幸运
                                    C_luckOriginal = C_luck; // 保存原始值
                                    C_character = menkeDataArray.Length > 8 ? menkeDataArray[8] : "0"; // 品性
                                    C_characterOriginal = C_character; // 保存原始值
                                    C_talent = menkeInfo.Length > 2 ? menkeInfo[2] : "0"; // 天赋
                                    C_talentOriginal = C_talent; // 保存原始值
                                    C_talentPoint = menkeInfo.Length > 3 ? menkeInfo[3] : "0"; // 天赋点
                                    C_talentPointOriginal = C_talentPoint; // 保存原始值
                                    C_skill = menkeInfo.Length > 6 ? menkeInfo[6] : "0"; // 技能
                                    C_skillOriginal = C_skill; // 保存原始值
                                    C_skillPoint = Member_MenKeData[16]; // 技能点
                                    C_skillPointOriginal = C_skillPoint; // 保存原始值
                                    C_intelligence = Member_MenKeData[4]; // 文才
                                    C_intelligenceOriginal = C_intelligence; // 保存原始值
                                    C_weapon = Member_MenKeData[5]; // 武才
                                    C_weaponOriginal = C_weapon; // 保存原始值
                                    C_business = Member_MenKeData[6]; // 商才
                                    C_businessOriginal = C_business; // 保存原始值
                                    C_art = Member_MenKeData[7]; // 艺才
                                    C_artOriginal = C_art; // 保存原始值
                                    C_strategy = Member_MenKeData[15]; // 计谋
                                    C_strategyOriginal = C_strategy; // 保存原始值
                                    C_MenKe_Reward = Member_MenKeData[18]; // 门客工资
                                    C_MenKe_RewardOriginal = C_MenKe_Reward; // 保存原始值
                                    
                                currentDataIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("加载{0}数据时出错: {1}", memberName, ex.Message));
            }
        }
        
        private void ApplyChanges()
        {
            try
            {
                if (string.IsNullOrEmpty(selectedMemberName) || currentDataIndex < 0)
                    return;

                // 根据当前模式执行对应的修改方法
                switch (currentMode)
                {
                    case 0: // 族人模式
                        ApplyClanMemberChanges();
                        break;
                    case 1: // 门客模式
                        ApplyMenKeModeChanges();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("应用{0}属性修改时出错: {1}", selectedMemberName, ex.Message));
            }
        }

        // 族人模式属性修改主方法
        private void ApplyClanMemberChanges()
        {
            // 根据子模式执行对应的修改
            switch (currentMemberSubMode)
            {
                case 1: // 族人家族模式
                    if (Mainload.Member_now != null && currentDataIndex < Mainload.Member_now.Count)
                    {
                        // 检查族人家族模式下的天赋和技能有效性
                        bool isA_TalentEmpty = string.IsNullOrEmpty(A_talent) || A_talent == "0";
                        bool hasA_TalentPoints = !string.IsNullOrEmpty(A_talentPoint) && A_talentPoint != "0";
                        
                        bool isA_SkillEmpty = string.IsNullOrEmpty(A_skill) || A_skill == "0";
                        bool hasA_SkillPoints = !string.IsNullOrEmpty(A_skillPoint) && A_skillPoint != "0";
                        
                        // 有效性检查
                        if (isA_TalentEmpty && hasA_TalentPoints)
                        {
                            ShowErrorMessage("天赋为空，天赋点不为零");
                            return;
                        }
                        
                        if (isA_SkillEmpty && hasA_SkillPoints)
                        {
                            ShowErrorMessage("技能为空，技能点不为零");
                            return;
                        }
                        
                        // 应用族人家族模式修改
                        ApplyClanFamilyChanges();
                    }
                    break;
                case 2: // 族人妻妾模式
                    if (Mainload.Member_qu != null && currentDataIndex < Mainload.Member_qu.Count)
                    {
                        // 检查族人妻妾模式下的天赋和技能有效性
                        bool isB_TalentEmpty = string.IsNullOrEmpty(B_talent) || B_talent == "0";
                        bool hasB_TalentPoints = !string.IsNullOrEmpty(B_talentPoint) && B_talentPoint != "0";
                        
                        bool isB_SkillEmpty = string.IsNullOrEmpty(B_skill) || B_skill == "0";
                        bool hasB_SkillPoints = !string.IsNullOrEmpty(B_skillPoint) && B_skillPoint != "0";
                        
                        // 有效性检查
                        if (isB_TalentEmpty && hasB_TalentPoints)
                        {
                            ShowErrorMessage("天赋为空，天赋点不为零");
                            return;
                        }
                        
                        if (isB_SkillEmpty && hasB_SkillPoints)
                        {
                            ShowErrorMessage("技能为空，技能点不为零");
                            return;
                        }
                        
                        // 应用族人妻妾模式修改
                        ApplyClanSpouseChanges();
                    }
                    break;
            }
        }

        // 门客模式属性修改主方法
        private void ApplyMenKeModeChanges()
        {
            if (Mainload.MenKe_Now != null && !string.IsNullOrEmpty(selectedMemberName) && currentDataIndex < Mainload.MenKe_Now.Count)
            {
                // 检查门客模式下的天赋和技能有效性
                bool isC_TalentEmpty = string.IsNullOrEmpty(C_talent) || C_talent == "0";
                bool hasC_TalentPoints = !string.IsNullOrEmpty(C_talentPoint) && C_talentPoint != "0";
                
                bool isC_SkillEmpty = string.IsNullOrEmpty(C_skill) || C_skill == "0";
                bool hasC_SkillPoints = !string.IsNullOrEmpty(C_skillPoint) && C_skillPoint != "0";
                
                // 有效性检查
                if (isC_TalentEmpty && hasC_TalentPoints)
                {
                    ShowErrorMessage("天赋为空，天赋点不为零");
                    return;
                }
                
                if (isC_SkillEmpty && hasC_SkillPoints)
                {
                    ShowErrorMessage("技能为空，技能点不为零");
                    return;
                }
                
                // 应用门客模式修改
                ApplyMenKeChanges();
            }
        }

        // 显示错误信息的辅助方法
        private void ShowErrorMessage(string errorReason)
        {
            Logger.LogWarning(string.Format("{0}的{1}，已阻止修改", selectedMemberName, errorReason));
            
            // 添加提示信息
            if (Mainload.Tip_Show != null)
            {
                Mainload.Tip_Show.Add(new List<string>
                {
                    "1",
                    string.Format("【{0}】的{1}，修改失败", selectedMemberName, errorReason)
                });
            }
        }

        // 应用族人家族成员属性修改
        private void ApplyClanFamilyChanges()
        {
            // 使用族人家族数据索引变量
            Member_nowData = Mainload.Member_now[currentDataIndex];
            string[] memberInfo = Member_nowData[4].Split('|');

            try
            {
                // 基本属性修改
                UpdateClanFamilyBasicAttributes(Member_nowData);

                // 扩展属性修改
                UpdateClanFamilyExtendedAttributes(Member_nowData);

                // 嵌套属性修改
                memberInfo = UpdateClanFamilyNestedAttributes(memberInfo);

                // 更新嵌套数据
                Member_nowData[4] = string.Join("|", memberInfo);

                // 处理爵位和封地
                HandleClanFamilyTitleAndLand(Member_nowData);

                // 检查并解锁封地
                bool fengDiUnlockedSuccessfully = UnlockFengDiIfNeeded();

                // 如果封地解锁失败，阻止修改并显示提示
                if (!fengDiUnlockedSuccessfully)
                {
                    return;
                }

                // 修改成功，添加提示信息
                ShowSuccessMessage("族人家族");
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("应用族人家族{0}的属性修改时出错: {1}", selectedMemberName, ex.Message));
            }
        }

        // 更新族人家族基本属性
        private void UpdateClanFamilyBasicAttributes(List<string> memberData)
        {
            memberData[5] = A_character;    // 品性
            memberData[6] = A_age;          // 年龄
            memberData[7] = A_intelligence; // 文才
            memberData[8] = A_weapon;       // 武才
            memberData[9] = A_business;     // 商才
            memberData[10] = A_art;         // 艺才
            memberData[11] = A_mood;        // 心情
            memberData[16] = A_reputation;  // 声誉
            memberData[20] = A_charm;       // 魅力
            memberData[21] = A_health;      // 健康
            memberData[27] = A_strategy;    // 计谋
            memberData[30] = A_power;       // 体力
        }

        // 更新族人家族扩展属性
        private void UpdateClanFamilyExtendedAttributes(List<string> memberData)
        {
            memberData[33] = A_skillPoint;// 技能点
        }

        // 更新族人家族嵌套属性
        private string[] UpdateClanFamilyNestedAttributes(string[] memberInfo)
        {
            // 确保数组长度足够存储所有属性，不足时扩展数组
            int requiredLength = 10; // 至少需要索引9
            string[] updatedMemberInfo = memberInfo;
            
            if (memberInfo.Length < requiredLength)
            {
                updatedMemberInfo = new string[requiredLength];
                memberInfo.CopyTo(updatedMemberInfo, 0);
                // 初始化新增元素为默认值
                for (int i = memberInfo.Length; i < requiredLength; i++)
                {
                    updatedMemberInfo[i] = "0";
                }
            }
            
            // 确保所有属性都被正确赋值
            updatedMemberInfo[2] = A_talent ?? "0";           // 天赋
            updatedMemberInfo[3] = A_talentPoint ?? "0";      // 天赋点
            updatedMemberInfo[6] = A_skill ?? "0";            // 技能
            updatedMemberInfo[7] = A_luck ?? "0";             // 幸运 
            updatedMemberInfo[9] = A_preference ?? "0";       // 喜好
            
            return updatedMemberInfo;
        }

        // 处理族人家族爵位和封地
        private void HandleClanFamilyTitleAndLand(List<string> memberData)
        {
            memberData[14] = string.IsNullOrEmpty(A_FengDi) ? A_JueWei : A_JueWei + "|" + A_FengDi;
        }

        // 显示修改成功消息
        private void ShowSuccessMessage(string memberType)
        {
            if (Mainload.Tip_Show != null)
            {
                Mainload.Tip_Show.Add(new List<string>
                {
                    "1",
                    string.Format("【{0}】属性修改成功", selectedMemberName)
                });

                // 如果是族人家族模式且有封地解锁，添加封地解锁成功提示
                if (memberType == "族人家族" && !string.IsNullOrEmpty(A_FengDi) && A_FengDi != "0")
                {
                    string fengDiName = FengDiCodeToChinese(A_FengDi);
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        string.Format("封地【{0}】解锁成功", fengDiName)
                    });
                }
            }

            Logger.LogInfo(string.Format("成功应用{0}{1}的属性修改", memberType, selectedMemberName));
        }

        // 应用族人妻妾属性修改
        private void ApplyClanSpouseChanges()
        {
            try
            {
                // 检查Mainload.Member_qu集合是否有效
                if (Mainload.Member_qu != null && currentDataIndex >= 0 && currentDataIndex < Mainload.Member_qu.Count)
                {
                    // 获取当前妻妾数据的副本
                    Member_quData = new List<string>(Mainload.Member_qu[currentDataIndex]);
                    string[] memberInfo = Member_quData[2].Split('|');

                    // 基本属性修改
                    UpdateClanSpouseBasicAttributes(Member_quData);

                    // 扩展属性修改
                    UpdateClanSpouseExtendedAttributes(Member_quData);

                    // 嵌套属性修改
                    memberInfo = UpdateClanSpouseNestedAttributes(memberInfo);

                    // 更新嵌套数据
                    Member_quData[2] = string.Join("|", memberInfo);

                    // 创建新的List<string>对象来保存修改后的数据
                    List<string> updatedData = new List<string>(Member_quData);
                    Mainload.Member_qu[currentDataIndex] = updatedData;

                    // 修改成功，添加提示信息
                    if (Mainload.Tip_Show != null)
                    {
                        Mainload.Tip_Show.Add(new List<string>
                        {
                            "1",
                            string.Format("【{0}】属性修改成功", selectedMemberName)
                        });
                    }

                    Logger.LogInfo(string.Format("成功应用族人妻妾{0}的属性修改", selectedMemberName));
                }
                else
                {
                    Logger.LogError("Mainload.Member_qu集合无效或索引超出范围");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("应用族人妻妾{0}的属性修改时出错: {1}", selectedMemberName, ex.Message));
            }
        }

        // 更新族人妻妾基本属性
        private void UpdateClanSpouseBasicAttributes(List<string> memberData)
        {
            memberData[5] = B_age;          // 年龄
            memberData[6] = B_intelligence; // 文才
            memberData[7] = B_weapon;       // 武才
            memberData[8] = B_business;     // 商才
            memberData[9] = B_art;          // 艺才
            memberData[10] = B_mood;        // 心情
            memberData[12] = B_reputation;  // 声誉
            memberData[15] = B_charm;       // 魅力
            memberData[16] = B_health;      // 健康
            memberData[19] = B_strategy;    // 计谋
            memberData[20] = B_power;       // 体力
        }

        // 更新族人妻妾扩展属性
        private void UpdateClanSpouseExtendedAttributes(List<string> memberData)
        {
            memberData[23] = B_skillPoint;  // 技能点
        }

        // 更新族人妻妾嵌套属性
        private string[] UpdateClanSpouseNestedAttributes(string[] memberInfo)
        {
            // 确保数组长度足够存储所有属性，不足时扩展数组
            int requiredLength = 11; 
            string[] updatedMemberInfo = memberInfo;
            
            if (memberInfo.Length < requiredLength)
            {
                updatedMemberInfo = new string[requiredLength];
                memberInfo.CopyTo(updatedMemberInfo, 0);
                // 初始化新增元素为默认值
                for (int i = memberInfo.Length; i < requiredLength; i++)
                {
                    updatedMemberInfo[i] = "0";
                }
            }
            
            // 确保所有属性都被正确赋值
            updatedMemberInfo[2] = B_talent ?? "0";     // 天赋
            updatedMemberInfo[3] = B_talentPoint ?? "0";// 天赋点
            updatedMemberInfo[6] = B_skill ?? "0";      // 技能
            updatedMemberInfo[7] = B_luck ?? "0";       // 幸运
            updatedMemberInfo[8] = B_character ?? "0";  // 品性
            updatedMemberInfo[10] = B_preference ?? "0"; // 喜好
            
            return updatedMemberInfo;
        }

        // 应用门客属性修改
        private void ApplyMenKeChanges()
        {
            // 使用门客数据索引变量
            Member_MenKeData = Mainload.MenKe_Now[currentDataIndex];
            string[] menkeDataArray = Member_MenKeData[2].Split('|');

            try
            {
                // 基本属性修改
                UpdateMenKeBasicAttributes(Member_MenKeData);

                // 扩展属性修改
                UpdateMenKeExtendedAttributes(Member_MenKeData);

                // 嵌套属性修改
                menkeDataArray = UpdateMenKeNestedAttributes(menkeDataArray);

                // 更新嵌套数据
                Member_MenKeData[2] = string.Join("|", menkeDataArray);

                // 修改成功，添加提示信息
                if (Mainload.Tip_Show != null)
                {
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        string.Format("【{0}】属性修改成功", selectedMemberName)
                    });
                }

                Logger.LogInfo(string.Format("成功应用门客{0}的属性修改", selectedMemberName));
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("应用门客{0}的属性修改时出错: {1}", selectedMemberName, ex.Message));
            }
        }

        // 更新门客基本属性
        private void UpdateMenKeBasicAttributes(List<string> memberData)
        {
            memberData[11] = C_reputation;  // 声誉
            memberData[19] = C_power;       // 体力
            memberData[3] = C_age;          // 年龄
            memberData[14] = C_health;      // 健康
            memberData[8] = C_mood;         // 心情
            memberData[13] = C_charm;       // 魅力
            memberData[16] = C_skillPoint;  // 技能点
            memberData[18] = C_MenKe_Reward;// 门客工资
        }

        // 更新门客扩展属性
        private void UpdateMenKeExtendedAttributes(List<string> memberData)
        {
            memberData[4] = C_intelligence; // 文才
            memberData[5] = C_weapon;       // 武才
            memberData[6] = C_business;     // 商才
            memberData[7] = C_art;          // 艺才
            memberData[15] = C_strategy;    // 计谋
        }

        // 更新门客嵌套属性
        private string[] UpdateMenKeNestedAttributes(string[] memberInfo)
        {
            // 确保数组长度足够存储所有属性，不足时扩展数组
            int requiredLength = 9; // 至少需要索引8
            string[] updatedMemberInfo = memberInfo;
            
            if (memberInfo.Length < requiredLength)
            {
                updatedMemberInfo = new string[requiredLength];
                memberInfo.CopyTo(updatedMemberInfo, 0);
                // 初始化新增元素为默认值
                for (int i = memberInfo.Length; i < requiredLength; i++)
                {
                    updatedMemberInfo[i] = "0";
                }
            }
            
            // 确保所有属性都被正确赋值
            updatedMemberInfo[2] = C_talent ?? "0";     // 天赋
            updatedMemberInfo[3] = C_talentPoint ?? "0";// 天赋点
            updatedMemberInfo[6] = C_skill ?? "0";      // 技能
            updatedMemberInfo[7] = C_luck ?? "0";       // 幸运
            updatedMemberInfo[8] = C_character ?? "0";  // 品性
            
            return updatedMemberInfo;
        }

        // 检查并解锁封地
        private bool UnlockFengDiIfNeeded()
        {
            bool fengDiUnlockedSuccessfully = true;
            
            if (!string.IsNullOrEmpty(A_FengDi) && A_FengDi != "0")
            {
                try
                {
                    int fengDiIndex = int.Parse(A_FengDi);
                    string fengDiName = FengDiCodeToChinese(A_FengDi);
                    // 确保索引在有效范围内 (1-12)
                    if (fengDiIndex >= 1 && fengDiIndex <= 12 && Mainload.Fengdi_now != null && Mainload.Fengdi_now.Count > fengDiIndex)
                    {
                        List<string> fengDiData = Mainload.Fengdi_now[fengDiIndex];
                        if (fengDiData != null && fengDiData.Count > 0)
                        {
                            // 检查封地是否已解锁 (第一个元素为0表示未解锁)
                            if (fengDiData[0] == "0")
                            {
                                // 尝试解锁封地
                                fengDiData[0] = "1";
                                // 验证解锁是否成功
                                if (fengDiData[0] != "1")
                                {
                                    fengDiUnlockedSuccessfully = false;
                                    Logger.LogError(string.Format("解锁封地失败: {0}", fengDiName));
                                }
                                else
                                {
                                    Logger.LogInfo(string.Format("已解锁封地: {0}", fengDiName));
                                }
                            }
                            else
                            {
                                // 封地已解锁，直接跳出解锁逻辑
                                Logger.LogInfo(string.Format("封地{0}已解锁，无需再次解锁", fengDiName));
                            }
                        }
                        else
                        {
                            fengDiUnlockedSuccessfully = false;
                            Logger.LogError(string.Format("封地数据无效: {0}", fengDiName));
                        }
                    }
                    else
                    {
                        fengDiUnlockedSuccessfully = false;
                        Logger.LogError(string.Format("无效的封地索引: {0}", fengDiIndex));
                    }
                }
                catch (Exception ex)
                {
                    fengDiUnlockedSuccessfully = false;
                    Logger.LogError(string.Format("解锁封地时出错: {0}", ex.Message));
                }
            }

            // 如果封地解锁失败，阻止修改并显示提示
            if (!fengDiUnlockedSuccessfully)
            {
                // 添加提示信息
                string fengDiName = FengDiCodeToChinese(A_FengDi);
                if (Mainload.Tip_Show != null)
                {
                    Mainload.Tip_Show.Add(new List<string>
                    {
                        "1",
                        string.Format("{0}封地解锁失败，修改失败", fengDiName)
                    });
                }
                Logger.LogError(string.Format("{0}封地解锁失败，已阻止属性修改", fengDiName));
            }

            return fengDiUnlockedSuccessfully;
        }

        // 辅助方法：应用族员属性修改（此方法现在已被整合到ApplyChanges中）
        private bool ApplyClanMemberChanges(List<List<string>> memberDataList)
        {
            // 此方法已被整合到ApplyChanges中，保留仅用于兼容性
            return false;
        }
    }
}
