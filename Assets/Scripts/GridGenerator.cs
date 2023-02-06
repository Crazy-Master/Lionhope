using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Cell tile;
    [SerializeField] private int gridXSize;
    [SerializeField] private int gridYSize;
    [SerializeField] private float tileXSize;
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private Transform gridStartPosition;

    public Cell[,] Tiles { get; private set; }
    public int XSize => gridXSize;
    public int YSize => gridYSize;

    private void Awake()
    {
        GenerateGrid();
    }
    private void GenerateGrid()
    {
        Tiles = new Cell[gridXSize, gridYSize];
        for(int x = 0; x < gridXSize; x++)
        {
            for (int y = 0; y < gridYSize; y++)
            {
                Cell newTile = Instantiate(tile,transform);
                float posX = gridStartPosition.position.x + (x * tileXSize - y * tileXSize) / 2f; 
                float posY = gridStartPosition.position.y + (x * tileXSize + y * tileXSize) / 3.4f; 
                newTile.transform.position = new Vector2(posX, posY);
                newTile.name = x + ", " + y;
                Tiles[x,y] = newTile;
            }
        }
    }
    public MergeableObject GetObject(Vector3Int position)
    {
        return Tiles[position.x, position.y].objectInCell;
    }
    public bool isFilledTile(Vector3 position)
    {
        Vector3Int cellPosition = tileMap.WorldToCell(position);
        if (tileMap.GetTile(cellPosition) == null)
        {
            return false;
        }
        return true;
    }
    public Vector3 GetCellCenter(Vector3 position)
    {
        Vector3Int cellPosition = tileMap.WorldToCell(position);
        return tileMap.GetCellCenterWorld(cellPosition);
    }
    public Vector3Int GetCell(Vector3 position)
    {
        Vector3Int cellPosition = tileMap.WorldToCell(position);
        cellPosition.x = Mathf.Clamp(cellPosition.x, 0, gridXSize - 1);
        cellPosition.y = Mathf.Clamp(cellPosition.y, 0, gridXSize - 1);
        return cellPosition;
    }
    public Vector3 GetWorldCellPosition(Vector3Int cellPosition)
    {
        Vector3 worldCellPosition = tileMap.CellToWorld(cellPosition);
        worldCellPosition += new Vector3(0, tileXSize / 3.4f, 0);
        return worldCellPosition;
    }
    public void SetObjectInCell(Vector3Int cell, MergeableObject mergeableObject)
    {
        Tiles[cell.x, cell.y].objectInCell = mergeableObject;
    }

    public void MoveObjectToCell(MergeableObject mergeableObject, Vector3Int newCell) 
    {
        if (GetObject(newCell) == null) 
        {
            SetObjectInCell(mergeableObject.currentCell, null);
            SetObjectInCell(newCell, mergeableObject);
            mergeableObject.currentCell = newCell;
        }
        else
        {
            Vector3Int oldCurrentCell = mergeableObject.currentCell;
            MergeableObject objectToSwapOut = Tiles[newCell.x, newCell.y].objectInCell;
            SetObjectInCell(newCell, mergeableObject);
            mergeableObject.currentCell = newCell;
            objectToSwapOut.currentCell = oldCurrentCell;
            SetObjectInCell(oldCurrentCell, objectToSwapOut);
            objectToSwapOut.MoveToCell(oldCurrentCell);
        }
    }
}
