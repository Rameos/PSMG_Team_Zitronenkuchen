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
        
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 viewPortPosition = cam.WorldToViewportPoint(target.position + offset);
        if (isFillState)
        {
            viewPortPosition.z = 100;
            if (thisTransform.GetComponentInParent<HexField>().spec is MilitarySpecialisation)
            {
                troopSize = ((MilitarySpecialisation)(thisTransform.GetComponentInParent<HexField>().spec)).Troops; 
            }
            else troopSize = ((BaseSpecialisation)(thisTransform.GetComponentInParent<HexField>().spec)).Troops;
            
            troopPercentage = (float)troopSize / 100;
            fillTexture.guiTexture.pixelInset = new Rect(fillTexture.pixelInset.x, fillTexture.pixelInset.y, 75f * troopPercentage, fillTexture.pixelInset.height);
        }
        thisTransform.position = viewPortPosition;
	}
}

