using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace cs.HoLMod.Tool
{
    public class WindowD
    {
        // GUI绘制方法
        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            
            // 第一列 - 可以放置按钮或其他UI元素
            GUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("窗口D - 第一列");
            
            if (GUILayout.Button("刷新数据"))
            {
                // 刷新数据的逻辑
            }
            
            // 这里可以添加更多UI元素
            GUILayout.Space(20);
            GUILayout.Label("窗口D的第一列内容");
            
            GUILayout.EndVertical();
            
            // 垂直分割线
            GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true));
            
            // 第二列 - 可以放置详细信息或其他内容
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("窗口D - 第二列");
            
            GUILayout.FlexibleSpace();
            GUILayout.Label("窗口D的第二列内容", EditorStyles.centeredGreyMiniLabel);
            GUILayout.FlexibleSpace();
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        // 更新方法
        public void Update()
        {
            // 窗口D的更新逻辑
        }
    }
}
