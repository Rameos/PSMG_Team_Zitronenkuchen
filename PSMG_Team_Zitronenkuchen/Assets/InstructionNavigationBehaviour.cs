using UnityEngine;
using System.Collections;

public class InstructionNavigationBehaviour : MonoBehaviour {

    private float scaleFactor = 1.7787114845f;
    public bool onFullScreen = false;

    private Rect scaleInset;
    private Rect noScaleInset = new Rect(-125,45, 250, 140);

    private Vector3 initialPosition;
    private Vector3 scaledPosition;
    private string initialText = "";


    public AudioClip selectSound;

    public int instructionId;

    void OnMouseEnter()
    {
        if (!onFullScreen)
        {
            audio.PlayOneShot(selectSound);
            gameObject.guiText.color = new Color32(87, 192, 195, 255);
        }
           
    }

    void OnMouseExit()
    {
        if (!onFullScreen)
            gameObject.guiText.color = new Color32(255, 255, 255, 255); //initialcolor
    }

    void OnMouseUp()
    {
        if (!onFullScreen)
        {
            hideOtherInstructions();
            scaleInstructionToFullScreen();
            
        }
        else
        {
            reActivateOtherInstructions();
            rescaleInstruction();
        }
        

    }

    private void hideOtherInstructions()
    {
        GameObject backBtn = GameObject.Find("BackText");
        backBtn.guiText.enabled = false;

        GameObject[] instructions = GameObject.FindGameObjectsWithTag("Instruction");
        foreach (GameObject instruction in instructions)
        {
            if (instruction != gameObject)
            {
                instruction.guiTexture.enabled = false;
                instruction.guiText.enabled = false;
            }
        }
    }

    private void reActivateOtherInstructions()
    {
        GameObject backBtn = GameObject.Find("BackText");
        backBtn.guiText.enabled = true;

        GameObject[] instructions = GameObject.FindGameObjectsWithTag("Instruction");
        foreach (GameObject instruction in instructions)
        {
            if (instruction != gameObject)
            {
                instruction.guiTexture.enabled = true;
                instruction.guiText.enabled = true;
            }
        }
    }

    private void scaleInstructionToFullScreen()
    {
        float distance = 0.28f;
        string instructionText = "";
        switch (instructionId)
        {
            case 1:
                instructionText = "Connect to your opponent by clicking 'CONNECT'. You will either be connected as\n Server or as Client. The Server starts the Game. Don't forget to choose an alien race.";

                break;
            case 2:
                instructionText = "Your building range is indicated by grey field. Focus the hexagon you wish to\n build on and press the space bar to open a menu. You can either biuld military\n or economy buildings. Military enlarge your building rage and enable fleet\n strengthening. Economy buildings generate Tirkid.";
                distance = 0.37f;
                break;
            case 3:
                instructionText = "After placing a military building focus the hexagon and press the space bar again.\n You can now choose a weapon type. Your space ship can be either armed with Proton\n Torpedo, Laser or Electromagnetic Pulse (EMP) cannons. Protons are inferior to \nLaser but beat EMP. Thus Laser lose to Protons but defeat EMP. EMP are superior to Laser.\n You ca'nt further interact with economy buildings. They are busy digging up Tirkid. ";
                distance = 0.37f;
                break;
            case 4:
                instructionText = "After wisely choosing a weapon type focus the hexagon and press the space bar again.\n You now have to build ships. The cost for 25 ships is 150 Tirkid units. The maximum \nnumber of ships of a military node is 100. The grey bar will be filled once you reach\n the maximum size.";
                distance = 0.37f;
                break;
            case 5:
                instructionText = "You can also move the troops from one military node to another or your base if they\n are located within the range. This might come in handy for strengthening your base\n because it can not produce troopps itself.";
                break;
            case 6:
                instructionText = "Nodes you can send your troops to other nodes that are highlighted. You can either send\n troops to unspecialised miltary nodes or military nodes with the same weapon type.\n Your base is an option as well if it is not armed with a different weapon type.";

                break;
            case 7:
                instructionText = "While you're trying to reach your opponent's base and thus try to defeat him you\n should'nt forget your own. It has room for up to 150 troops and receives its weapon type\n after the first fleets arrive. Unfortunately it ca'nt generate troops itself.";

                break;
            case 8:
                instructionText = "In order to send troops from a military node to attack the opponent focus the\n hexagon and press the space bar again. After selecting the attack option you can\n attack one opposing military node ore the base if they are located within the range.\n If fleet sizes are equal the weapon type is decisive. Weapon types are explained\n in 'CHOOSING A FLEET'";
                distance = 0.37f;
                break;
            case 9:
                instructionText = "If you have carefully planned your moves and reached the range of your opponent's\n base heavily armed you now have the possibility to defeat him with a few more moves. ";
                
                break;
        }


        float height = Screen.height - Screen.height / 2;
        float width = height * scaleFactor;

        scaledPosition = new Vector3(0.5f, distance, 0.4f);
        scaleInset = new Rect(-width / 2, 60.0f, width, height);

        onFullScreen = true;

        gameObject.guiText.text = instructionText;
        gameObject.guiText.fontSize = 40;
        gameObject.guiText.color = new Color32(255, 255, 255, 255); //initialcolor
        gameObject.guiTexture.pixelInset = scaleInset;
        gameObject.transform.position = scaledPosition;
    }

    private void rescaleInstruction()
    {
        gameObject.guiText.fontSize = 25;
        gameObject.guiText.text = initialText;
        onFullScreen = false;
        gameObject.guiTexture.pixelInset = noScaleInset;
        gameObject.transform.position = initialPosition;
    }

	// Use this for initialization
	void Start () {


        initialText = gameObject.guiText.text;
        initialPosition = gameObject.transform.position;
        gameObject.AddComponent<AudioSource>();
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
