using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitSO unitSO;

    public UnitSO GetUnitSO() {
        return unitSO;
    }
}
