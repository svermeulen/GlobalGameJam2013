using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour
{
    public Texture2D healthBarBackground;
    public Vector2 pos;
    public float minScale;
    public float maxScale;

    public float scale;

    private void OnGUI()
    {
        var width = scale * healthBarBackground.width;
        var height = scale * healthBarBackground.height;

        var center = new Vector2(Screen.width - pos.x, Screen.height - pos.y);

        // Draw background of health bar
        GUI.DrawTexture(new Rect(center.x - width / 2.0f, center.y - height / 2.0f, width, height), healthBarBackground);
    }

    void Update()
    {
        
    }
}
