using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rules : MonoBehaviour
{
    public bool used = false;

    public string name;
    public string comparator;
    public string valueComparator;
    public string effect;
    public string valueEffect;



    public void setValues(string name, string comparator, string valueComparator, string effect, string valueEffect)
    {
        this.name = name;
        this.comparator = comparator;
        this.valueComparator = valueComparator;
        this.effect = effect;
        this.valueEffect = valueEffect;
        this.used = true;
    }


    public void setComparator(string comparator)
    {
        this.comparator = comparator;
    }

    public void setValueComparator(string valueComparator)
    {
        this.valueComparator = valueComparator;
    }

    public void setEffect(string effect)
    {
        this.effect = effect;
    }

    public void setValueEffect(string valueEffect)
    {
        this.valueEffect = valueEffect;
    }
}
