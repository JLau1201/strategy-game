using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class UnitSO : ScriptableObject
{
    public string unitName;
    
    public float health;
    
    public float attack;
    
    public int range;

    public GameObject prefab;
}
