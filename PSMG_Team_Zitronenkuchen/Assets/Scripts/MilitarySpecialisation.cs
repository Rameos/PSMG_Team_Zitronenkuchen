using UnityEngine;
using System.Collections;

public class MilitarySpecialisation : Specialisation {

    private int troops = 0;
    private int buildCounter = 0;

    public MilitarySpecialisation(GameObject hex, Vector3 pos) : base(hex, pos) { }

    public override int Cost
    {
        get
        {
            return 150;
        }
    }

    public override string type
    {
        get { return "mil"; }
    }

    public override int BuildCounter
    {
        get
        {
            return buildCounter;
        }
        set
        {
            buildCounter = value;
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
        troops += 5;
    }
}
