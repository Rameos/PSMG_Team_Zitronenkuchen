
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using iViewX;

public class ShowGazeCursor : MonoBehaviour
{

    

    //Position and Texture for the Gaze
    public Texture2D gazeCursor;

    
    private bool isGazeCursorActive = false;

    void Start()
    {
        Screen.lockCursor = true;
    }

    void OnGUI()
    {
        Vector3 posGaze = (gazeModel.posGazeLeft + gazeModel.posGazeRight) * 0.5f;
        GUI.DrawTexture(new Rect(posGaze.x, posGaze.y, gazeCursor.width, gazeCursor.height), gazeCursor);
        
    }
}
