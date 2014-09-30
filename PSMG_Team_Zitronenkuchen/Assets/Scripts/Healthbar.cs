using UnityEngine;
using System.Collections;

/**
 * This script is assigned to the empty as well as to the full HealthBar objects which are children to each Military and base Specialisations.
 * It is responsible for visualizing the troop count on the Base/MilitarySpecialisation for both(!) players
 * The empty healthbar serves as background for the full healthbar. the width of the full healthbar is determined by the troopsize
 **/
public class Healthbar : MonoBehaviour {

    public Transform target;
    public Vector3 offset = Vector3.up;

    // this bool is false for the empty healthbar and true for the full healthbar
    public bool isFillState;

    public GUITexture fillTexture;

    private Transform thisTransform;
    private Transform camTransform;
    private Camera cam = Camera.main;

    public int troopSize;
    private float troopPercentage;
    private int fraction;

	// Use this for initialization
	void Start () {
	    thisTransform = transform;
        fillTexture.guiTexture.pixelInset = new Rect(fillTexture.pixelInset.x, fillTexture.pixelInset.y, 0f, fillTexture.pixelInset.height);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 viewPortPosition = cam.WorldToViewportPoint(target.position + offset);

        if (isFillState)
        {
            // only true for the full healthbar
            viewPortPosition.z = 100;
            if (CustomGameProperties.cameraInUse == 2 && thisTransform.GetComponentInParent<HexField>().specialisation == "Military")
            {  
                // player is client
                viewPortPosition.z = -100;
            }
            Vector3 emptyBarPos = target.Find("Healthbar_Empty").transform.position;
            fillTexture.transform.localPosition = new Vector3(emptyBarPos.x, emptyBarPos.y, 100);
        }
        thisTransform.position = viewPortPosition;
	}

  

    public void updateTroopsize(float troops)
    {
        
        if (isFillState)
        {
            // only true for the full healthbar

            if (thisTransform.GetComponentInParent<HexField>().specialisation == "Military")
            {
                // healthbar assigned to a military node. military nodes can have up to 100 troops -> fraction=100
                fraction = 100;                
            }
            else
            {
                // healthbar assigned to a base node(there are only 2 types of nodes with healthbars). base nodes can have up to 150 troops -> fraction=150
                fraction = 150;
            }

            // fill the (full) healthbar according to the percentage of troops to troop maximum on the node
            troopPercentage = troops / fraction;
            fillTexture.guiTexture.pixelInset = new Rect(fillTexture.pixelInset.x, fillTexture.pixelInset.y, 75f * troopPercentage, fillTexture.pixelInset.height);
            
        }
        
    }
}

