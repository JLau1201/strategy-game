using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{

    public static GameInputManager Instance { get; private set; }

    public event EventHandler OnSelectInput;
    public event EventHandler OnCancelInput;
    public event EventHandler OnToggleCursorInput;

    private PlayerInputActions inputActions;

    private void Awake() {
        Instance = this;
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        inputActions.Player.Select.performed += Select_performed;
        inputActions.Player.Cancel.performed += Cancel_performed;
        inputActions.Player.ToggleCursor.performed += ToggleCursor_performed;
    }

    private void ToggleCursor_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnToggleCursorInput?.Invoke(this, EventArgs.Empty);
    }

    private void Cancel_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnCancelInput?.Invoke(this, EventArgs.Empty);
    }

    private void Select_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnSelectInput?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetCursorInput() {
        return inputActions.Player.MoveCursor.ReadValue<Vector2>();
    }
}
