using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    public event EventHandler<OnUnitSelectedEventArgs> OnUnitSelected;
    public class OnUnitSelectedEventArgs : EventArgs {
        public GameObject selectedUnit;
        public GameObject selectedTile;
    }

    private GameObject selectedUnit = null;
    private UnitSO selectedUnitSO;

    private void Start() {
        GameInputManager.instance.OnSelectInput += GameInputManager_OnSelectInput;
    }

    private void Awake() {
        instance = this;
    }

    // On select get the unit on cursor
    private void GameInputManager_OnSelectInput(object sender, System.EventArgs e) {
        GameObject unitOnCursor = Cursor.instance.GetUnitOnCursor();
        if (unitOnCursor != null) {
            HandleUnitSelected(unitOnCursor);
        }
    }


    // Do things when a unit is selected
    private void HandleUnitSelected(GameObject unitOnCursor) {
        selectedUnit = unitOnCursor;
        selectedUnitSO = selectedUnit.GetComponent<Unit>().GetUnitSO();
        GameObject selectedTile = Cursor.instance.GetTileOnCursor();

        // OnSelectUnit event maybe?
        OnUnitSelected?.Invoke(this, new OnUnitSelectedEventArgs {
            selectedUnit = this.selectedUnit,
            selectedTile = selectedTile,
        });
    }
}
