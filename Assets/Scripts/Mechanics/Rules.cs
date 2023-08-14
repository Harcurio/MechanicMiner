using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Rules 
{
    public bool used = false;

    public new string name;
    public string comparator;
    public string valueComparator;
    public string effect;
    public string valueEffect;
    public string allToString;



    public void setValues(string name, string comparator, string valueComparator, string effect, string valueEffect)
    {
        this.name = name;
        this.comparator = comparator;
        this.valueComparator = valueComparator;
        this.effect = effect;
        this.valueEffect = valueEffect;
        this.used = true;
        this.allToString = name + "|" + comparator + "|" + valueComparator + "|" + effect +"|"+ valueEffect;
    }

    public void printValues()
    {
        Debug.Log(this.name);
        Debug.Log(this.comparator);
        Debug.Log(this.valueComparator);
        Debug.Log(this.effect);
        Debug.Log(this.valueEffect);


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
