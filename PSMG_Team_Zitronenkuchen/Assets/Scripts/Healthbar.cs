using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour {

    public Transform target;
    public Vector3 offset = Vector3.up;
    public bool isFillState = false;
    public GUITexture fillTexture;

    private Transform thisTransform;
    private Transform camTransform;
    private Camera cam = Camera.main;

    private int troopSize;
    private float troopPercentage;

	// Use this for initialization
	void Start () {
	    thisTransform = transform;
        troopSize = 0;
	}
	
	// Update is called once per frame
	void Update () {
        int fraction;
        Vector3 viewPortPosition = cam.WorldToViewportPoint(target.position + offset);
        if (isFillState)
        {
            viewPortPosition.z = 100;

            if (thisTransform.GetComponentInParent<HexField>().spec is MilitarySpecialisation)
            {
                troopSize = ((MilitarySpecialisation)(thisTransform.GetComponentInParent<HexField>().spec)).Troops;
                fraction = 100;
            }
            else
            {
                troopSize = ((BaseSpecialisation)(thisTransform.GetComponentInParent<HexField>().spec)).Troops;
                fraction = 150;
            }

            troopPercentage = (float)troopSize / fraction;
            fillTexture.guiTexture.pixelInset = new Rect(fillTexture.pixelInset.x, fillTexture.pixelInset.y, 75f * troopPercentage, fillTexture.pixelInset.height);
        }
        thisTransform.position = viewPortPosition;
	}

    [RPC]
    public void showTroops(int troops)
    {
        //Debug.Log("show troops on " + gameObject.transform.parent.name + ", troops: "+troops);
        troopSize = troops;
    }
}

