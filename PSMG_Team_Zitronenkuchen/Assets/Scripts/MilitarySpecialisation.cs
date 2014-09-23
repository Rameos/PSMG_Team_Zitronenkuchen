using UnityEngine;
using System.Collections;

public class MilitarySpecialisation : Specialisation {

    private int troops = 0;
    private int recruitCounter = 0;
    private int buildCounter = 1;
    private int weaponType = 0;

    public const int LASER = 1;
    public const int PROTONS = 2;
    public const int EMP = 3;

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
        troops += 1;
    }

    public int WeaponType
    {
        get
        {
            return weaponType;
        }
        set
        {
            switch (value)
            {
                case 1: 
                    weaponType = LASER;
                    break;
                case 2:
                    weaponType = PROTONS;
                    break;
                case 3:
                    weaponType = EMP;
                    break;
                case 0:
                    weaponType = 0;
                    break;
                default:
                    weaponType = 0;
                    break;
            }
        }
    }

    public string WeaponName
    {
        get
        {
            switch(weaponType)
            {
                case 1:
                    return "LASER";
                case 2:
                    return "PROTONS";
                case 3:
                    return "EMP";
                default:
                    return "NONE";
            }
        }   
    }
}
