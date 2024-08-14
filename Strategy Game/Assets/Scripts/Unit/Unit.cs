using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    [SerializeField] private UnitDataSO unitDataSO;
    [SerializeField] private UnitHealthBarUI healthBarUI;

    // Unit data
    private float maxHealth;

    public float Health { get; private set; }
    public float Attack { get; private set; }
    public int MoveRange { get; private set; }
    public int AttackRange { get; private set; }
    public int AbilityRange { get; private set; }
    public int AbilityRadius { get; private set; }
    public float Cooldown { get; private set; }

    private void Awake() {
        UnitSetup();
    }

    private void UnitSetup() {
        maxHealth = unitDataSO.health;
        Health = maxHealth;
        Attack = unitDataSO.attack;
        MoveRange = unitDataSO.moveRange;
        AttackRange = unitDataSO.attackRange;
        AbilityRange = unitDataSO.abilityRange;
        AbilityRadius = unitDataSO.abilityRadius;
        Cooldown = 0;
    }

    public void ModifyHealth(float modifier) {
        Health += modifier;
        healthBarUI.UpdateHealthBar(Health / maxHealth);
    }
}
