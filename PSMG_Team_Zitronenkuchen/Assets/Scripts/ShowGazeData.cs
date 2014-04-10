
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using iViewX;

public class ShowGazeData : MonoBehaviour {

    // Set 'true' if you want to render a standard Window for the AccuacyScreen
    public bool visualisationOfAccuracyWindow = true;

    //Position and Texture for the Gaze
    public Texture2D gazeCursor;
    public GUIStyle fontStyle; 
    
    //String for the GazeButtonElement
    private string gazeButtonText = "Show GazeCursor";
    private bool isGazeCursorActive = false;
    private int xPos_Elements;
    private int yPos_Element;


    void OnGUI()
    {

        //Calculate the Position of the GUI Elements
        xPos_Elements = (int)(Screen.width * 0.45f);
        yPos_Element = (int)(Screen.height * 0.45f);

        //Render the Informations from the GazeModel
        #region showGazeData
        GUI.Label(new Rect(xPos_Elements, yPos_Element, Screen.width * 0.3f, Screen.height * 0.3f), "Data From GazeModel",fontStyle);
        GUI.Label(new Rect(xPos_Elements, yPos_Element+40, Screen.width * 0.3f, Screen.height * 0.3f), "GazeRightEye:" + gazeModel.posGazeRight.ToString(),fontStyle);
        GUI.Label(new Rect(xPos_Elements, yPos_Element+60, Screen.width * 0.3f, Screen.height * 0.3f), "GazeLeftEye:" + gazeModel.posGazeLeft.ToString(),fontStyle);
        GUI.Label(new Rect(xPos_Elements, yPos_Element+80, Screen.width * 0.3f, Screen.height * 0.3f), "3D-PositionEyeRight: " + gazeModel.posRightEye.ToString(),fontStyle);
        GUI.Label(new Rect(xPos_Elements, yPos_Element+100, Screen.width * 0.3f, Screen.height * 0.3f), "3D-PositionEyeLeft: " + gazeModel.posLeftEye.ToString(),fontStyle);
        GUI.Label(new Rect(xPos_Elements, yPos_Element + 120, Screen.width * 0.3f, Screen.height * 0.3f), "TimeStamp of Sample: " + gazeModel.timeStamp.ToString(), fontStyle);
        GUI.Label(new Rect(xPos_Elements, yPos_Element + 140, Screen.width * 0.3f, Screen.height * 0.3f), "Screen Position: " + gazeModel.gameScreenPosition.ToString(), fontStyle);
        #endregion

        #region Buttons
        //Start CalibrationButton
        if (GUI.Button(new Rect(Screen.width * 0.35f, Screen.height * 0.7f, Screen.width * 0.1f, Screen.height * 0.1f), "Start Calibration"))
            GazeControlComponent.Instance.StartCalibration();

        //Start ValidationButton
        if (GUI.Button(new Rect(Screen.width * 0.45f, Screen.height * 0.7f, Screen.width * 0.1f, Screen.height * 0.1f), "Start Validation"))
        {
            GazeControlComponent.Instance.StartValidation();
        }
        
        //ShowGazeButton
        if (GUI.Button(new Rect(Screen.width * 0.55f, Screen.height * 0.7f, Screen.width * 0.1f, Screen.height * 0.1f), gazeButtonText))
        {
            if(!isGazeCursorActive)
            {
                isGazeCursorActive = true;
                gazeButtonText = "Hide GazeCursor";
            }
            else
            {
                isGazeCursorActive = false;
                gazeButtonText = "Show GazeCursor";

            }
        }
        #endregion

        #region drawGazeCursor
        
        //Draw GazeCursor only if it is activated
        if(isGazeCursorActive)
        {
            Vector3 posGaze = (gazeModel.posGazeLeft + gazeModel.posGazeRight)*0.5f;
            GUI.DrawTexture(new Rect(posGaze.x, posGaze.y, gazeCursor.width, gazeCursor.height), gazeCursor);
        }
        #endregion
    }
}
