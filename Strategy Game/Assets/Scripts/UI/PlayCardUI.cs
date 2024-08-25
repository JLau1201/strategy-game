using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCardUI : BaseUI
{
    public static PlayCardUI Instance { get; private set; }

    private void Awake() {
        Instance = this;
        Hide();
    }
}
