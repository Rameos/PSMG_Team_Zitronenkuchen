using UnityEngine;
using System.Collections;

/**
 * This script is assigned to the BaseSpecialisation of each player
 **/
public class BaseSpecialisation : Specialisation
{
	private int troops = 0;
    private int recruitCounter = 0;
    private int buildCounter = 0;
    private int weaponType = 0;

    public const int LASER = 1;
    public const int PROTONS = 2;
    public const int EMP = 3;

    // call constructor of base class
    public BaseSpecialisation(GameObject hex, Vector3 pos) : base(hex, pos) { }

    // bases are built autmatically at the beginning and cannot be built by the player later on. thus they cost no ressources
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
            switch (weaponType)
            {
                case LASER:
                    return "LASER";
                case PROTONS:
                    return "PROTONS";
                case EMP:
                    return "EMP";
                default:
                    return "NONE";
            }
        }
    }
}
