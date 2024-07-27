using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{

    public static GameInputManager instance { get; private set; }

    public event EventHandler OnSelectInput;

    private PlayerInputActions inputActions;

    private void Awake() {
        instance = this;
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        inputActions.Player.Select.performed += Select_performed;
    }

    private void Select_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnSelectInput?.Invoke(this, EventArgs.Empty);
    }
}
