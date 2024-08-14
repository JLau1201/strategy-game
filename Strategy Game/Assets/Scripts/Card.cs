using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Vector3 originalPos;
    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
    }


    public void OnPointerEnter(PointerEventData eventData) {
        rectTransform.anchoredPosition = new Vector2(originalPos.x, 250);
    }

    public void OnPointerExit(PointerEventData eventData) {
        rectTransform.anchoredPosition = originalPos;
    }
}
