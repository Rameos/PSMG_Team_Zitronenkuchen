using UnityEngine;
using System.Collections;

public class BaseSpecialisation : Specialisation
{

	private int troops = 0;
    private int buildCounter = 0;

    public BaseSpecialisation(GameObject hex, Vector3 pos) : base(hex, pos) { }

    public override int Cost
    {
        get
        {
            return 0;
        }
    }

    public int Troops
    {
        get
        {
            return troops;
        }
        set
        {
            troops = value;
        }
    }

    public override string type
    {
        get { return "base"; }
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

    public void recruit()
    {
        troops += 5;
    }
}
