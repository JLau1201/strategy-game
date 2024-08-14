using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button abilityButton;
    [SerializeField] private Button endMoveButton;

    [SerializeField] private GameObject actionMenu;

    private void Awake() {
        Hide();

        attackButton.onClick.AddListener(() => {
            Player.Instance.SetTurnAction(Player.TurnAction.UnitAttack);
            Hide();
        });

        abilityButton.onClick.AddListener(() => {
            Player.Instance.SetTurnAction(Player.TurnAction.UnitAbility);
            Hide();
        });

        endMoveButton.onClick.AddListener(() => {
            Player.Instance.SetTurnAction(Player.TurnAction.Nothing);
            Hide();
        });
    }

    public void Show() {
        actionMenu.SetActive(true);
    }
    
    public void Hide() {
        actionMenu.SetActive(false);
    }
}
