using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

    [SerializeField] private CardSO cardSO;
    private RectTransform rectTransform; 
    private Canvas canvas;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }


    // Mouse down on card -> drag card
    public void OnPointerDown(PointerEventData eventData) {
        Player.Instance.SetTurnAction(Player.TurnAction.Cards);
        Player.Instance.SetSelectedCard(cardSO);
        PlayCardUI.Instance.Show();
    }

    // Mouse exit on card -> release card
    public void OnPointerUp(PointerEventData eventData) {
        Player.Instance.SetTurnAction(Player.TurnAction.Nothing);
        PlayCardUI.Instance.Hide();
        Destroy(gameObject);
        cardSO.PlayCard();
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, canvas.worldCamera, out localPoint);

        rectTransform.anchoredPosition = localPoint;
    }
}
