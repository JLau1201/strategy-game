using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    public event EventHandler<OnUnitSelectedEventArgs> OnUnitSelected;
    public event EventHandler OnUnitUnselected;

    public class OnUnitSelectedEventArgs : EventArgs {
        public GameObject selectedUnit;
        public GameObject selectedTile;
    }

    private GameObject selectedUnit = null;
    private UnitSO selectedUnitSO;
    private bool canSelect = true;

    // Track what player is doing
    private Selected currentSelected;
    private enum Selected {
        Nothing,
        AllyUnit,
        EnemyUnit,
        Terrain,
    }

    private void Start() {
        GameInputManager.instance.OnSelectInput += GameInputManager_OnSelectInput;
    }

    private void Awake() {
        instance = this;
    }

    // On select get the unit on cursor
    private void GameInputManager_OnSelectInput(object sender, System.EventArgs e) {
        if (!canSelect) return;
        switch (currentSelected) {
            case Selected.Nothing:
                GameObject unitOnCursor = Cursor.instance.GetUnitOnCursor();
                if (unitOnCursor != null) {
                    HandleUnitSelected(unitOnCursor);
                }
                break;
            case Selected.AllyUnit:
                OnUnitUnselected?.Invoke(this, EventArgs.Empty);
                currentSelected = Selected.Nothing;
                break;
            case Selected.EnemyUnit:
                break;
            case Selected.Terrain:
                break;
        }
    }


    // Do things when a unit is selected
    private void HandleUnitSelected(GameObject unitOnCursor) {
        currentSelected = Selected.AllyUnit;

        selectedUnit = unitOnCursor;
        selectedUnitSO = selectedUnit.GetComponent<Unit>().GetUnitSO();
        GameObject selectedTile = Cursor.instance.GetTileOnCursor();

        // OnSelectUnit event maybe?
        OnUnitSelected?.Invoke(this, new OnUnitSelectedEventArgs {
            selectedUnit = this.selectedUnit,
            selectedTile = selectedTile,
        });
    }

    public void SetCanSelect(bool canSelect) {
        this.canSelect = canSelect;
    }
}
