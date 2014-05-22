using UnityEngine;
using System.Collections;

public class MilitarySpecialisation : Specialisation {

    private int troops = 0;

    public MilitarySpecialisation(GameObject hex, Vector3 pos) : base(hex, pos) { }

    public override int Cost
    {
        get
        {
            return 150;
        }
    }

    public int Troops
    {
        get
        {
            return troops;
        }
    }

    public void recruit()
    {
        troops += Level;
    }
}
