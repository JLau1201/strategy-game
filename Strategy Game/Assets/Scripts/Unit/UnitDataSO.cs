using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Card/UnitData")]
public class UnitDataSO : CardSO
{
    public enum AbilityType {
        Circle,
    }

    // Unit stats
    [Header("Base Stats")]
    public int health;
    public int attack;
    public int moveRange;
    public int attackRange;

    [Header("Ability Stats")]
    public int abilityRange;
    public int abilityRadius;
    public int abilityMultiplier;
    public int cooldown;
    public AbilityType abilityType;
    public AbilitySO abilitySO;

    // Summon Card
    public override void PlayCard() {
        Player.Instance.SetTurnAction(Player.TurnAction.UnitSummon);
    }
}
