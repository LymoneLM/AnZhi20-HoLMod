using UnityEngine;
using UnityEditor;

public class WindowMain : EditorWindow
{
    private int selectedTab = 0;
    private string[] tabs = { "窗口A", "窗口B", "窗口C", "窗口D", "窗口E" };
    private WindowA windowA;
    private WindowB windowB;
    private WindowC windowC;
    private WindowD windowD;
    private WindowE windowE;

    [MenuItem("HolMod/主窗口")]
    public static void ShowWindow()
    {
        GetWindow<WindowMain>(false, "HolMod 工具", true);
    }

    private void OnEnable()
    {
        // 初始化各个窗口的实例
        windowA = new WindowA();
        windowB = new WindowB();
        windowC = new WindowC();
        windowD = new WindowD();
        windowE = new WindowE();
    }

    private void OnGUI()
    {
        // 绘制标签栏
        selectedTab = GUILayout.Toolbar(selectedTab, tabs, GUILayout.Height(30));

        // 绘制分割线
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

        // 根据选中的标签显示对应的窗口内容
        switch (selectedTab)
        {
            case 0:
                windowA.OnGUI();
                break;
            case 1:
                windowB.OnGUI();
                break;
            case 2:
                windowC.OnGUI();
                break;
            case 3:
                windowD.OnGUI();
                break;
            case 4:
                windowE.OnGUI();
                break;
        }
    }

    private void Update()
    {
        // 更新各个窗口的逻辑
        windowA?.Update();
        windowB?.Update();
        windowC?.Update();
        windowD?.Update();
        windowE?.Update();
    }
}