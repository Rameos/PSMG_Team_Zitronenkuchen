using UnityEngine;
using System.Collections;

public class SelectionMenu : MonoBehaviour {

    public AudioClip selectSound;
    public AudioClip militaryGrowl;
    public AudioClip researchGrowl;
    public AudioClip economyGrowl;

    public bool selected; 

    private static int selectedRaceIdx = -1; // 0 for research, 1 for military, 2 for economy

    private Vector3 noScale = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 highlightScale = new Vector3 (0..1f, 0.1f, 0.1f);

    private bool alienSelected = false;

	// Use this for initialization
	void Start () {
	
	}

    void OnMouseEnter()
    {
        gameObject.transform.localScale = new Vector3(0.0f, 0.1f, 0.1f);
        highlightText();
        audio.PlayOneShot(selectSound);
    }

    void OnMouseUp()
    { 
        switch (gameObject.tag)
        {
            case "ResearchRace":
                if (selectedRaceIdx != 0) 
                {
                    selected = true;
                    selectedRaceIdx = 0;
                    highlightText();
                    rescaleAlien("MilitaryRace");
                    rescaleAlien("EconomyRace");
                    //audio.PlayOneShot(researchGrowl);
                }
                else
                {
                    rescaleAlien("ResearchRace");
                    selectedRaceIdx = -1;
                    selected = false;
                }
                break;
            case "MilitaryRace":
                if (selectedRaceIdx != 1)
                {
                    selected = true;
                    highlightText();
                    selectedRaceIdx = 1;
                    rescaleAlien("ResearchRace");
                    rescaleAlien("EconomyRace");
                    //audio.PlayOneShot(militaryGrowl); 
                } else {
                    rescaleAlien("MilitaryRace");
                    selectedRaceIdx = -1;
                    selected = false;
                }

                
                break;
            case "EconomyRace":
                if (selectedRaceIdx != 2)
                {
                    selected = true;
                    highlightText();
                    selectedRaceIdx = 2;
                    rescaleAlien("MilitaryRace");
                    rescaleAlien("ResearchRace");
                    //audio.PlayOneShot(economyGrowl);
                }
                else
                {
                    rescaleAlien("EconomyRace");
                    selectedRaceIdx = -1;
                    selected = false;
                }
                break;
        }
    }

    private void highlightText()
    {
        gameObject.guiText.color = Color.cyan;
        gameObject.guiText.text = "" + selectedRaceIdx;

    }

    private void scaleAlienBigger(tag)
    {
        GameObject alien = GameObject.FindGameObjectWithTag(tag);
        alien.transform.localScale = noScale;
    }

    private void rescaleAlien(string tag)
    {
        GameObject alien = GameObject.FindGameObjectWithTag(tag);
        alien.transform.localScale = noScale;
        alien.guiText.color = Color.white;
       
    }


    public static int getRaceType()
    {
        return selectedRaceIdx;
    }
    void OnMouseExit()
    {
        if (!selected)
        {
            rescaleAlien(gameObject.tag);
        }
        
    }


	
	// Update is called once per frame
	void Update () {
	
	}
}
