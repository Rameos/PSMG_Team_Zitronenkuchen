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

    public AudioClip sound;

    void OnGUI()
    {

        Screen.lockCursor = false;
        // draw background
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);

        //draw buttons
        GUI.skin = myGuiSkin;

        Rect playBtn = new Rect(Screen.width * btnPosX1, Screen.height * btnPosY1, Screen.width * btnWidth1, Screen.height * btnHeight1);
        Rect calibBtn = new Rect(Screen.width * btnPosX2, Screen.height * btnPosY2, Screen.width * btnWidth2, Screen.height * btnHeight2);

        if (GUI.Button(playBtn, ""))
        {
            Application.LoadLevel("Create_Gamefield");
        }
        if (GUI.Button(calibBtn, ""))
        {
            GazeControlComponent.Instance.StartCalibration();
        }
    }
	
}
