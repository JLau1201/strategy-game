using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    // Unit movement events
    public event EventHandler<OnUnitSelectedEventArgs> OnUnitMoveStart;
    public event EventHandler OnUnitMoveComplete;
    public event EventHandler OnUnitMoveCancelled;

    // Unit attack events
    public event EventHandler<OnUnitSelectedEventArgs> OnUnitAttackStart;
    public event EventHandler OnUnitAttackComplete;
    public event EventHandler OnUnitAttackCancelled;

    // Unit ability events
    public event EventHandler<OnUnitSelectedEventArgs> OnUnitAbilityStart;
    public event EventHandler OnUnitAbilityComplete;
    public event EventHandler OnUnitAbilityCancelled;

    // Unit Summon events
    public event EventHandler<OnCardPlayedEventArgs> OnUnitSummonStart;
    public event EventHandler OnUnitSummonComplete;
    public event EventHandler OnUnitSummonCancelled;

    // Unit display events
    public event EventHandler OnUnitInfoDisplayStart;

    // Event args for unit action events
    private OnUnitSelectedEventArgs onUnitSelectedEventArgs;

    public class OnUnitSelectedEventArgs : EventArgs {
        public GameObject selectedUnit;
        public GameObject selectedTile;
    }
    
    // Event args for card actions
    public class OnCardPlayedEventArgs : EventArgs {
        public CardSO cardSO;
    }

    private GameObject selectedTile;
    private GameObject selectedUnit = null;
    private Vector3 selectedUnitPos;
    private bool canSelect = true;
    private CardSO selectedCard;

    // Track what player is doing
    private TurnAction currTurnAction;
    public enum TurnAction {
        Nothing,        // CursorInput canMoveCursor Enabled : Player canSelect Enabled
        UnitMove,       // CursorInput canMoveCursor Enabled : Player canSelect Enabled - Disable during animation
        UnitMenu,       // CursorInput canMoveCursor Disabled : Player canSelect Disabled
        UnitAttack,     // CursorInput canMoveCursor Enabled : CursorInput IsAttacking Enabled : Player canSelect Enabled - Disable during animation
        UnitAbility,    // CursorInput canMoveCursor Enabled : CursorInput IsAttacking Enabled : Player canSelect Enabled - Disable during animation
        UnitSummon,     // CursorInput canMoveCursor Enabled : Player canSelect Enabled
        Cards,          // Disable Everything
    }
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        GameInputManager.Instance.OnSelectInput += GameInputManager_OnSelectInput;
        GameInputManager.Instance.OnCancelInput += GameInputManager_OnCancelInput;
    }

    // Unit action rollbacks
    // On Cancel Input revert back to prior unit action 
    // For TurnAction Nothing -> display unit information
    private void GameInputManager_OnCancelInput(object sender, EventArgs e) {
        if (!canSelect && currTurnAction != TurnAction.UnitMenu) return;

        switch (currTurnAction) {
            case TurnAction.Nothing:
                // Check for unit -> display unit info
                OnUnitInfoDisplayStart?.Invoke(this, EventArgs.Empty);

                break;
            case TurnAction.UnitMove:
                // Rollback to Nothing turn action
                OnUnitMoveCancelled?.Invoke(this, EventArgs.Empty);

                SetTurnAction(TurnAction.Nothing);
                break;
            case TurnAction.UnitMenu:
                // Rollback to UnitMove turn action
                selectedUnit.GetComponent<ActionMenuUI>().Hide();
                // Move unit back to position when selected
                selectedUnit.transform.position = selectedUnitPos;

                SetTurnAction(TurnAction.UnitMove);
                break;

            case TurnAction.UnitAttack:
                OnUnitAttackCancelled?.Invoke(this, EventArgs.Empty);
                // Show ActionMenu
                SetTurnAction(TurnAction.UnitMenu);
                break;
            case TurnAction.UnitAbility:
                OnUnitAbilityCancelled?.Invoke(this, EventArgs.Empty);
                // Show ActionMenu
                SetTurnAction(TurnAction.UnitMenu);
                break;
            case TurnAction.UnitSummon:
                OnUnitSummonCancelled?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    // Do action when something is selected in different turn action
    private void GameInputManager_OnSelectInput(object sender, System.EventArgs e) {
        if (!canSelect) return;

        switch (currTurnAction) {
            case TurnAction.Nothing:
                // Check if a unit is selected
                GameObject unitOnCursor = MapManager.Instance.GetUnit(CursorInput.Instance.GetCursorPosition());
                if (unitOnCursor != null) {
                    HandleUnitSelected(unitOnCursor);
                }
                break;
            case TurnAction.UnitMove:
                // Complete unit movement
                OnUnitMoveComplete?.Invoke(this, EventArgs.Empty);
                
                break;
            case TurnAction.UnitAttack:
                // Check if an enemy exists on tile -> attack it
                if(MapManager.Instance.GetUnit(CursorInput.Instance.GetCursorPosition()) != null) {
                    OnUnitAttackComplete?.Invoke(this, EventArgs.Empty);
                }
                break;
            case TurnAction.UnitAbility:
                OnUnitAbilityComplete?.Invoke(this, EventArgs.Empty);

                break;
            case TurnAction.UnitSummon:
                OnUnitSummonComplete?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    // Handle when a unit is selected
    private void HandleUnitSelected(GameObject unitOnCursor) {
        // Assign the unit and tile selected 
        selectedUnit = unitOnCursor;
        selectedTile = MapManager.Instance.GetTile(CursorInput.Instance.GetCursorPosition());

        // Store position when unit is selected for rollback
        selectedUnitPos = selectedUnit.transform.position;

        // Create event args for unit actions
        onUnitSelectedEventArgs = new OnUnitSelectedEventArgs {
            selectedUnit = selectedUnit,
            selectedTile = selectedTile
        };

        // Change turn action to unit move
        SetTurnAction(TurnAction.UnitMove);
    }

    // Change whether the player can select anything
    public void SetCanSelect(bool canSelect) {
        this.canSelect = canSelect;
    }

    // Change the turn action from other scripts
    public void SetTurnAction(TurnAction turnAction) {
        currTurnAction = turnAction;
        // Set Player canSelect/CursorInput canMoveCursor
        switch (currTurnAction) {
            case TurnAction.Nothing:
                canSelect = true;
                CursorInput.Instance.SetCanMoveCursor(true);
                break;
            case TurnAction.UnitMove:
                canSelect = true;
                CursorInput.Instance.SetCanMoveCursor(true);
                OnUnitMoveStart?.Invoke(this, onUnitSelectedEventArgs);
                break;
            case TurnAction.UnitMenu:
                selectedUnit.GetComponent<ActionMenuUI>().Show();
                canSelect = false;
                CursorInput.Instance.SetCanMoveCursor(false);
                break;
            case TurnAction.UnitAttack:
                canSelect = true;
                CursorInput.Instance.SetCanMoveCursor(true);
                // CursorInput isAttacking is set in UnitAttackManager
                OnUnitAttackStart?.Invoke(this, onUnitSelectedEventArgs);
                break;
            case TurnAction.UnitAbility:
                canSelect = true;
                CursorInput.Instance.SetCanMoveCursor(true);
                // CursorInput isAttacking is set in UnitAttackManager
                OnUnitAbilityStart?.Invoke(this, onUnitSelectedEventArgs);
                break;
            case TurnAction.UnitSummon:
                canSelect = true;
                CursorInput.Instance.SetCanMoveCursor(true);
                OnUnitSummonStart?.Invoke(this, new OnCardPlayedEventArgs {
                    cardSO = selectedCard
                });
                break;
            case TurnAction.Cards:
                canSelect = false;
                CursorInput.Instance.SetCanMoveCursor(false);
                // Maybe set isAttacking to false idk
                break;
        }
    }

    // Set card when clicking on it in hand
    public void SetSelectedCard(CardSO cardSO) {
        selectedCard = cardSO;
    }
}
