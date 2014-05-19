using UnityEngine;
using System.Collections;
using iViewX;

public class ScrollController : MonoBehaviourWithGazeComponent
{

	// Use this for initialization
	void Start () {
       
	}

    void moveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            moveToLayer(child, layer);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnGazeEnter(RaycastHit hit)
    {
        Debug.Log("Helloooo!!");
        GameObject field = GameObject.FindGameObjectWithTag("Field");
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        moveToLayer(field.transform, layer);
        highlightMaterial();
    }

    public override void OnGazeStay(RaycastHit hit)
    {
        float currentTime = Time.time;
        Debug.Log(Time.time);
        Debug.Log(gameObject);
        GameObject leftArrow = GameObject.FindGameObjectWithTag("LeftArrow");
        GameObject rightArrow = GameObject.FindGameObjectWithTag("RightArrow");
        GameObject upArrow = GameObject.FindGameObjectWithTag("UpArrow");
        GameObject downArrow = GameObject.FindGameObjectWithTag("DownArrow");
    }

    public override void OnGazeExit()
    {
        resetMaterial();
    }

    private void highlightMaterial()
    {
        gameObject.transform.renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
    }


    private void resetMaterial()
    {
        gameObject.transform.renderer.material.shader = Shader.Find("Diffuse");
    }


}
