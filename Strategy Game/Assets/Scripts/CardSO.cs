using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base card class SO
public abstract class CardSO : ScriptableObject
{
    // Common card properties
    [Header("Card Properties")]
    public string cardName;
    public Sprite splashArt;
    public int cost;

    [Header("Game Objects")]
    public GameObject unitPrefab; // For Units
    public GameObject cardPrefab;


    public abstract void PlayCard();
}
