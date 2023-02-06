using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class MergeableObject : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 0.1f;
    [SerializeField] private string currentObjectType;
    [SerializeField] private int currentObjectLevel;

    [HideInInspector] public Vector3 nearestCellWorldPosition;
    [HideInInspector] public Vector3Int nearestCell;
    [HideInInspector] public Vector3Int oldNearestCell;
    [HideInInspector] public Vector3Int currentCell;

    private GridGenerator grid;
    private Merger merger;
    private bool isDrag;

    public string ObjectType => currentObjectType;
    public int ObjectLevel => currentObjectLevel;

    public Action highlightON; 
    public Action highlightOFF;


    void Start()
    {
        grid = LinksContainer.instance.Grid;
        merger = LinksContainer.instance.Merger;
        nearestCellWorldPosition = grid.GetCellCenter(gameObject.transform.position);
        transform.position = nearestCellWorldPosition;
        currentCell = grid.GetCell(nearestCellWorldPosition);
        oldNearestCell = currentCell;
        nearestCell = currentCell;
        grid.SetObjectInCell(currentCell, this);
    }
    public void FixedUpdate() 
    {
        Vector3 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (isDrag && grid.GetCellCenter(mousePosition) != nearestCellWorldPosition && grid.isFilledTile(mousePosition))
        {
            nearestCell = grid.GetCell(mousePosition);
            oldNearestCell = grid.GetCell(nearestCellWorldPosition);
            nearestCellWorldPosition = grid.GetCellCenter(mousePosition);
            OnChangeLocation();
        }
        transform.position = Vector3.Lerp(transform.position, nearestCellWorldPosition, lerpSpeed);
    }
    public void OnChangeLocation()
    {
        HighLightMergeable();
    }
    public void OnMouseDown()
    {
        isDrag = true;
        grid.SetObjectInCell(currentCell, null);
        highlightON();
    }
    public void OnMouseUp() 
    {
        isDrag = false;
        highlightOFF();
        if (merger.CanMerge(this, nearestCell))
        {
            merger.Merge(this, merger.GetConnectedSame(grid.GetObject(nearestCell)));
        }
        else
        {
            grid.MoveObjectToCell(this, grid.GetCell(nearestCellWorldPosition));
        }
    }
    public void MoveToCell(Vector3Int newCell)
    {
        nearestCellWorldPosition = grid.GetWorldCellPosition(newCell);
    }
    public void HighLightMergeable()
    {
        if (merger.CanMerge(this, oldNearestCell))
        {
            Highlighter.HighLightObjectsOff(merger.GetConnectedSame(grid.GetObject(oldNearestCell)));
        }
        if (merger.CanMerge(this, nearestCell))
        {
            Highlighter.HighLightObjectsOn(merger.GetConnectedSame(grid.GetObject(nearestCell)));
        }
    }
}
