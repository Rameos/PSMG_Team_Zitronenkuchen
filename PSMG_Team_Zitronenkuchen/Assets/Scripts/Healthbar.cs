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

    private int troopSize;
    private float troopPercentage;
    private int fraction;

	// Use this for initialization
	void Start () {
	    thisTransform = transform;
        troopSize = 0;
	}
	
	// Update is called once per frame
	void Update () {
        //update
        
        Vector3 viewPortPosition = cam.WorldToViewportPoint(target.position + offset);
        Debug.Log("Target: " + target.name + " pos: " + target.position + " viewport: " + viewPortPosition);
        Debug.Log("thisTransform: " + thisTransform);
        if (isFillState)
        {
            viewPortPosition.z = 100;

            if (thisTransform.GetComponentInParent<HexField>().spec is MilitarySpecialisation)
            {
                troopSize = ((MilitarySpecialisation)(thisTransform.GetComponentInParent<HexField>().spec)).Troops;
                fraction = 100;
                if (CustomGameProperties.cameraInUse == 2)
                {
                    viewPortPosition.z = -100;
                }
            }
            else
            {
                troopSize = ((BaseSpecialisation)(thisTransform.GetComponentInParent<HexField>().spec)).Troops;
                fraction = 150;
            }

            troopPercentage = (float)troopSize / fraction;
            fillTexture.guiTexture.pixelInset = new Rect(fillTexture.pixelInset.x, fillTexture.pixelInset.y, 75f * troopPercentage, fillTexture.pixelInset.height);
            fillTexture.transform.localPosition = new Vector3(fillTexture.transform.localPosition.x, fillTexture.transform.localPosition.y, 100);
        }
        thisTransform.position = viewPortPosition;
	}

    [RPC]
    public void showTroops(int troops)
    {
        troopSize = troops;

    }
}

