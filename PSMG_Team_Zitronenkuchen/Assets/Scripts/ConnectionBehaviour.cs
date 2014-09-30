using UnityEngine;
using System.Collections;

/**
 * This script connects the two players with each other using Unitys Network.InitializeServer and Network.Connect
 * It is attached to the Connect Button of the SelectionMenu Scene
 * The ip is either 127.0.0.1 for local testing on one machine or determined by the LanBroadcastService script
 **/
public class ConnectionBehaviour : MonoBehaviour
{
    // the standard ip is only used for testing on local machine(localhost)
    string ip = "127.0.0.1";

    public static bool clickedConnect = false;

    public static bool initializedServer = false;

    public int connectionPort = 25001;

    public GUISkin mySkin;


    void OnMouseExit()
    {
        if (gameObject.tag == "ConnectButton" && !clickedConnect)
        {
            // restore default connect button color
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
            // try to connect to localhost. only for testing on one machine(with to unity instances). commented out by default
            // if (Network.peerType == NetworkPeerType.Disconnected) Network.Connect(ip, connectionPort);
            // try to connect via the lanbroadcastservice. if there is already a server running connect will be called with that server's ip address. if there is no server running initialize will be called. not working for testing on localhost!
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

            // For initializing a server on the localhost for local testing. commented out by default.
            //if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 80 + 125, 300, 50), "Initialize Server"))
            //{                
            //    Network.InitializeServer(32, connectionPort, false);
            //    initializedServer = true;
            //}

        }
        else if (Network.peerType == NetworkPeerType.Client)
        {
            // client
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50 + 125, 300, 50), "Status: Connected as Client");
            CustomGameProperties.connectionType = 2;
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
            CustomGameProperties.connectionType = 1;
            // disconnect button
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 80 + 125, 300, 50), "Disconnect"))
            {
                Network.Disconnect(200);
                gameObject.GetComponent<LANBroadcastService>().StopBroadCasting();
            }
        }

       
    }

    // called from the lanbroadcastservice. connect to the given ip
    public void Connect(string ip){
        Network.Connect(ip, connectionPort);
    }

    // called from the lanbroadcastservice. initialize server and start broadcasting that a server was initialized
    public void Initialize()
    {
        initializedServer = true;
        Network.InitializeServer(32, connectionPort, false);
        gameObject.GetComponent<LANBroadcastService>().StopBroadCasting();
        gameObject.GetComponent<LANBroadcastService>().StartAnnounceBroadCasting();
    }
}