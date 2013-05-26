using UnityEngine;

public class Debugging
{
    public static Debugging _instance = null;

    public static Debugging Instance
    {
        get
        {
            _instance = new Debugging();
            return _instance;
        }
    }

    public void ShowText(string text)
    {
        var dbgObj = GameObject.Find("DebugText");
        if (dbgObj != null)
        {
            dbgObj.guiText.text = text;   
        }
    }
}