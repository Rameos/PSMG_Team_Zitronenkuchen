using UnityEngine;
using System.Collections;
using iViewX;

public class ChangeFieldStateOnClick : MonoBehaviourWithGazeComponent
{

    private Material defaultMaterial = Resources.Load("DefaultMaterial", typeof(Material)) as Material;
    private Material highlightedMaterial = Resources.Load("HighlightedMaterial", typeof(Material)) as Material;
    private bool set = false;
 
    PopUpMenu popUpMenu;
    


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


    [RPC]
    void buildMilitary(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject milBuilding = Resources.Load("military-building2", typeof(GameObject)) as GameObject;
        GameObject militaryBuilding = Network.Instantiate(milBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), 0) as GameObject;
        selectedHexagon.renderer.material = Resources.Load("militaryMaterial", typeof(Material)) as Material;
        militaryBuilding.transform.parent = selectedHexagon.transform;
        GameObject unitText = new GameObject();
        TextMesh text = unitText.AddComponent<TextMesh>();
        text.characterSize = 0.1f;
        Font font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        text.font = font;
        text.renderer.material = font.material;
        text.anchor = TextAnchor.MiddleCenter;
        unitText.transform.parent = selectedHexagon.transform;
        unitText.transform.position = selectedHexagon.transform.position;
        unitText.transform.Rotate(new Vector3(45, 0, 0));
    }

    [RPC]
    void buildResearch(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject resBuilding = Resources.Load("research-building2", typeof(GameObject)) as GameObject;
        GameObject researchBuilding = Instantiate(resBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        selectedHexagon.renderer.material = Resources.Load("researchMaterial", typeof(Material)) as Material;
        researchBuilding.transform.parent = selectedHexagon.transform;
    }

    [RPC]
    void buildEconomy(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject ecoBuilding = Resources.Load("economy-building2", typeof(GameObject)) as GameObject;
        GameObject economyBuilding = Instantiate(ecoBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject; ;
        selectedHexagon.renderer.material = Resources.Load("economyMaterial", typeof(Material)) as Material;
        economyBuilding.transform.parent = selectedHexagon.transform;
    }

    [RPC]
    void updateTroops(NetworkViewID id, int troops)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        hex.transform.GetComponentInChildren<TextMesh>().text = "" + troops;
    }

}
