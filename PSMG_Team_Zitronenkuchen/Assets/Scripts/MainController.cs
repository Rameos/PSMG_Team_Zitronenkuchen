using UnityEngine;
using System.Collections;

/**
 * This is the central logic script for each player. Here the ressources of the player are stored and updated.
 * All actions which can be called form the popupmenues like Building, recruiting, moving and attacking take place here.
 **/
public class MainController : MonoBehaviour {

    // the central ressource of the game. the value is the amount which the player has at the moment.
    private int tirkid;

    // these arraylists contain the buildings a player already has, the buildings he is currently building and the buildings(/buildingstates) that are designated for removal
    public ArrayList specialisedNodes = new ArrayList();
    public ArrayList buildingNodes = new ArrayList();
    public ArrayList removeNodes = new ArrayList();
    
    // the amount of troops which are to be sent is stored here. if no troop send takes place it is 0
    private int sendingTroops = 0;
    // the gameobject from which a troopsend originates is stored here. if no troop send takes place it is null
    private static GameObject sendOrigin;
    // the gameobject to which a troops are sent is stored here. if no troop send takes place it is null
    private static GameObject destinationHex;

    // the audio clips to play if a building is built or an action is denied(because of not enough ressources)
    public AudioClip building;
    public AudioClip denied;

    private int selectedRace = CustomGameProperties.alienRace;

	// Use this for initialization
	void Start () {
        tirkid = 300;

        // this method is invoked every second. it is responsible for updateing the ressources and general gameflow(building etc)
        InvokeRepeating("updateRessources", 1, 1);
        // this method is invoked 5 times per second. it is responsible for updating the troopsize
        InvokeRepeating("updateTroops", 1, 0.2f);
	}

	
	// Update is called once per frame
	void Update () {        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // exits the game and returns to the main menu. ATTENTION: the game is not paused, it is terminated. If errors occur(no connection possible etc.) a complete restart of the game is needed.
            Application.LoadLevel("Alternate_Main_Menu");
        }
	}

    // this method is invoked 5 times per second. it is responsible for updating the troopsize
    void updateTroops()
    {
        foreach (Specialisation node in specialisedNodes)
        {
            // iterate through all specialisations the player has built
            if (node is MilitarySpecialisation)
            {
                if (((MilitarySpecialisation)node).Troops < 100)
                {
                    // troops below the maximum
                    if (((MilitarySpecialisation)node).RecruitCounter >= 1)
                    {
                        // troops have to be recruited
                        // increase the troop count by calling recruit() on the military node
                        ((MilitarySpecialisation)node).recruit();
                        ((MilitarySpecialisation)node).RecruitCounter--;


                    }
                }
                else
                {
                    // maximum troopcount reached. abort recruit
                    ((MilitarySpecialisation)node).RecruitCounter = 0;
                }
                // visualize the troopcount
                NetworkView nview = node.Hex.networkView;
                nview.RPC("showTroops", RPCMode.All, ((MilitarySpecialisation)node).Troops);
            }
            else if (node is BaseSpecialisation)
            {
                // visualize troopcount on base node
                NetworkView nview = node.Hex.networkView;
                nview.RPC("showTroops", RPCMode.All, ((BaseSpecialisation)node).Troops);
            }
        }
    }

    // this method is invoked every second. it is responsible for updateing the ressources and general gameflow(building etc)
    void updateRessources()
    {
        updateBuildingStates();
        // this is the general income(from the basespecialisation)
        earn(5);
        foreach (Specialisation node in specialisedNodes)
        {
            // iterate through all specialisations the player has built
            if (node is EconomySpecialisation)
            {
                // income from each military specialisation is computed from the level of the specialisation and the race of the player(economy race gets 0.2f bonus)
                float raceModifier = 0;
                int sum = node.Level * 5;
                if (selectedRace == 2)
                {
                    raceModifier = 0.2f;
                }
                sum += (int) (sum * raceModifier);
                earn(sum);
            }
        }
    }

    // this method is called from the popupmenu when the player decides to build something
    // the parameters define the type of specialisation to build, the hex and its position where to build the specialisation
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
            default:
                newBuilt = null;
                break;
        }

        if (newBuilt != null)
        {
            if (spend(newBuilt.Cost))
            {
                // only true if the player can afford the costs of the new specialisation
                hex.GetComponent<HexField>().isFilled = true;
                hex.GetComponent<HexField>().spec = newBuilt;
                NetworkView nview = hex.networkView;
                nview.RPC("setSpecialisation", RPCMode.AllBuffered, type);

                int owner;
                if(Network.isServer) owner = 1;
                else owner = 2;
                nview.RPC("setOwner", RPCMode.AllBuffered, nview.viewID, owner);
                // add specialisation to the list of specialisations that are currently built
                buildingNodes.Add(newBuilt);
                audio.PlayOneShot(building);
                return true;
            }
        }
        // player cannot afford the specialisation. play negative sound
        audio.PlayOneShot(denied);
        return false;
    }

    // this method handles the building animation
    private void updateBuildingStates()
    {
        int state = 0;
        string alienBuildingState = "";
        foreach (Specialisation node in buildingNodes)
        {
            // iterate through all specialisations the player is currently building
            NetworkView nview = node.Hex.networkView;
            NetworkViewID nviewId = nview.viewID;

            if (node is MilitarySpecialisation)
            {
                if (selectedRace == 1)
                {
                    alienBuildingState = "militaryMIL";
                }
                else
                {
                    alienBuildingState = "militaryECO";
                }

                if (((MilitarySpecialisation)node).BuildCounter <= 3)
                {
                    // toggle the next state of the building animation
                    state = ((MilitarySpecialisation)node).BuildCounter;
                    
                    nview.RPC("toggleVisibility", RPCMode.AllBuffered, nviewId, state, alienBuildingState);
                    ((MilitarySpecialisation)node).BuildCounter++;
                }
                else
                {
                    // last state of the animation reached. destroy previous states. extend influence are and add specialisation to the list of already built specialisations
                    nview.RPC("destroyBuilding", RPCMode.AllBuffered, nviewId, alienBuildingState);
                    removeNodes.Add(node);
                    extendInfluenceArea(node.Hex);
                    specialisedNodes.Add(node);
                    nview.RPC("setBuildingStatus", RPCMode.AllBuffered, 1);
                }
            }
            else if (node is EconomySpecialisation)
            {
                if (selectedRace == 1)
                {
                    alienBuildingState = "economyMIL";
                }
                else
                {
                    alienBuildingState = "economyECO";
                }
                if (((EconomySpecialisation)node).BuildCounter <= 3)
                {
                    // toggle the next state of the building animation
                    state = ((EconomySpecialisation)node).BuildCounter;
                    
                    nview.RPC("toggleVisibility", RPCMode.AllBuffered, nviewId, state, alienBuildingState);
                    ((EconomySpecialisation)node).BuildCounter++;
                }
                else
                {
                    // last state of the animation reached. destroy previous states. extend influence are and add specialisation to the list of already built specialisations
                    nview.RPC("destroyBuilding", RPCMode.AllBuffered, nviewId, alienBuildingState);
                    removeNodes.Add(node);
                    specialisedNodes.Add(node);
                    nview.RPC("setBuildingStatus", RPCMode.AllBuffered, 1);
                }
            }
        }
        foreach (Specialisation removeNode in removeNodes)
        {
            // remove Specialisations that already finished building from the building list
            buildingNodes.Remove(removeNode);
        }
        removeNodes.Clear();
    }

    // this method is called after an attack if the influence area of the player has to be redefined(if he lost a military specialisation)
    private void updateArea(GameObject hex, int owner)
    {
        // get the neighbours of the lost hexfield
        ArrayList neighbours = hex.GetComponent<HexField>().getSurroundingFields();
        foreach (GameObject neighbourHex in neighbours)
        {
            if (neighbourHex.GetComponent<HexField>().owner == owner)
            {
                // if this neighbourfield is not occupied by the opponent get the neighbours of this neighbourhex to determined if it has to be destroyed(if there is no other military specialisation or the base nearby)
                ArrayList neighboursNeighbours = neighbourHex.GetComponent<HexField>().getSurroundingFields();
                bool destroy = true;
                foreach(GameObject neighboursNeighboursHex in neighboursNeighbours){
                    if ((neighboursNeighboursHex.GetComponent<HexField>().specialisation == "Military" || neighboursNeighboursHex.GetComponent<HexField>().specialisation == "Base") && neighboursNeighboursHex.GetComponent<HexField>().owner == owner)
                    {
                        // this field is "saved" and still belongs to the player because there is another Military Specialisation or the base nearby
                        destroy = false;
                        break;
                    }                    
                }
                if (destroy)
                {
                    // there is no other Military Specialisation nearby -> everything(e.g. economy specialisations) on this field is destroyed
                    neighbourHex.GetComponent<HexField>().decolorUnownedArea();
                    foreach (Specialisation node in specialisedNodes)
                    {
                        if (neighbourHex.Equals(node.Hex))
                        {
                            specialisedNodes.Remove(node);
                            NetworkView nview = node.Hex.networkView;
                            NetworkViewID nviewId = nview.viewID;
                            nview.RPC("destroyBuilding", RPCMode.AllBuffered, nviewId);
                            break;
                        }
                    }
                }
            }
        }
    }

    
    // this method extends the influence are of the player to all fields around the given hex(except there is already an enemy specialisation on it)
    private void extendInfluenceArea(GameObject hex)
    {
        ArrayList neighbours = hex.GetComponent<HexField>().getSurroundingFields();
        hex.GetComponent<HexField>().colorOwnedArea();
        foreach (GameObject obj in neighbours)
        {
            int newOwner;
            if (Network.isServer) newOwner = 1;
            else newOwner = 2;
            if (obj.GetComponent<HexField>().owner == 0)
            {
                // only if the field is currently unowned
                obj.GetComponent<HexField>().owner = newOwner;
                obj.GetComponent<HexField>().colorOwnedArea();
            }
        }
    }

    // return if the player can afford the given sum. subtract given sum from player's ressources if true
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

    // return if the player can afford to recruit troops. subtract 150 from player's ressources if true
    public bool buildTroops()
    {
        if (tirkid - 150 >= 0)
        {
            tirkid -= 150;
            return true;
        }
        return false;
    }

    // show the ressources of the player on screen
    void OnGUI()
    {
        GameObject ressourcelabel = GameObject.FindGameObjectWithTag("GUIRessources");
        ressourcelabel.guiText.text = " " + tirkid;
    }

    // returns how much troops the player can move from the given hexfield. sets it as sendorigin if the number is larger than 0
    public int moveTroops(GameObject selectedHexagon)
    {
        foreach (Specialisation node in specialisedNodes)
        {
            if (selectedHexagon.Equals(node.Hex))
            {
                sendOrigin = selectedHexagon;
                return ((MilitarySpecialisation)node).Troops;
            }
        }
        // hexagon not found. no troops can be sent
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

    // called from the military menu if the player wants to attack or move his troops. the int cpunt is the number of troops to be moved. the bool attack states what type of movement it is.
    public void startTroopSend(int count, bool attack){
        foreach (GameObject obj in sendOrigin.GetComponent<HexField>().getSurroundingFields())
        {
            HexField hex = obj.GetComponent<HexField>();
            if (hex.specialisation == "Military" || hex.specialisation == "Base")
            {
                if (attack)
                {
                    // player is attacking
                    if ((hex.owner == 2 && Network.isServer) || (hex.owner == 1 && Network.isClient))
                    {
                        // highlight enemy nodes as possible attack target
                        highlightMilitaryNode(hex, false);
                        hex.InRange = true;
                    }
                }
                else
                {
                    // player is moving troops peacefully
                    if ((hex.owner == 1 && Network.isServer) || (hex.owner == 2 && Network.isClient))
                    {
                        // highlight own nodes as possible target for movement
                        if (hex.specialisation == "Military")
                        {
                            if (((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType == ((MilitarySpecialisation)hex.spec).WeaponType || ((MilitarySpecialisation)hex.spec).WeaponType == 0)
                            {
                                highlightMilitaryNode(hex, true);
                                hex.InRange = true;
                            }
                        }
                        else if (hex.specialisation == "Base")
                        {
                            if (((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType == ((BaseSpecialisation)hex.spec).WeaponType || ((BaseSpecialisation)hex.spec).WeaponType == 0)
                            {
                                highlightMilitaryNode(hex, true);
                                hex.InRange = true;
                            }
                        }
                        
                    }
                }
            }
            
        }
        sendingTroops = count;
    }

    // highlight enemy node as possible attack target or own node as possible target for movement. determined by given bool.
    private void highlightMilitaryNode(HexField hex, bool ownNode)
    {
        if (ownNode && hex != sendOrigin.GetComponent<HexField>())
        {
            Vector3 pos = hex.transform.position;
            pos.y = 0.65f;
            GameObject highlighter = Resources.Load("moveHighlighter", typeof(GameObject)) as GameObject;
            Instantiate(highlighter, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        }
        else if(!ownNode &&  hex != sendOrigin.GetComponent<HexField>())
        {
            Vector3 pos = hex.transform.position;
            pos.y = 0.65f;
            GameObject highlighter = Resources.Load("attackHighlighter", typeof(GameObject)) as GameObject;
            Instantiate(highlighter, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        }
    }

    // this method is called from main menu when the player has chosen a target for the peaceful troop movement to his own node
    public void sendTroops(GameObject destination)
    {
        destinationHex = destination;

        if (sendOrigin != destination)
        {
            foreach (Specialisation node in specialisedNodes)
            {
                if (destination.Equals(node.Hex))
                {
                    // send troops to this node
                    if (node is MilitarySpecialisation)
                    {
                        // if new troopcount on destination would be above troop limit it is now exactly the troop limit. "overflow" troops are lost!
                        if ((((MilitarySpecialisation)node).Troops + sendingTroops) <= 100)
                        {
                            ((MilitarySpecialisation)node).Troops += sendingTroops;
                        }
                        else
                        {
                            ((MilitarySpecialisation)node).Troops = 100;
                        }

                        // determine the weapontyoe of the sent troops and set the weapon type of the destination
                        int milint = ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType;
                        if (milint != 0)
                        {
                            ((MilitarySpecialisation)node).WeaponType = ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType;
                        }
                       
                        // reset the weapontype and troop count on the now empty sendorigin
                        ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).Troops = 0;
                        ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType = 0;
                        sendingTroops = 0;

                        if (sendOrigin.GetComponent<HexField>().destinationHasNoShip())
                        {
                            NetworkView view = destination.networkView;
                            NetworkViewID id = view.viewID;
                            view.RPC("initiateTroopBuilding", RPCMode.AllBuffered, selectedRace, id);
                        }
                        return;
                      
                    }
                    else if (node is BaseSpecialisation)
                    {
                        // if new troopcount on destination would be above troop limit it is now exactly the troop limit. "overflow" troops are lost!
                        if ((((BaseSpecialisation)node).Troops + sendingTroops) <= 150)
                        {
                            ((BaseSpecialisation)node).Troops += sendingTroops;
                        }
                        else
                        {
                            ((BaseSpecialisation)node).Troops = 150;
                        }

                        // determine the weapontyoe of the sent troops and set the weapon type of the destination
                        int milint = ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType;
                        if (milint != 0)
                        {
                            ((BaseSpecialisation)node).WeaponType = ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType;
                        }

                        // reset the weapontype and troop count on the now empty sendorigin
                        ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).Troops = 0;
                        ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType = 0;
                        sendingTroops = 0;
                        return;
                        
                    }

                }
            }
            // only called when the destination was not found. no sending takes place.
            sendingTroops = 0;
               
        }        
    }

    // called from hexfield after the troop movement animation is finished. builds a new ship on the destination if it needs one
    public static void receiveAnimationCallback()
    {        
        if (sendOrigin.GetComponent<HexField>().destinationHasNoShip())
        {
            NetworkView view = destinationHex.networkView;
            NetworkViewID id = view.viewID;
            view.RPC("initiateTroopBuilding", RPCMode.AllBuffered, CustomGameProperties.alienRace, id);
        }
    }

    // this method is called from the attacking player(!) when he choses the attack here option in military menu on an oppenent node.
    // as the enemy specialisations are not available to the player this is solved by a number of rpc calls
    public void sendAttack(GameObject destination)
    {
        NetworkViewID destinationNviewId = destination.networkView.viewID;
        int attackerWeapontype = ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType;

        // send rpc call to other player to process this attack -> receive attack gets called on the other players maincontroller
        destination.networkView.RPC("processAttack", RPCMode.OthersBuffered, destinationNviewId, sendingTroops, attackerWeapontype, selectedRace);
        // reset troops to 0 on sendorigin
        foreach (Specialisation node in specialisedNodes)
        {
            if (sendOrigin.Equals(node.Hex))
            {
                ((MilitarySpecialisation)node).Troops = 0;
            }
        }
        sendingTroops = 0;
    }

    // this method is called from the rpc method of the attacked Hexfield by the attackingplayer to the defnding player(!). it determines the winner of the fight.
    public void receiveAttack(GameObject destination, int sentTroops, int attackerWeaponType, int attackerRace)
    {
        foreach (Specialisation node in specialisedNodes)
        {
            if (destination.Equals(node.Hex))
            {
                // get weapontype and troopcount for the defender. those of the attacker are given above
                int defenderWeaponType = 0;
                if (node is MilitarySpecialisation) defenderWeaponType= ((MilitarySpecialisation)node).WeaponType;
                if (node is BaseSpecialisation) defenderWeaponType = ((BaseSpecialisation)node).WeaponType;
                int troops = 0;
                if (node is MilitarySpecialisation) troops = ((MilitarySpecialisation)node).Troops;
                if (node is BaseSpecialisation) troops = ((BaseSpecialisation)node).Troops;

                bool attackSuccess = false;
                // compute the attackModifier. initially the defender has a bonus of 0.1 thus the attackModifier starts at 0.9
                float attackModifier = 0.9f;
                if (attackerRace != selectedRace)
                {
                    // players chose different races; if they had chosen the same race noone would get an advantage
                    if (attackerRace == 1)
                    {
                        // attacker chose military race -> 0.2 advantage for the attacker
                        attackModifier += 0.2f;
                    }
                    else
                    {
                        // defender chose military race -> 0.2 advantage for the defender
                        attackModifier -= 0.2f;
                    }
                }
                // determine weather the weapontype results in an advantage for the attacker or defender. the weapontypes follow a Rock Paper Scissors model. Laser Cannon > Proton Torpedos > EMP Cannon > Laser Cannon...
                switch (attackerWeaponType)
                {
                    case MilitarySpecialisation.LASER:
                        switch (defenderWeaponType)
                        {
                            case MilitarySpecialisation.LASER:
                                // same weapon, attackmodifier not changed
                                break;
                            case MilitarySpecialisation.PROTONS:
                                // attacker weapon beats defender weapon -> 0.2 advantage for the attacker
                                attackModifier += 0.2f;
                                break;
                            case MilitarySpecialisation.EMP:
                                // defender weapon beats attacker weapon -> 0.2 advantage for the defender
                                attackModifier -= 0.2f;
                                break;
                        }
                        break;
                    case MilitarySpecialisation.PROTONS:
                        switch (defenderWeaponType)
                        {
                            case MilitarySpecialisation.LASER:
                                // defender weapon beats attacker weapon -> 0.2 advantage for the defender
                                attackModifier -= 0.2f;
                                break;
                            case MilitarySpecialisation.PROTONS:
                                // same weapon, attackmodifier not changed
                                break;
                            case MilitarySpecialisation.EMP:
                                // attacker weapon beats defender weapon -> 0.2 advantage for the attacker
                                attackModifier += 0.2f;
                                break;
                        }
                        break;
                    case MilitarySpecialisation.EMP:
                        switch (defenderWeaponType)
                        {
                            case MilitarySpecialisation.LASER:
                                // attacker weapon beats defender weapon -> 0.2 advantage for the attacker
                                attackModifier += 0.2f;
                                break;
                            case MilitarySpecialisation.PROTONS:
                                // defender weapon beats attacker weapon -> 0.2 advantage for the defender
                                attackModifier -= 0.2f;
                                break;
                            case MilitarySpecialisation.EMP:
                                // same weapon, attackmodifier not changed
                                break;
                        }
                        break;
                }
                // compute the actual troop power of the attacker by multiplying the troopcount with the modifier
                sentTroops = (int) (attackModifier * sentTroops);

                if (troops < sentTroops)
                {
                    attackSuccess = true;
                }
                else if (troops > sentTroops)
                {
                    attackSuccess = false;
                }
                else if (troops == sentTroops)
                {
                    // if troops are even after modifier, defender wins
                    attackSuccess = false;
                }
                if (attackSuccess)
                {
                    // successful attack. specialisation of the defender is destroyed
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
                        // the attacker destroyed the base specialisation of the defender. the game is over. the attacker won
                        win = true;
                        destination.networkView.RPC("successfulAttack", RPCMode.OthersBuffered, destinationNviewId, survivingTroops, node.Pos, win);
                        gameEnd(!win);
                    }
                    else
                    {
                        // decolor lost influence area of the defender. call the successful attack rpc method in the attacker hexfield script -> attackSuccess method called on the other player's maincontroller
                        if (Network.isServer) owner = 1;
                        if (Network.isClient) owner = 2;
                        updateArea(node.Hex, owner);
                        destination.GetComponent<HexField>().decolorUnownedArea();
                        destination.networkView.RPC("successfulAttack", RPCMode.OthersBuffered, destinationNviewId, survivingTroops, node.Pos, win);
                    }

                    break;
                }
                else
                {
                    // attack failed. the troops of the defender that died are subtracted from his troopcount
                    troops -= sentTroops;
                    if (node is MilitarySpecialisation) ((MilitarySpecialisation)node).Troops = troops;
                    if (node is BaseSpecialisation) ((BaseSpecialisation)node).Troops = troops;

                    break;
                }
            }
        }
    }

    public void resetRanges()
    {
        foreach (Specialisation spec in specialisedNodes)
        {
            spec.Hex.GetComponent<HexField>().InRange = false;
        }
    }

    public void setRanges()
    {
        foreach (Specialisation spec in specialisedNodes)
        {
            spec.Hex.GetComponent<HexField>().InRange = true;
        }
    }

    // this method is called from the rpc method of the attacked Hexfield by the defending player to the attacking player(!). it is called after the fight is over and the attacker has won.
    public void attackSuccess(GameObject destination, int survivingTroops, Vector3 pos, bool win)
    {
        if (win)
        {
            // game ends if the win condition was fulfilled. attacking player is the winner.
            gameEnd(win);
        }
        else
        {
            // build militarynode on conquered field for free(=earn+build)
            earn(150);
            int builder = Network.isServer ? 1:0;
            destination.GetComponent<HexField>().owner = Network.isServer ? 1 : 2;
            destination.networkView.RPC("buildMilitary", RPCMode.AllBuffered, destination.networkView.viewID, selectedRace, builder);
            Specialisation spec = new MilitarySpecialisation(destination, destination.transform.position);
            buildingNodes.Add(spec);
            audio.PlayOneShot(building);

            destination.GetComponent<HexField>().isFilled = true;
            destination.GetComponent<HexField>().spec = spec;
            destination.networkView.RPC("setSpecialisation", RPCMode.AllBuffered, "Military");
            // set troops to surviving troops and weapontypoe to the weapontype of the troops which have attacked
            ((MilitarySpecialisation)spec).Troops = survivingTroops;
            ((MilitarySpecialisation)spec).WeaponType = ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType;
            ((MilitarySpecialisation)sendOrigin.GetComponent<HexField>().spec).WeaponType = 0;
            destination.GetComponent<HexField>().colorOwnedArea();
        }
        
    }

    // end the game. if the bool is true the player is the winner
    private void gameEnd(bool win)
    {
        if (win)
        {
            Application.LoadLevel("WinScreen");
        }
        else
        {
            Application.LoadLevel("LoseScreen");
        }
    }
}
