using UnityEngine;
using System.Collections;

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
        if (gameObject.tag == "StartGame")
        {
            audio.PlayOneShot(UFO);
            if (Network.isServer) networkView.RPC("LoadLevel", RPCMode.AllBuffered, "Create_Gamefield");
        }

        if (gameObject.tag == "BackToMenu") 
        {
            Application.LoadLevel("Alternate_Main_Menu");
        }
    }

    void OnMouseExit()
    {
        gameObject.guiText.color = new Color32(87, 192, 195, 255); //initialcolor
    }
	
	// Update is called once per frame
	void Update () {

    }

    [RPC]
    public void LoadLevel(string level)
    {
        StartCoroutine(loadLevel(level));
    }

    private IEnumerator loadLevel(string level)
    {
        // omitted code

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
