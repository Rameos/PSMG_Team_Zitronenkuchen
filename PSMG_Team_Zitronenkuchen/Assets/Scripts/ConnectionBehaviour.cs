using UnityEngine;
using System.Collections;

public class ConnectionBehaviour : MonoBehaviour
{

    string ip = "127.0.0.1";

    public static bool clickedConnect = false;

    public static bool initializedServer = false;

    public int connectionPort = 25001;

    public GUISkin mySkin;

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
            // restore connect button color
            gameObject.guiText.color = Color.white;
        }
    }

    void OnMouseEnter()
    {
        if (gameObject.tag == "ConnectButton")
        {
            // highlight connect button
            gameObject.guiText.color = new Color32(87, 192, 195, 255); //turqoise
        }
    }

    void OnMouseUp()
    {
        if (gameObject.tag == "ConnectButton")
        {
            gameObject.guiText.color = new Color32 (218, 164, 59, 255); //orange
            clickedConnect = true;
            // try to connect to the entered ip
             //if (Network.peerType == NetworkPeerType.Disconnected) Network.Connect(ip, connectionPort);
             gameObject.GetComponent<LANBroadcastService>().StartSearchBroadCasting(Connect, Initialize);
        }
    }

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            // not connected
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50 + 125, 300, 50), "Status: Disconnected");
            // init Server
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 80 + 125, 300, 50), "Initialize Server"))
            {                
                Network.InitializeServer(32, connectionPort, false);
                initializedServer = true;
            }
        }
        else if (Network.peerType == NetworkPeerType.Client)
        {
            // client
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50 + 125, 300, 50), "Status: Connected as Client");
            CustomGameProperties.conntectionType = 2;
            // disconnect button
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / + 80 + 125, 300, 50), "Disconnect"))
            {
                Network.Disconnect(200);
                gameObject.GetComponent<LANBroadcastService>().StopBroadCasting();
            }
        }
        else if (Network.peerType == NetworkPeerType.Server)
        {
            // server
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50 + 125, 300, 50), "Status: Connected as Server");
            CustomGameProperties.conntectionType = 1;
            // disconnect button
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 80 + 125, 300, 50), "Disconnect"))
            {
                Network.Disconnect(200);
                gameObject.GetComponent<LANBroadcastService>().StopBroadCasting();
            }
        }

       
    }

    public void Connect(string ip){
        Network.Connect(ip, connectionPort);
    }

    public void Initialize()
    {
        initializedServer = true;
        Network.InitializeServer(32, connectionPort, false);
        gameObject.GetComponent<LANBroadcastService>().StopBroadCasting();
        gameObject.GetComponent<LANBroadcastService>().StartAnnounceBroadCasting();
    }
}