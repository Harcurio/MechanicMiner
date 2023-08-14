using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    public enum effects
    {
        add,
        subtract,
        multiply,
        divide,
        residue,
        change
    }

    public int applyEffect(int var, effects x, int quantity)
    {
        switch ((int)x)
        {
            case 0:
                return var + quantity;
            case 1:
                return var - quantity;
            case 2:
                return var * quantity;
            case 3:
                if (quantity == 0)
                    return -1;
                return var / quantity;
            case 4:
                return var % quantity;
            default:
                break;
        }
        return -1;
    }

    public float applyEffect(float var, effects x, float quantity)
    {
        switch ((int)x)
        {
            case 0:
                return (float)(var + quantity);
            case 1:
                return (float)(var - quantity);
            case 2:
                return (float)(var * quantity);
            case 3:
                if (quantity == 0)
                    return -1;
                return (float)(var / quantity);
            case 4:
                return (float)(var % quantity);
            default:
                break;
        }
        return -1;
    }

    public bool applyEffect(bool var, effects x)
    {
        if ((int)x == 5)
        {
            if (var)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }





}