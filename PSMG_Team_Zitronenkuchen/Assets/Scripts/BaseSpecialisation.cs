using UnityEngine;
using System.Collections;

public class BaseSpecialisation : Specialisation
{
	private int troops = 0;
    private int recruitCounter = 0;

    // call base constructor
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

    public int RecruitCounter
    {
        get
        {
            return recruitCounter;
        }
        set
        {
            recruitCounter = value;
        }
    }

    // maybe needed later for not building instant
    //public override int BuildCounter
    //{
    //    get
    //    {
    //        return buildCounter;
    //    }
    //    set
    //    {
    //        buildCounter = value;
    //    }
    //}

    // increase troop count
    public void recruit()
    {
        troops += 5;
    }
}
