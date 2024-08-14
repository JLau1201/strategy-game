using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    // Camera move
    [SerializeField] private float moveSpeed;
    [SerializeField] private float edgeBuffer;
    // Camera zoom
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoomSize;
    [SerializeField] private float maxZoomSize;

    private Vector2Int screenSize;
    private Vector3 targetPosition;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake() {
        screenSize = new Vector2Int(Screen.width, Screen.height);
    }

    private void Update() {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        
        // Move camera with mouse
        if (mousePos.x < edgeBuffer) {
            targetPosition += Vector3.left * moveSpeed * Time.deltaTime;
        } else if (mousePos.x > screenSize.x - edgeBuffer) {
            targetPosition += Vector3.right * moveSpeed * Time.deltaTime;
        }

        if (mousePos.y < edgeBuffer) {
            targetPosition += Vector3.down * moveSpeed * Time.deltaTime;
        } else if (mousePos.y > screenSize.y - edgeBuffer) {
            targetPosition += Vector3.up * moveSpeed * Time.deltaTime;
        }

        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 0.1f);

        // Camera zoom
        Vector2 scrollInput = Mouse.current.scroll.ReadValue();
        float scrollDelta = scrollInput.y;
        if (scrollDelta != 0) {
            Camera.main.orthographicSize -= scrollDelta * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoomSize, maxZoomSize);
        }
    }
}
