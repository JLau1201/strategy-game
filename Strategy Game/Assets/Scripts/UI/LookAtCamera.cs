using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // For UI elements to appear 2d
    private void Awake() {
        transform.forward = Camera.main.transform.forward;
    }
}
