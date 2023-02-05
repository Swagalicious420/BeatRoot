using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RootController : MonoBehaviour
{
    public Snake player;

    [SerializeField] public Tilemap tilemap;

    [SerializeField] public RuleTile root;

    private RuleTile tileInstance;

    public static RootController instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        tilemap = GameObject.Find("Roots").GetComponent<Tilemap>();
        player = GameObject.Find("Snake").GetComponent<Snake>();
        tileInstance = Instantiate(root);
    }

    public void PlaceRoot()
    {
        if (tilemap != null && root != null) {
            var cell = tilemap.LocalToCell(player.transform.position);
           
            print(cell);
            print(tileInstance);
            print("Placeing stuff");
            tilemap.SetTile(cell, tileInstance);
        }

    }
}
