using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour {

    public Transform target;
    public Vector3 offset = Vector3.up;
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
        //update
        Vector3 viewPortPosition = cam.WorldToViewportPoint(target.position + offset);
        if (isFillState)
        {
            viewPortPosition.z = 100;
            if (CustomGameProperties.cameraInUse == 2)
            {
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
            

            if (thisTransform.GetComponentInParent<HexField>().specialisation == "Military")
            {
                fraction = 100;
                
            }
            else
            {
                fraction = 150;
            }

            troopPercentage = troops / fraction;
            Debug.Log("troopsize(float): " + troopSize + " trooppercentage: " + troopPercentage + " thisTranform owner: " + thisTransform.GetComponentInParent<HexField>().owner);
            fillTexture.guiTexture.pixelInset = new Rect(fillTexture.pixelInset.x, fillTexture.pixelInset.y, 75f * troopPercentage, fillTexture.pixelInset.height);
            
        }
        
    }
}

