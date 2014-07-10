using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

    private int tirkid;
    private int researchPoints;
    private ArrayList ui = new ArrayList();
    public ArrayList specialisedNodes = new ArrayList();
    private ArrayList labelUI = new ArrayList();
    private int sendingTroops = 0;
    private GameObject sendOrigin;

    public AudioClip building;
    public AudioClip denied;

	// Use this for initialization
	void Start () {
        tirkid = 500;
        researchPoints = 0;
        Debug.Log("Start");
        InvokeRepeating("updateRessources", 1, 1);
	}

	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Application.LoadLevel("Alternate_Main_Menu");
        }
	}

    void updateRessources()
    {
        earn(5);
        research(5);
        foreach (Specialisation node in specialisedNodes)
        {
            if (node is EconomySpecialisation)
            {
                earn(node.Level * 5);
            }
            else if (node is ResearchSpecialisation)
            {
                research(node.Level * 5);
            }
            else if (node is MilitarySpecialisation)
            {
                if (((MilitarySpecialisation)node).Troops < 100)
                {
                    if (((MilitarySpecialisation)node).BuildCounter > 0)
                    {
                        Debug.Log(((MilitarySpecialisation)node).BuildCounter);
                        ((MilitarySpecialisation)node).recruit();
                        ((MilitarySpecialisation)node).BuildCounter -= 1;
                    }
                    //Debug.Log(((MilitarySpecialisation)node).Troops + " troops on " + node.Pos);
                }

            }
            else if (node is BaseSpecialisation)
            {
                if (((BaseSpecialisation)node).Troops < 150)
                {
                    if (((BaseSpecialisation)node).BuildCounter > 0)
                    {
                        Debug.Log(((BaseSpecialisation)node).BuildCounter);
                        ((BaseSpecialisation)node).recruit();
                        ((BaseSpecialisation)node).BuildCounter -= 1;
                    }
                }
            }
        }
    }

    public bool build(string type, GameObject hex, Vector3 pos)
    {   
        Specialisation newBuilt;
        switch (type)
        {
            case "Economy":
                newBuilt = new EconomySpecialisation(hex, pos);
                //hex.guiText.font = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GUIText>().font;
                //hex.guiText.material.color = new Color(1.0f, 1.0f, 1.0f);
                //hex.guiText.transform.position = pos;
                //hex.guiText.text = "Economy";
                //labelUI.Add(hex.guiText);
                break;
            case "Military":
                newBuilt = new MilitarySpecialisation(hex, pos);                
                break;
            case "Research":
                newBuilt = new ResearchSpecialisation(hex, pos);
                break;
            default:
                newBuilt = null;
                break;
        }
        Debug.Log(newBuilt + ", " + newBuilt.Cost);
        if (newBuilt != null)
        {
            if (spend(newBuilt.Cost))
            {
                hex.GetComponent<HexField>().isFilled = true;
                hex.GetComponent<HexField>().spec = newBuilt;
                NetworkView nview = hex.networkView;
                nview.RPC("setSpecialisation", RPCMode.AllBuffered, type);
                if (newBuilt is MilitarySpecialisation)
                {
                    extendInfluenceArea(hex);
                    /*GameObject unitText = new GameObject();
                    TextMesh text = unitText.AddComponent<TextMesh>();
                    text.characterSize = 0.1f;
                    Font font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    text.font = font;
                    text.renderer.material = font.material;
                    text.anchor = TextAnchor.MiddleCenter;
                    unitText.transform.parent = hex.transform;
                    unitText.transform.position = hex.transform.position;
                    unitText.transform.Rotate(new Vector3(45, 0, 0));*/
                }
                specialisedNodes.Add(newBuilt);
                audio.PlayOneShot(building);
                return true;
            }
        }
        audio.PlayOneShot(denied);
        return false;
    }

    

    private void extendInfluenceArea(GameObject hex)
    {
        ArrayList neighbours = hex.GetComponent<HexField>().getSurroundingFields();
        foreach (GameObject obj in neighbours)
        {
            if(Network.isServer) obj.GetComponent<HexField>().owner = 1;
            if (Network.isClient) obj.GetComponent<HexField>().owner = 2;
            obj.GetComponent<HexField>().colorOwnedArea(obj);
        }
    }

    public bool spend(int value)
    {
        if (tirkid >= value)
        {
            tirkid -= value;
            return true;
        }
        else return false;
    }

    public int earn(int value)
    {
        tirkid += value;
        return tirkid;
    }

    public bool buildTroops()
    {
        if (tirkid - 150 >= 0)
        {
            tirkid -= 150;
            return true;
        }
        return false;
    }

    private void research(int value)
    {
        researchPoints += value;
    }

    public bool discover(int value)
    {
        if (researchPoints >= value)
        {
            researchPoints -= value;
            return true;
        }
        else return false;
    }

    void OnGUI()
    {

        //GUI.Label(new Rect(0, 0, 300, 150), ("Tirkid: " + tirkid + "   Research: " + researchPoints));
        GameObject ressourcelabel = GameObject.FindGameObjectWithTag("GUIRessources");
        ressourcelabel.guiText.text = "Tirkid: " + tirkid + "   Research: " + researchPoints;
        
        foreach (Specialisation node in specialisedNodes)
        {
            if (node is MilitarySpecialisation)
            {
                NetworkView nview = node.Hex.networkView;
                nview.RPC("showTroops", RPCMode.AllBuffered, ((MilitarySpecialisation)node).Troops);
            }
            else if (node is BaseSpecialisation)
            {
                NetworkView nview = node.Hex.networkView;
                nview.RPC("showTroops", RPCMode.AllBuffered, ((BaseSpecialisation)node).Troops);
            }
           
        }

    }

    public int moveTroops(GameObject selectedHexagon)
    {
        foreach (Specialisation node in specialisedNodes)
        {
            if (selectedHexagon.Equals(node.Hex))
            {
                //Debug.Log("GOT ONE");
                sendOrigin = selectedHexagon;
                return ((MilitarySpecialisation)node).Troops;
            }
        }
        return 0;
    }

    public void cancelTroopMovement()
    {
        sendOrigin = null;
        sendingTroops = 0;
    }

    public int isSending()
    {
        return sendingTroops;
    }

    public void startTroopSend(int count, bool attack){
        foreach (GameObject obj in sendOrigin.GetComponent<HexField>().getSurroundingFields())
        {
            HexField hex = obj.GetComponent<HexField>();
            if (hex.spec is MilitarySpecialisation)
            {
                if (attack)
                {
                    if ((hex.owner == 2 && Network.isServer) || (hex.owner == 1 && Network.isClient))
                    {
                        highlightMilitaryNode(hex, false);
                    }
                }
                else
                {
                    if ((hex.owner == 1 && Network.isServer) || (hex.owner == 2 && Network.isClient))
                    {
                        highlightMilitaryNode(hex, true);
                    }
                }
            }
            
        }
        sendingTroops = count;
    }

    private void highlightMilitaryNode(HexField hex, bool ownNode)
    {
        if (ownNode)
        {
            hex.gameObject.transform.renderer.material.shader = Shader.Find("Rim");
        }
        else
        {
            hex.gameObject.transform.renderer.material.shader = Shader.Find("Rim");
        }
    }

    public void sendTroops(GameObject destination)
    {
        foreach (Specialisation node in specialisedNodes)
        {
            if (destination.Equals(node.Hex))
            {
                if (node is MilitarySpecialisation && (((MilitarySpecialisation)node).Troops + sendingTroops) <= 100)
                {
                    ((MilitarySpecialisation)node).Troops += sendingTroops;
                }
                else if (node is BaseSpecialisation && (((BaseSpecialisation)node).Troops + sendingTroops) <= 150)
                {
                    ((BaseSpecialisation)node).Troops += sendingTroops;
                }
                
            }
            if (sendOrigin.Equals(node.Hex))
            {
                ((MilitarySpecialisation)node).Troops = 0;
            }
        }
        sendingTroops = 0;
    }

    public void sendAttack(GameObject destination)
    {
        // only working with 2 players!!!
        // opponent nodes not on arraylist. start the rpc call insanity!
        // this is the attacking player
        NetworkViewID destinationNviewId = destination.networkView.viewID;
        destination.networkView.RPC("processAttack", RPCMode.OthersBuffered, destinationNviewId, sendingTroops);
        foreach (Specialisation node in specialisedNodes)
        {
            if (sendOrigin.Equals(node.Hex))
            {
                ((MilitarySpecialisation)node).Troops = 0;
            }
        }
        sendingTroops = 0;
    }

    public void receiveAttack(GameObject destination, int sentTroops)
    {
        // this is the defending player
        foreach (Specialisation node in specialisedNodes)

        {
            if (destination.Equals(node.Hex))
            {
                int troops = 0;
                if (node is MilitarySpecialisation) troops = ((MilitarySpecialisation)node).Troops;
                if (node is BaseSpecialisation) troops = ((BaseSpecialisation)node).Troops;
                if (troops < sentTroops)
                {
                    // successful attack
                    Debug.Log("attack successful");
                    int survivingTroops = sentTroops - troops;
                    if (Network.isServer) destination.GetComponent<HexField>().owner = 1;
                    if (Network.isClient) destination.GetComponent<HexField>().owner = 2;
                    specialisedNodes.Remove(node);
                    NetworkViewID destinationNviewId = destination.networkView.viewID;
                    bool win = false;
                    if (node is BaseSpecialisation)
                    {
                        win = true;
                        gameEnd(!win);
                    }
                    destination.networkView.RPC("successfulAttack", RPCMode.OthersBuffered, destinationNviewId, survivingTroops, node.Pos, win);
                }
                
                else
                {
                    // attack failed
                    Debug.Log("attack failed");
                    troops -= sentTroops;
                }

            }
        }
    }

    public void attackSuccess(GameObject destination, int survivingTroops, Vector3 pos, bool win)
    {
        // this is the attacking player
        if (win)
        {
            gameEnd(win);
        }
        else
        {
            earn(150);
            build("Military", destination, pos);
            foreach (Specialisation node in specialisedNodes)
            {
                if (destination.Equals(node.Hex))
                {
                    int troops = 0;
                    if (node is MilitarySpecialisation) troops = ((MilitarySpecialisation)node).Troops;
                    if (node is BaseSpecialisation) troops = ((BaseSpecialisation)node).Troops;
                    troops = survivingTroops;
                }
            }
        }
        
    }

    private void gameEnd(bool win)
    {
        if (win)
        {
            Debug.Log("YOU WIN");
            Application.LoadLevel("WinScreen");
        }
        else
        {
            Debug.Log("YOU LOSE");
            Application.LoadLevel("LoseScreen");
        }
    }
}
