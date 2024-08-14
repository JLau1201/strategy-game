using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilitySO : ScriptableObject
{
    string abilityName;
    float abilityModifier;

    public abstract void Activate();
}
