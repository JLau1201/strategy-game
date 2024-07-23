using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private float x;
    private float y;
    private GameObject tileGO;

    public float X {
        get { return x; }
        set { x = value; }
    }

    public float Y {
        get { return y; }
        set { y = value; }
    }

    public GameObject TileGO {
        get { return tileGO; }
        set { tileGO = value; }
    }

    // Constructor
    public Tile (float x, float y, GameObject tileGO) {
        this.x = x;
        this.y = y;
        this.tileGO = tileGO;
    }
}
