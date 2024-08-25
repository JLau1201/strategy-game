using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // Camera zoom
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoomSize;
    [SerializeField] private float maxZoomSize;

    private bool isMovingCam = false;
    private Vector3 mouseOrigin;


    private void Start() {
        GameInputManager.Instance.OnMoveCameraStart += GameInputManager_OnMoveCameraStart;
        GameInputManager.Instance.OnMoveCameraCancel += GameInputManager_OnMoveCameraCancel;
    }

    private void GameInputManager_OnMoveCameraStart(object sender, System.EventArgs e) {
        mouseOrigin = GetMousePosition;
        isMovingCam = true;
        Cursor.visible = false;
    }
    
    private void GameInputManager_OnMoveCameraCancel(object sender, System.EventArgs e) {
        isMovingCam = false;
        Cursor.visible = true;
    }

    private void LateUpdate() {

        if (isMovingCam) {
            Vector3 mouseDelta = GetMousePosition - transform.position;
            transform.position = mouseOrigin - mouseDelta;
        }

        // Camera zoom
        Vector2 scrollInput = Mouse.current.scroll.ReadValue();
        float scrollDelta = scrollInput.y;
        if (scrollDelta != 0) {
            Camera.main.orthographicSize -= scrollDelta * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoomSize, maxZoomSize);
        }
    }

    private Vector3 GetMousePosition => Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
}
