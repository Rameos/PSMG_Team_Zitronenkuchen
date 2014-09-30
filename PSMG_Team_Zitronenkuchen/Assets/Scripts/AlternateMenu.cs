using UnityEngine;
using System.Collections;
using iViewX;

/**
 * This Script is used by the Alternate_Main_Menu Scene to navigate in the Main Menu, determine if only Mouse or Eyetracker and Mouse is selected and to Start the Game or Tutorial
 **/
public class AlternateMenu : MonoBehaviour {

    public bool isQuit = false;
    public bool isStart = false;
    public bool isTutorial = false;
    public bool isMouse = false;
    public bool isEyetracker = false;

    public AudioClip selectSound;

    void OnMouseEnter()
    {
        // option selected - highlight color
        renderer.material.color = Color.cyan;
        audio.PlayOneShot(selectSound);
    }

    void OnMouseExit()
    {
        // option deselected - default color
        renderer.material.color = Color.white;
    }

    void OnMouseUp()
    {
        if (isQuit == true)
        {
            // quit the game
            Application.Quit();
        }
        else if (isStart == true)
        {
            // let the user chose between eyetracker or mouse
            transform.position = new Vector3(-75, transform.position.y, transform.position.z);
            GameObject useMouseBtn = GameObject.FindGameObjectWithTag("UseMouse");
            GameObject useEyetrackerBtn = GameObject.FindGameObjectWithTag("UseEyetracker");
            useMouseBtn.transform.position = new Vector3(-22, transform.position.y, transform.position.z);
            useEyetrackerBtn.transform.position = new Vector3(-5, transform.position.y, transform.position.z);
        }
        else if (isMouse)
        {
            // start using only mouse
            CustomGameProperties.usesMouse = true;
            Application.LoadLevel("Selection_Menu");
        }
        else if (isEyetracker)
        {
            // start using eyetracker and mouse
            CustomGameProperties.usesMouse = false;
            // calibrate before starting
            GazeControlComponent.Instance.StartCalibration();
            Application.LoadLevel("Selection_Menu");
        }
        else if (isTutorial == true)
        {
            // start the tutorial
            Application.LoadLevel("Tutorial");
        }
    }

    // last two classes are for levelLoading
    [RPC]
    public void LoadLevel(string level)
    {
        StartCoroutine(loadLevel(level));
    }

    private IEnumerator loadLevel(string level)
    {
        Application.LoadLevel(level);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // Allow receiving data again
        Network.isMessageQueueRunning = true;
        // Now the level has been loaded and we can start sending out data
        Network.SetSendingEnabled(0, true);

        // Notify our objects that the level and the network is ready
        foreach (GameObject go in FindObjectsOfType(typeof(GameObject)))
            go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
    }
}

