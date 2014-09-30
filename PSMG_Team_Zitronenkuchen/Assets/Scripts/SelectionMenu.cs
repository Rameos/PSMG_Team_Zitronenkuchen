using UnityEngine;
using System.Collections;

/**
 * This script is attached to the Selection_Menu scene and handels the selection of Alien races.
 **/
public class SelectionMenu : MonoBehaviour {

    public AudioClip selectSound;
    public AudioClip growl;

    public bool selected;

    public int raceIndex;

    public int width;
    public int height;

    // 1 for military, 2 for economy
    private static int selectedRaceIdx = -1; 

    private Vector3 noScale = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 highlightScale = new Vector3 (0.05f, 0.05f, 0.0f);

    private Rect scaleInsetMIL = new Rect (-10, 0, 329, 228);
    private Rect scaleInsetECO = new Rect(-10, 0, 240, 300);

    private Rect noScaleInsetMIL = new Rect(0, 0, 309, 208);
    private Rect noScaleInsetECO = new Rect(0, 0, 220, 280);

    private bool alienSelected = false;

    private float horizRatio = Screen.width / 800;
    private float vertRatio = Screen.height / 600;
 


	// Use this for initialization
	void Start () {
        GameObject militaryAlien = GameObject.Find("MilitaryAlienRace");
        GameObject economyAlien = GameObject.Find("EconomyAlienRace");
        militaryAlien.transform.position = new Vector3(0.10f,0.30f, 0);
        economyAlien.transform.position = new Vector3(0.70f,0.30f, 0);
	}

    void OnMouseEnter()
    {
        if (raceIndex != selectedRaceIdx)
        {
            if (raceIndex == 1)
            {
                gameObject.guiTexture.pixelInset = scaleInsetMIL;
            }
            else gameObject.guiTexture.pixelInset = scaleInsetECO;
            highlightText();
            audio.PlayOneShot(selectSound);
        }
        
    }

    // player selected a race. highlight the race(image and text). eventually de-highlight the previous selected race
    void OnMouseUp()
    { 
        switch (gameObject.tag)
        {
            case "MilitaryRace":
                if (selectedRaceIdx != raceIndex)
                {
                    selected = true;
                    gameObject.guiText.color = new Color32(218, 164, 59, 255);
                    scaleAlienBigger("MilitaryRace");
                    selectedRaceIdx = raceIndex;
                    CustomGameProperties.alienRace = selectedRaceIdx;
                    rescaleAlien("EconomyRace");
                    audio.PlayOneShot(growl); 
                } else {
                    rescaleAlien("MilitaryRace");
                    selectedRaceIdx = -1;
                    selected = false;
                }

                
                break;
            case "EconomyRace":
                if (selectedRaceIdx != raceIndex)
                {
                    selected = true;
                    gameObject.guiText.color = new Color32(218, 164, 59, 255);
                    scaleAlienBigger("EconomyRace");
                    selectedRaceIdx = raceIndex;
                    CustomGameProperties.alienRace = selectedRaceIdx;
                    rescaleAlien("MilitaryRace");
                    audio.PlayOneShot(growl);
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
        gameObject.guiText.color = new Color32(87, 192, 195, 255);

    }

    private void scaleAlienBigger(string tag)
    {
        GameObject alien = GameObject.FindGameObjectWithTag(tag);
        if (tag == "MilitaryRace")
        {
            alien.guiTexture.pixelInset = scaleInsetMIL;
        }
        else alien.guiTexture.pixelInset = scaleInsetECO;

    }
    private void rescaleAlien(string tag)
    {
        GameObject alien = GameObject.FindGameObjectWithTag(tag);
        if (tag == "MilitaryRace")
        {
            alien.guiTexture.pixelInset = noScaleInsetMIL;
        }
        else alien.guiTexture.pixelInset = noScaleInsetECO;
        alien.guiText.color = Color.white;
    }

    //returns index of selected race:  1 for military, 2 for economy
    public static int getRaceType()
    {
        return selectedRaceIdx;
    }

    void OnMouseExit()
    {
        if (raceIndex != selectedRaceIdx)
        {
            rescaleAlien(gameObject.tag);
        }
        
    }


	
	// Update is called once per frame
	void Update () {
	
	}
}
