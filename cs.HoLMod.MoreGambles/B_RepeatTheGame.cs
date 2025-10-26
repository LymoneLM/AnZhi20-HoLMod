using System;
using UnityEngine;
using BepInEx.Logging;
using cs.HoLMod.TaskCheat; // 引入LanguageManager所在命名空间

namespace cs.HoLMod.MoreGambles
{
    /// <summary>
    /// 重复游戏逻辑管理类
    /// </summary>
    public static class B_RepeatTheGame
    {
        private static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("B_RepeatTheGame");
        
        /// <summary>
        /// 游戏结束处理逻辑
        /// </summary>
        /// <param name="remainingMoney">剩余货币数量</param>
        /// <param name="gameInstance">游戏实例</param>
        /// <param name="resetGameAction">重置游戏的委托</param>
        /// <param name="returnToMainMenuAction">返回主菜单的委托</param>
        /// <returns>是否继续游戏</returns>
        public static bool HandleGameEnd(int remainingMoney, MonoBehaviour gameInstance, Action resetGameAction, Action returnToMainMenuAction)
        {
            try
            {
                // 检查剩余货币是否为0
                if (remainingMoney <= 0)
                {
                    // 弹出借贷窗口（暂时不实现，默认为不借贷继续运行）
                Logger.LogInfo(LanguageManager.GetText("RemainingMoneyZeroLoanWindow"));
                
                // 模拟借贷后再次检查（默认为不借贷）
                // 由于借贷功能暂未实现，剩余货币仍为0，直接返回主窗口
                Logger.LogInfo(LanguageManager.GetText("LoanFunctionNotImplemented"));
                    returnToMainMenuAction?.Invoke();
                    return false;
                }
                else
                {
                    // 显示重复游戏选择窗口
                    return ShowRepeatGameDialog(gameInstance, resetGameAction, returnToMainMenuAction);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("处理游戏结束逻辑时出错: " + ex.Message);
                returnToMainMenuAction?.Invoke();
                return false;
            }
        }
        
        /// <summary>
        /// 显示重复游戏选择窗口
        /// </summary>
        /// <param name="gameInstance">游戏实例</param>
        /// <param name="resetGameAction">重置游戏的委托</param>
        /// <param name="returnToMainMenuAction">返回主菜单的委托</param>
        /// <returns>是否继续游戏</returns>
        private static bool ShowRepeatGameDialog(MonoBehaviour gameInstance, Action resetGameAction, Action returnToMainMenuAction)
        {
            try
            {
                // 这里应该显示一个对话框让玩家选择是否重复游戏
                // 由于Unity的GUI需要在OnGUI方法中处理，我们创建一个临时的UI处理对象
                RepeatGameDialog dialog = gameInstance.gameObject.AddComponent<RepeatGameDialog>();
                dialog.Initialize(resetGameAction, returnToMainMenuAction);
                return true; // 返回true表示正在处理中
            }
            catch (Exception ex)
            {
                Logger.LogError("显示重复游戏对话框时出错: " + ex.Message);
                returnToMainMenuAction?.Invoke();
                return false;
            }
        }
        
        /// <summary>
        /// 重复游戏对话框临时组件
        /// </summary>
        private class RepeatGameDialog : MonoBehaviour
        {
            private Action resetGameAction;
            private Action returnToMainMenuAction;
            private Rect windowRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200);
            private bool isVisible = true;
            private float scaleFactor = 1f;
            
            /// <summary>
            /// 初始化对话框
            /// </summary>
            /// <param name="resetAction">重置游戏的委托</param>
            /// <param name="returnAction">返回主菜单的委托</param>
            public void Initialize(Action resetAction, Action returnAction)
            {
                this.resetGameAction = resetAction;
                this.returnToMainMenuAction = returnAction;
                UpdateResolutionSettings();
            }
            
            private void UpdateResolutionSettings()
            {
                // 根据屏幕分辨率调整UI比例
                float screenWidth = Screen.width;
                if (screenWidth < 1024)
                {
                    scaleFactor = 0.8f;
                }
                else if (screenWidth < 1440)
                {
                    scaleFactor = 1.0f;
                }
                else
                {
                    scaleFactor = 1.2f;
                }
                
                // 调整窗口大小和位置
                windowRect.width = 300f * scaleFactor;
                windowRect.height = 200f * scaleFactor;
                windowRect.x = Screen.width / 2 - windowRect.width / 2;
                windowRect.y = Screen.height / 2 - windowRect.height / 2;
            }
            
            private void OnGUI()
            {
                if (!isVisible) return;
                
                GUI.skin.box.fontSize = Mathf.RoundToInt(14 * scaleFactor);
                GUI.skin.button.fontSize = Mathf.RoundToInt(14 * scaleFactor);
                GUI.skin.label.fontSize = Mathf.RoundToInt(14 * scaleFactor);
                
                windowRect = GUI.Window(0, windowRect, DrawDialogWindow, LanguageManager.GetText("GameOver"), GUI.skin.window);
            }
            
            private void DrawDialogWindow(int windowID)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(20f * scaleFactor);
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(LanguageManager.GetText("PlayAgainQuestion"), GUILayout.ExpandWidth(false));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(30f * scaleFactor);
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button(LanguageManager.GetText("PlayAgain"), GUILayout.Width(120f * scaleFactor), GUILayout.Height(40f * scaleFactor)))
                {
                    // 玩家选择再玩一局
                    isVisible = false;
                    resetGameAction?.Invoke();
                    Destroy(this);
                }
                
                GUILayout.Space(20f * scaleFactor);
                
                if (GUILayout.Button(LanguageManager.GetText("ReturnToMainMenu"), GUILayout.Width(120f * scaleFactor), GUILayout.Height(40f * scaleFactor)))
                {
                    // 玩家选择返回主窗口
                    isVisible = false;
                    returnToMainMenuAction?.Invoke();
                    Destroy(this);
                }
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                
                // 允许拖动窗口
                GUI.DragWindow(new Rect(0, 0, windowRect.width, 25));
            }
        }
    }
}
