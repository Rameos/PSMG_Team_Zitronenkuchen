using UnityEngine;
using System.Collections;
using iViewX;

public class ChangeFieldStateOnClick : MonoBehaviourWithGazeComponent
{
    private bool set = false;
 
    PopUpMenu popUpMenu;
    MilitaryMenu milMenu;
    HexField currentField;
    


    public override void OnGazeEnter(RaycastHit hit)
    {
        // nothing to do here
    }

    public override void OnGazeStay(RaycastHit hit)
    {
        highlightMaterial();


        if (Input.GetKeyDown(KeyCode.Space))
        {
            // user staring at the field and pressing space -> open menu
            Vector3 pos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 nullVect = new Vector3(0, 0, 0);

            
            Debug.Log(pos);
            
            if (!set)
            {
                // field not set yet
                showPopupMenu(pos);
            }

            else if ((hit.transform.gameObject.GetComponent<HexField>().specialisation == "Military" || hit.transform.gameObject.GetComponent<HexField>().specialisation == "Base") && hit.transform.gameObject.GetComponent<HexField>().FinishedBuilding == true)
            {                
                // field set to military or base
                if ((hit.transform.gameObject.GetComponent<HexField>().owner == 1 && Network.isServer) || (hit.transform.gameObject.GetComponent<HexField>().owner == 2 && Network.isClient))
                {
                    if (hit.transform.gameObject.GetComponent<HexField>().specialisation == "Military")
                    {
                        if (((MilitarySpecialisation)hit.transform.gameObject.GetComponent<HexField>().spec).RecruitCounter == 0) showMilitaryMenu(pos);
                    }
                    else
                    {                       
                        if (((BaseSpecialisation)hit.transform.gameObject.GetComponent<HexField>().spec).RecruitCounter == 0) showMilitaryMenu(pos);
                    }      
                }
                showMilitaryMenu(pos);
            }
            else
            {
                // if user hits already set field that ius nit military -> nothing happens
            }
            
                
        }
    }

    public static void resetHighlighting(GameObject currentHex)
    {
        currentHex.transform.renderer.material.shader = Shader.Find("Diffuse");
    }

    private void highlightMaterial()
    {
        if (!PopUpMenu.isOpen() && !MilitaryMenu.isOpen())
        {
            gameObject.transform.renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
        }
    }

    //Reset the Element.Transform when the gaze leaves the Collider
    public override void OnGazeExit()
    {
        resetMaterial();
    }

    private void resetMaterial()
    {
        if (!PopUpMenu.isOpen() && !MilitaryMenu.isOpen())
        {
            gameObject.transform.renderer.material.shader = Shader.Find("Diffuse"); 
        }
    }



    // show the standard popup menu when a field which is not set was clicked
    private void showPopupMenu(Vector3 pos)
    {
        // Debug.Log("showPopupMenu");

        GameObject field = GameObject.FindGameObjectWithTag("Field");
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        moveToLayer(field.transform, layer);

        popUpMenu = GameObject.FindWithTag("PointLight").GetComponent<PopUpMenu>();
        popUpMenu.openMenu(pos, gameObject, this);
    }

    // show military menu when a military field is clicked
    private void showMilitaryMenu(Vector3 pos)
    {
        // Debug.Log("showMilitaryMenu");

        GameObject field = GameObject.FindGameObjectWithTag("Field");
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        moveToLayer(field.transform, layer);

        milMenu = GameObject.FindWithTag("PointLight").GetComponent<MilitaryMenu>();
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
