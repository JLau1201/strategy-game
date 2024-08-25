using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoDisplayUI : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI maxHealthText;
    [SerializeField] private TextMeshProUGUI moveRangeText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI attackRangeText;

    [Header("Ability Stats")]
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI abilityRangeText;
    [SerializeField] private TextMeshProUGUI abilityRadiusText;

    [Header("Misc.")]
    [SerializeField] private Image splashArt;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        Player.Instance.OnUnitInfoDisplayStart += Player_OnUnitInfoDisplayStart;
    }

    private void Player_OnUnitInfoDisplayStart(object sender, System.EventArgs e) {
        GameObject selectedUnit = MapManager.Instance.GetUnit(CursorInput.Instance.GetCursorPosition());

        if (selectedUnit == null) return;

        //GetData(selectedUnit);

        rectTransform.anchoredPosition = new Vector2(-200, 0);
    }

    private void GetData(GameObject selectedUnit) {
        Unit unit = selectedUnit.GetComponent<Unit>();

        costText.text = unit.Cost.ToString();
        // Health as health bar with text in middle
        healthText.text = unit.Health.ToString();
        maxHealthText.text = unit.MaxHealth.ToString();

        moveRangeText.text = unit.MoveRange.ToString();
        attackText.text = unit.Attack.ToString();
        attackRangeText.text = unit.AttackRange.ToString();
        abilityRangeText.text = unit.AttackRange.ToString();
        abilityRadiusText.text = unit.AbilityRadius.ToString();
        cooldownText.text = unit.Cooldown.ToString();
    }
}
