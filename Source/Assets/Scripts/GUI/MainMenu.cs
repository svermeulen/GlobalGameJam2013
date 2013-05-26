using System;
using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GUIStyle headerStyle;
    public GUIStyle titleStyle;
    public Texture2D titleImage;
    public GUIStyle labelGuiStyle = new GUIStyle();
    public Vector2 labelPos;
    public Vector2 labelPos2;
    public float labelGap;
    public Vector2 dropdownPos;
    public float comboGap;
    public float comboHeight = 40;

    public Vector2 levelSelectPos;
    public Vector2 levelSelectDimension;

    public Vector2 comboDimensions;

    public Vector2 startButtonPos;
    public Vector2 startButtonDimensions;

    private ComboBox comboBoxControl1 = new ComboBox();
    private ComboBox comboBoxControl2 = new ComboBox();
    private ComboBox comboBoxControl3 = new ComboBox();

    private GUIStyle listStyle = new GUIStyle();
    private GUIStyle buttonStyle = new GUIStyle();

    public float comboItemHeight;
    public float comboVerticalOffset;

    public float comboItemHeight2;
    public float comboVerticalOffset2;

    GUIContent[] comboBoxList;
    GUIContent[] comboBoxList2;

    private int selectedItemIndex1 = 0;
    private int selectedItemIndex2 = 0;
    private int selectedItemIndex3 = 0;

    void Awake()
    {
#if UNITY_WEBPLAYER
        comboBoxList = new GUIContent[2];
        comboBoxList[0] = new GUIContent("Keyboard Arrows");
        comboBoxList[1] = new GUIContent("Keyboard WASD");
#else
        comboBoxList = new GUIContent[6];
        comboBoxList[0] = new GUIContent("Controller 1 Left Joystick");
        comboBoxList[1] = new GUIContent("Controller 1 Right Joystick");
        comboBoxList[2] = new GUIContent("Controller 2 Left Joystick");
        comboBoxList[3] = new GUIContent("Controller 2 Right Joystick");
        comboBoxList[4] = new GUIContent("Keyboard Arrows");
        comboBoxList[5] = new GUIContent("Keyboard WASD");
#endif

        comboBoxList2 = new GUIContent[6];
        comboBoxList2[0] = new GUIContent("1");
        comboBoxList2[1] = new GUIContent("2");
        comboBoxList2[2] = new GUIContent("3");
        comboBoxList2[3] = new GUIContent("4");
        comboBoxList2[4] = new GUIContent("5");
        comboBoxList2[5] = new GUIContent("6");

    	listStyle.normal.textColor = Color.white; 
    	listStyle.onHover.background =
    	listStyle.hover.background = new Texture2D(2, 2);
    	listStyle.padding.left =
    	listStyle.padding.right =
    	listStyle.padding.top =
    	listStyle.padding.bottom = 4;
        listStyle.alignment = TextAnchor.MiddleCenter;

        comboBoxControl1.selectedItemIndex = (int)GameConfig.GetControlType(0);
        comboBoxControl2.selectedItemIndex = (int)GameConfig.GetControlType(1);
        comboBoxControl3.selectedItemIndex = 0;
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), titleImage);

        GUI.Label(new Rect(labelPos.x, labelPos.y, 200, 20), "Player 1 Controls", labelGuiStyle);
        GUI.Label(new Rect(labelPos.x + labelGap, labelPos.y, 200, 20), "Player 2 Controls", labelGuiStyle);

        GUI.Label(new Rect(labelPos2.x, labelPos2.y, 200, 20), "Level", labelGuiStyle);

        if (GUI.Button(new Rect(startButtonPos.x, startButtonPos.y, startButtonDimensions.x, startButtonDimensions.y), "Start!"))
        {
            StartLevel();
        }

        comboBoxControl1.itemHeight = comboItemHeight;
        comboBoxControl2.itemHeight = comboItemHeight;

        comboBoxControl1.verticalOffset = comboVerticalOffset;
        comboBoxControl2.verticalOffset = comboVerticalOffset;

        comboBoxControl3.itemHeight = comboItemHeight2;
        comboBoxControl3.verticalOffset = comboVerticalOffset2;

        selectedItemIndex1 = comboBoxControl1.GetSelectedItemIndex();

        selectedItemIndex1 = comboBoxControl1.List(
            new Rect(dropdownPos.x, dropdownPos.y, comboDimensions.x, comboDimensions.y), comboBoxList[selectedItemIndex1].text, comboBoxList, listStyle);

        selectedItemIndex2 = comboBoxControl2.GetSelectedItemIndex();

        selectedItemIndex2 = comboBoxControl2.List(
            new Rect(dropdownPos.x + comboGap, dropdownPos.y, comboDimensions.x, comboDimensions.y), comboBoxList[selectedItemIndex2].text, comboBoxList, listStyle);

        selectedItemIndex3 = comboBoxControl3.GetSelectedItemIndex();

        selectedItemIndex3 = comboBoxControl3.List(
            new Rect(levelSelectPos.x, levelSelectPos.y, levelSelectDimension.x, levelSelectDimension.y), comboBoxList2[selectedItemIndex3].text, comboBoxList2, listStyle);
    }

    private void StartLevel()
    {
#if UNITY_WEBPLAYER
        GameConfig.SetControlType(0, selectedItemIndex1 == 0 ? ControlType.KeyboardArrows : ControlType.KeyboardWasd);
        GameConfig.SetControlType(1, selectedItemIndex1 == 0 ? ControlType.KeyboardArrows : ControlType.KeyboardWasd);
#else
        GameConfig.SetControlType(0, (ControlType)selectedItemIndex1);
        GameConfig.SetControlType(1, (ControlType)selectedItemIndex2);
#endif

        LevelMgr.Instance.LoadLevel(selectedItemIndex3);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            Application.Quit();
        }
    }
}
