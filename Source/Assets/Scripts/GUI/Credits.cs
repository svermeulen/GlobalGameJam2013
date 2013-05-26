using System;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public GUIStyle headerStyle;
    public GUIStyle titleStyle;
    public Texture2D titleImage;

    void Start()
    {
        CameraFade.StartAlphaFade(Color.black, false, 2f, 10f, () => { Application.LoadLevel("MainMenu"); });
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), titleImage);

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();

                    CreditsGui();

                    GUILayout.FlexibleSpace();
                }

                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    private void CreditsGui()
    {
        var buttonWidth = 300;
        var buttonHeight = 40;

        GUILayout.BeginVertical();
        {
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
//                GUILayout.Label("YOU WIN", titleStyle);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(60);

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

//                if (GUILayout.Button("Main Menu", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
//                {
//                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}
