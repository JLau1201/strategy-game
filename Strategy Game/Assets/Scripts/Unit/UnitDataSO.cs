using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "UnitData")]
public class UnitDataSO : ScriptableObject
{

    public enum AbilityType {
        Circle,
    }

    // Unit stats
    public string unitName;
    
    public int health;
    
    public int attack;
    
    public int moveRange;

    public int attackRange;

    public int abilityRange;

    public int abilityRadius;

    public int cooldown;

    public AbilityType abilityType;

    public GameObject prefab;
}
