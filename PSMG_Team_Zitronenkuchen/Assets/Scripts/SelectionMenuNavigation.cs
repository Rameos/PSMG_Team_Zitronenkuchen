using UnityEngine;
using System.Collections;

/**
 *  This script is attached to the Selection_Menu scene and handels the general navigation in this Menu scene.
 **/
public class SelectionMenuNavigation : MonoBehaviour {

    public AudioClip selectSound;

    public AudioClip UFO;

	// Use this for initialization
	void Start () {
	    
	}

    void OnMouseEnter()
    {
        gameObject.guiText.color = new Color32 (218, 164, 59, 255);
        audio.PlayOneShot(selectSound);
    }

    void OnMouseUp()
    {
        if (gameObject.tag == "StartGame" && ConnectionBehaviour.initializedServer && Network.isServer)
        {
            // only the server can start the game
            audio.PlayOneShot(UFO);
            // call load level rpc method on both(!) players
            networkView.RPC("LoadLevel", RPCMode.AllBuffered, "Create_Gamefield");
        }

        if (gameObject.tag == "BackToMenu") 
        {
            Application.LoadLevel("Alternate_Main_Menu");
        }
    }

    void OnMouseExit()
    {
        //initialcolor
        gameObject.guiText.color = new Color32(87, 192, 195, 255); 
    }

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
