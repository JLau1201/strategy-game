using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;

    public void UpdateHealthBar(float currentHealthPercent) {
        healthBarImage.fillAmount = currentHealthPercent;
    }
}
