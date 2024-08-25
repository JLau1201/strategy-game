using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilitySO : ScriptableObject
{
    public string abilityName;
    public float abilityModifier;

    public abstract void Activate(GameObject target, float abilityDamage);
}
