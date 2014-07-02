using UnityEngine;
using System.Collections;

public class SelectionMenu : MonoBehaviour {

    public AudioClip selectSound;
    public AudioClip growl;

    public bool selected;

    public int raceIndex;

    private static int selectedRaceIdx = -1; // 0 for research, 1 for military, 2 for economy

    private Vector3 noScale = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 highlightScale = new Vector3 (0.05f, 0.05f, 0.0f);

    private Rect scaleInset = new Rect(-10, 0, 240, 300);

    private Rect noScaleInset  = new Rect(0, 0, 220, 280);

    private bool alienSelected = false;

    private float horizRatio = Screen.width / 800;
    private float vertRatio = Screen.height / 600;
 


	// Use this for initialization
	void Start () {
       // GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
	}

    void OnMouseEnter()
    {
        if (raceIndex != selectedRaceIdx)
        {
            gameObject.guiTexture.pixelInset = scaleInset;
            highlightText();
            audio.PlayOneShot(selectSound);
        }
        
    }

    void OnMouseUp()
    { 
        switch (gameObject.tag)
        {
            case "ResearchRace":
                if (selectedRaceIdx != raceIndex) 
                {
                    selected = true;
                    selectedRaceIdx = raceIndex;
                    gameObject.guiText.color = new Color32(218, 164, 59, 255);
                    scaleAlienBigger("ResearchRace");
                    rescaleAlien("MilitaryRace");
                    rescaleAlien("EconomyRace");
                    audio.PlayOneShot(growl);
                }
                else
                {
                    rescaleAlien("ResearchRace");
                    selectedRaceIdx = -1;
                    selected = false;
                }
                break;
            case "MilitaryRace":
                if (selectedRaceIdx != raceIndex)
                {
                    selected = true;
                    gameObject.guiText.color = new Color32(218, 164, 59, 255);
                    scaleAlienBigger("MilitaryRace");
                    selectedRaceIdx = raceIndex;
                    rescaleAlien("ResearchRace");
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
                    rescaleAlien("MilitaryRace");
                    rescaleAlien("ResearchRace");
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
        alien.guiTexture.pixelInset = scaleInset;

    }
    private void rescaleAlien(string tag)
    {
        GameObject alien = GameObject.FindGameObjectWithTag(tag);
        alien.guiTexture.pixelInset = noScaleInset;
        alien.guiText.color = Color.white;
    }

    //returns index of selected race:  0 for research, 1 for military, 2 for economy
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
