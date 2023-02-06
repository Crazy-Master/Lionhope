using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class LinksContainer : MonoBehaviour
{
    public static LinksContainer instance;
    [SerializeField] private GridGenerator grid;
    [SerializeField] private Merger merger;

    public GridGenerator Grid => grid;
    public Merger Merger => merger;

    public void Awake()
    {
        instance = this;
    }
}
