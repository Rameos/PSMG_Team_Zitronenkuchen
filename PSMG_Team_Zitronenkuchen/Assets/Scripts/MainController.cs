using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

    private int tirkid;
    // private int researchPoints;
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
        // researchPoints = 0;
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
        // research(5);
        foreach (Specialisation node in specialisedNodes)
        {
            if (node is EconomySpecialisation)
            {
                earn(node.Level * 5);
            }
            // Research is postponed to nice to have
            //else if (node is ResearchSpecialisation)
            //{
            //    research(node.Level * 5);
            //}
            else if (node is MilitarySpecialisation)
            {
                if (((MilitarySpecialisation)node).Troops < 100)
                {
                    if (((MilitarySpecialisation)node).RecruitCounter > 0)
                    {
                        Debug.Log(((MilitarySpecialisation)node).RecruitCounter);
                        ((MilitarySpecialisation)node).recruit();
                        ((MilitarySpecialisation)node).RecruitCounter -= 1;
                    }
                    //Debug.Log(((MilitarySpecialisation)node).Troops + " troops on " + node.Pos);
                }

            }
            else if (node is BaseSpecialisation)
            {
                if (((BaseSpecialisation)node).Troops < 150)
                {
                    if (((BaseSpecialisation)node).RecruitCounter > 0)
                    {
                        Debug.Log(((BaseSpecialisation)node).RecruitCounter);
                        ((BaseSpecialisation)node).recruit();
                        ((BaseSpecialisation)node).RecruitCounter -= 1;
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
                break;
            case "Military":
                newBuilt = new MilitarySpecialisation(hex, pos);                
                break;
            // Research is postponed to nice to have
            //case "Research":
            //    newBuilt = new ResearchSpecialisation(hex, pos);
            //    break;
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
                int owner;
                if(Network.isServer) owner = 1;
                else owner = 2;
                nview.RPC("setOwner", RPCMode.AllBuffered, nview.viewID, owner);
                if (newBuilt is MilitarySpecialisation)
                {
                    extendInfluenceArea(hex);
                }
                specialisedNodes.Add(newBuilt);
                audio.PlayOneShot(building);
                return true;
            }
        }
        audio.PlayOneShot(denied);
        return false;
    }

    private void updateArea(GameObject hex, int owner)
    {
        ArrayList neighbours = hex.GetComponent<HexField>().getSurroundingFields();
        foreach (GameObject neighbourHex in neighbours)
        {
            if (neighbourHex.GetComponent<HexField>().owner == owner)
            {
                ArrayList neighboursNeighbours = neighbourHex.GetComponent<HexField>().getSurroundingFields();
                bool destroy = true;
                foreach(GameObject neighboursNeighboursHex in neighboursNeighbours){

                    if ((neighboursNeighboursHex.GetComponent<HexField>().specialisation == "Military" || neighboursNeighboursHex.GetComponent<HexField>().specialisation == "Base") && neighboursNeighboursHex.GetComponent<HexField>().owner == owner)
                    {
                        destroy = false;
                        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                        Debug.Log("Specialisation: " + neighboursNeighboursHex.GetComponent<HexField>().specialisation);
                        break;
                    }                    
                }
                if (destroy)
                {
                    neighbourHex.GetComponent<HexField>().owner = 0;
                    neighbourHex.GetComponent<HexField>().specialisation = null;
                    Debug.Log(neighbourHex.GetComponent<HexField>().specialisation + "; " + neighbourHex.GetComponent<HexField>().owner);
                    neighbourHex.GetComponent<HexField>().decolorUnownedArea();
                    foreach (Specialisation node in specialisedNodes)
                    {
                        if (neighbourHex.Equals(node.Hex))
                        {
                            specialisedNodes.Remove(node);
                            NetworkView nview = node.Hex.networkView;
                            NetworkViewID nviewId = nview.viewID;
                            nview.RPC("DestroyBuilding", RPCMode.AllBuffered, nviewId);
                            break;
                        }
                    }
                }
            }
        }
    }

    

    private void extendInfluenceArea(GameObject hex)
    {
        ArrayList neighbours = hex.GetComponent<HexField>().getSurroundingFields();
        hex.GetComponent<HexField>().colorOwnedArea();
        foreach (GameObject obj in neighbours)
        {
            int newOwner;
            if(Network.isServer) newOwner = 1;
            else newOwner = 2;
            if (obj.GetComponent<HexField>().owner == 0)
            {
                obj.GetComponent<HexField>().owner = newOwner;
                obj.GetComponent<HexField>().colorOwnedArea();
            }
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

    //private void research(int value)
    //{
    //    researchPoints += value;
    //}

    //public bool discover(int value)
    //{
    //    if (researchPoints >= value)
    //    {
    //        researchPoints -= value;
    //        return true;
    //    }
    //    else return false;
    //}

    void OnGUI()
    {

        //GUI.Label(new Rect(0, 0, 300, 150), ("Tirkid: " + tirkid + "   Research: " + researchPoints));
        GameObject ressourcelabel = GameObject.FindGameObjectWithTag("GUIRessources");
        ressourcelabel.guiText.text = "Tirkid: " + tirkid; // +"   Research: " + researchPoints;
        
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
                    int owner = 0;
                    if (Network.isServer) owner = 2;
                    if (Network.isClient) owner = 1;
                    destination.GetComponent<HexField>().owner = owner;
                    specialisedNodes.Remove(node);
                    NetworkViewID destinationNviewId = destination.networkView.viewID;
                    node.Hex.networkView.RPC("explobumm", RPCMode.All, destinationNviewId);
                    bool win = false;
                    if (node is BaseSpecialisation)
                    {
                        win = true;
                        gameEnd(!win);
                    }
                    if (Network.isServer) owner = 1;
                    if (Network.isClient) owner = 2;
                    updateArea(node.Hex, owner);
                    destination.GetComponent<HexField>().decolorUnownedArea();
                    destination.networkView.RPC("successfulAttack", RPCMode.OthersBuffered, destinationNviewId, survivingTroops, node.Pos, win);
                    break;
                }
                
                else
                {
                    // attack failed
                    Debug.Log("attack failed");
                    troops -= sentTroops;
                    if (node is MilitarySpecialisation) ((MilitarySpecialisation)node).Troops = troops;
                    if (node is BaseSpecialisation) ((BaseSpecialisation)node).Troops = troops;
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
            destination.networkView.RPC("buildMilitary", RPCMode.AllBuffered, destination.networkView.viewID);
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
            destination.GetComponent<HexField>().colorOwnedArea();
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
