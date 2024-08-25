using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{

    public static DeckManager Instance { get; private set; }

    [SerializeField] private Transform deckHolder;
    [SerializeField] private GameObject cardBackPrefab;
    [SerializeField] private Button button;
    
    // Temp serialize field to populate deck
    [SerializeField] private List<CardSO> deckList = new List<CardSO>();

    private void Awake() {
        Instance = this;

        for (int i = 0; i < deckList.Count; i++) {
            Instantiate(cardBackPrefab, deckHolder);
        }
        button.onClick.AddListener(() => {
            DrawCard();
        });
    }

    // Draw card
    private void DrawCard() {
        Destroy(deckHolder.GetChild(0).gameObject);

        CardSO drawnCard = deckList[deckList.Count - 1];
        deckList.RemoveAt(deckList.Count - 1);

        HandManager.Instance.AddCard(drawnCard);
    }

    // Shuffle Deck
    private void ShuffleDeck() {
        System.Random rng = new System.Random();

        int n = deckList.Count;
        while(n > 1) {
            n--;
            int k = rng.Next(n + 1);

            CardSO card = deckList[k];
            deckList[k] = deckList[n];
            deckList[n] = card;
        }
    }
}
