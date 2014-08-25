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
            if (Network.peerType == NetworkPeerType.Disconnected) Network.Connect(ip, connectionPort);
            // gameObject.GetComponent<LANBroadcastService>().StartSearchBroadCasting(Connect, Initialize);
        }
    }

    void OnGUI()
    {

        ip = GUI.TextField(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 125, 160, 25), ip, 40);

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            // not connected
            GUI.Label(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 30 + 125, 300, 20), "Status: Disconnected");
            // init Server
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 60 + 125, 120, 20), "Initialize Server"))
            {                
                Network.InitializeServer(32, connectionPort, false);
                initializedServer = true;
            }
        }
        else if (Network.peerType == NetworkPeerType.Client)
        {
            // client
            GUI.Label(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 30 + 125, 300, 20), "Status: Connected as Client");
            // disconnect button
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / + 60 + 125, 120, 20), "Disconnect"))
            {
                Network.Disconnect(200);
            }
        }
        else if (Network.peerType == NetworkPeerType.Server)
        {
            // server
            GUI.Label(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 30 + 125, 300, 20), "Status: Connected as Server");
            // disconnect button
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 60 + 125, 120, 20), "Disconnect"))
            {
                Network.Disconnect(200);
            }
        }

       
    }

    public void Connect(string ip){
        Network.Connect(ip, connectionPort);
    }

    public void Initialize()
    {
        Network.InitializeServer(32, connectionPort, false);
    }
}