using UnityEngine;
using System.Collections;

public class MilitarySpecialisation : Specialisation {

    private int troops = 0;
    private int recruitCounter = 0;

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
        set
        {
            troops = value;
        }
    }

    public override string type
    {
        get { return "mil"; }
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

    public void recruit()
    {
        troops += 5;
    }
}
