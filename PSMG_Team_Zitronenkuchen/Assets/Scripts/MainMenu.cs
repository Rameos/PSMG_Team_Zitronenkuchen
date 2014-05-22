using UnityEngine;
using System.Collections;
using iViewX;

public class MainMenu : MonoBehaviour {

    public Texture backgroundTexture;

    public float btnPosX1;
    public float btnPosX2;
    public float btnPosY1;
    public float btnPosY2;
    public float btnHeight1;
    public float btnHeight2;
    public float btnWidth1;
    public float btnWidth2;

    public GUISkin myGuiSkin;

    void OnGUI()
    {

        Screen.lockCursor = false;
        // draw background
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);

        //draw buttons
        GUI.skin = myGuiSkin;

        if (GUI.Button(new Rect(Screen.width * btnPosX1, Screen.height * btnPosY1, Screen.width * btnWidth1, Screen.height * btnHeight1), ""))
        {
            Application.LoadLevel("Create_Gamefield");
        }
        if (GUI.Button(new Rect(Screen.width * btnPosX2, Screen.height * btnPosY2, Screen.width * btnWidth2, Screen.height * btnHeight2), ""))
        {
            GazeControlComponent.Instance.StartCalibration();
        }
    }
	
}
