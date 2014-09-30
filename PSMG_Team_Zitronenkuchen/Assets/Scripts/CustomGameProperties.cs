using UnityEngine;
using System.Collections;

/**
 * This script contains some properties about the player.
 **/
public static class CustomGameProperties {

    public static bool usesMouse = false;
    // 1 = Military(Krata'ra); 2 = Economy(Nomicon)
    public static int alienRace = 0;
    //1 = Server; 2 = Client
    public static int connectionType = 0; 
    //1 = Server; 2 = Client
    public static int cameraInUse = 0; 
}
