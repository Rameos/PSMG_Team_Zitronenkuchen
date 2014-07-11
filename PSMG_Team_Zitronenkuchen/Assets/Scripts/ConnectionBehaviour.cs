using UnityEngine;
using System.Collections;

public class ConnectionBehaviour : MonoBehaviour
{

    string ip = "Enter IP adress!";

    public static bool clickedConnect = false;

    public static bool initializedServer = false;

    public int connectionPort = 25001;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnMouseExit()
    {
        if (gameObject.tag == "ConnectButton" && !clickedConnect)
        {
            gameObject.guiText.color = Color.white;
        }
    }

    void OnMouseEnter()
    {
        if (gameObject.tag == "ConnectButton")
        {
            gameObject.guiText.color = new Color32(87, 192, 195, 255); //turqoise
        }
    }

    void OnMouseUp()
    {
        if (gameObject.tag == "ConnectButton")
        {
            gameObject.guiText.color = new Color32 (218, 164, 59, 255); //orange
            clickedConnect = true;
            if (Network.peerType == NetworkPeerType.Disconnected) Network.Connect(ip, connectionPort);
        }
    }

    void OnGUI()
    {

        ip = GUI.TextField(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 125, 160, 25), ip, 40);

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            GUI.Label(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 30 + 125, 300, 20), "Status: Disconnected");
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 60 + 125, 120, 20), "Initialize Server"))
            {
                
                Network.InitializeServer(32, connectionPort, true);
                initializedServer = true;
            }
        }
        else if (Network.peerType == NetworkPeerType.Client)
        {
            GUI.Label(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 30 + 125, 300, 20), "Status: Connected as Client");
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / + 60 + 125, 120, 20), "Disconnect"))
            {
                Network.Disconnect(200);
            }
        }
        else if (Network.peerType == NetworkPeerType.Server)
        {
            GUI.Label(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 30 + 125, 300, 20), "Status: Connected as Server");
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 60 + 125, 120, 20), "Disconnect"))
            {
                Network.Disconnect(200);
            }
        }

       
    }
}