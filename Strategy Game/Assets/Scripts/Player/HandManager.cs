using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HandManager : MonoBehaviour
{

    public static HandManager Instance { get; private set; }

    [SerializeField] private Transform cardHolder;

    private void Awake() {
        Instance = this;
    }

    public void AddCard(CardSO card) {
        Instantiate(card.cardPrefab, cardHolder);
    }
}
