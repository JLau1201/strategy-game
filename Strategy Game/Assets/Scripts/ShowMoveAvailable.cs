using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMoveAvailable : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Image availableTile;

    public void ShowTile() {
        availableTile.gameObject.SetActive(true);
    }

    public void HideTile() {
        availableTile.gameObject.SetActive(false);
    }
}
