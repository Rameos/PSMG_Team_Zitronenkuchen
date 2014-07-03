using UnityEngine;
using System.Collections;
using iViewX;

public class ChangeFieldStateOnClick : MonoBehaviourWithGazeComponent
{

    private Material defaultMaterial = Resources.Load("DefaultMaterial", typeof(Material)) as Material;
    private Material highlightedMaterial = Resources.Load("HighlightedMaterial", typeof(Material)) as Material;
    private bool set = false;
 
    PopUpMenu popUpMenu;
    MilitaryMenu milMenu;
    


    public override void OnGazeEnter(RaycastHit hit)
    {
        //Debug.Log("Enter");
    }

    //Rotate the Element if the Gaze stays on the Collider
    public override void OnGazeStay(RaycastHit hit)
    {
        //Debug.Log("Stay");
        highlightMaterial();


        if (Input.GetKeyDown(KeyCode.Space))
        {

            Vector3 posGaze = (gazeModel.posGazeLeft + gazeModel.posGazeRight) * 0.5f;
            Vector3 nullVect = new Vector3(0, 0, 0);
            {
                posGaze = new Vector3(Input.mousePosition.x, ((Input.mousePosition.y)-Screen.height)*(-1), 0);
                Debug.Log(posGaze);
            }
           
            if (!set)
            {
                showPopupMenu(posGaze);
            }
            else if (hit.transform.gameObject.GetComponent<HexField>().spec.type == "mil")
            {
                showMilitaryMenu(posGaze);
            }
            
                
        }

    }

    private void highlightMaterial()
    {
        gameObject.transform.renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
    }

    //Reset the Element.Transform when the gaze leaves the Collider
    public override void OnGazeExit()
    {
        //Debug.Log("Exit");
        resetMaterial();
    }

    private void resetMaterial()
    {
        gameObject.transform.renderer.material.shader = Shader.Find("Diffuse");
    }

    // show the popup menu when a field was clicked
    private void showPopupMenu(Vector3 pos)
    {
        Debug.Log("showPopupMenu");

        Debug.Log(pos);

        GameObject field = GameObject.FindGameObjectWithTag("Field");
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        moveToLayer(field.transform, layer);

        popUpMenu = GameObject.FindWithTag("PointLight").GetComponent<PopUpMenu>();
        Debug.Log(popUpMenu);
        popUpMenu.openMenu(pos, gameObject, this);
    }

    private void showMilitaryMenu(Vector3 pos)
    {
        Debug.Log("showMilitaryMenu");

        Debug.Log(pos);

        GameObject field = GameObject.FindGameObjectWithTag("Field");
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        moveToLayer(field.transform, layer);

        milMenu = GameObject.FindWithTag("PointLight").GetComponent<MilitaryMenu>();
        Debug.Log(milMenu);
        milMenu.openMenu(pos, gameObject, this);
    }

    void moveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            moveToLayer(child, layer);
    }


    [RPC]
    public void fieldSet()
    {
        set = true;
    }
    

}
