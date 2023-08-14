using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Conditions
{

    public int numberOfConditions = 5;
    public int conditionsNumbers = 3;

    /*we will start with the conditions made for numbers and at the end made for bools*/


    public enum conditions
    {
        biggerThan,
        lessThan,
        equalThan,
        isTrue,
        isFalse,
        COUNT
    }



    public bool applyCondition(int var1, conditions x, int var2)
    {
        switch ((int)x)
        {
            case 0:
                if (var1 > var2)
                    return true;
                break;
            case 1:
                if (var1 < var2)
                    return true;
                break;
            case 2:
                if (var1 == var2)
                    return true;
                break;
            default:
                return false;
        }
        return false;
    }

    public bool applyCondition(float var1, conditions x, float var2)
    {
        switch ((int)x)
        {
            case 0:
                if (var1 > var2)
                    return true;
                break;
            case 1:
                if (var1 < var2)
                    return true;
                break;
            case 2:
                if (var1 == var2)
                    return true;
                break;
            default:
                return false;
        }
        return false;
    }

    public bool applyCondition(bool var1, conditions x)
    {
        switch ((int)x)
        {
            case 3:
                if (var1)
                    return true;
                break;
            case 4:
                if (!var1)
                    return true;
                break;
            default:
                return false;
        }
        return false;
    }

}
