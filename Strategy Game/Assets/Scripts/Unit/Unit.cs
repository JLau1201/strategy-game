using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField] private UnitDataSO unitDataSO;
    [SerializeField] private UnitHealthBarUI healthBarUI;

    // Unit data
    public float MaxHealth { get; private set; }
    public int Cost { get; private set; }
    public float Health { get; private set; }
    public float Attack { get; private set; }
    public int MoveRange { get; private set; }
    public int AttackRange { get; private set; }
    public int AbilityRange { get; private set; }
    public int AbilityRadius { get; private set; }
    public int AbilityMultiplier { get; private set; }
    public float Cooldown { get; private set; }

    private void Awake() {
        UnitSetup();
    }

    private void UnitSetup() {
        Cost = unitDataSO.cost;
        MaxHealth = unitDataSO.health;
        Health = MaxHealth;
        Attack = unitDataSO.attack;
        MoveRange = unitDataSO.moveRange;
        AttackRange = unitDataSO.attackRange;
        AbilityRange = unitDataSO.abilityRange;
        AbilityRadius = unitDataSO.abilityRadius;
        AbilityMultiplier = unitDataSO.abilityMultiplier;
        Cooldown = 0;
    }

    public void ModifyHealth(float modifier) {
        Health += modifier;
        healthBarUI.UpdateHealthBar(Health / MaxHealth);
    }

    public UnitDataSO GetUnitDataSO() {
        return unitDataSO;
    }
}
