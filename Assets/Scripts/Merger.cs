using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class Type
{
    public string name;
    public List<GameObject> prefabs;
}
public class Merger : MonoBehaviour
{
    [SerializeField] private GridGenerator grid;
    [SerializeField] private List<Type> types;

    private Vector3Int[] connectedCellsOffset = new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0) };
    public void Start()
    {
        grid = LinksContainer.instance.Grid;
    }
    public bool IsSame(MergeableObject obj1, MergeableObject obj2) 
    {
        return obj1 != null && obj2 != null && obj1.ObjectType == obj2.ObjectType && obj1.ObjectLevel == obj2.ObjectLevel;
    }
    public List<MergeableObject> GetNearestConnected(MergeableObject obj) 
    {
        List<MergeableObject> connected = new List<MergeableObject>(4);
        Vector3Int currentCell = obj.currentCell;

        foreach (Vector3Int offset in connectedCellsOffset)
        {
            Vector3Int connectedCell = currentCell + offset;
            if (connectedCell.x < grid.XSize && connectedCell.x >= 0 && connectedCell.y < grid.YSize && connectedCell.y >= 0)
            {
                if (grid.Tiles[connectedCell.x, connectedCell.y].objectInCell != null)
                    connected.Add(grid.Tiles[connectedCell.x, connectedCell.y].objectInCell);
            }
        }
        return connected;
    }
    public List<MergeableObject> GetConnectedSame(MergeableObject obj)
    {
        List<MergeableObject> connectedSame = new List<MergeableObject>();
        connectedSame.Add(obj);
        GetNearestConnectedSame(connectedSame, obj);
        return connectedSame;
    }
    public void GetNearestConnectedSame(List<MergeableObject> connectedSame, MergeableObject obj) // recursion function
    {
        Debug.Log(obj == null);
        List<MergeableObject> connected = GetNearestConnected(obj);
        List<MergeableObject> nearestConnectedSame = new List<MergeableObject>();

        foreach (MergeableObject connectedObject in connected)
        {
            if (IsSame(obj, connectedObject) && connectedSame.Contains(connectedObject) == false)
            {
                nearestConnectedSame.Add(connectedObject);
                connectedSame.Add(connectedObject);
            }
        }
        if (nearestConnectedSame.Count != 0)
        {
            foreach (MergeableObject connectedObject in nearestConnectedSame)
            {
                GetNearestConnectedSame(connectedSame, connectedObject);
            }
        }
    }
    public void Merge(MergeableObject draggedObject, List<MergeableObject> objectsToMerge) 
    {
        objectsToMerge.Add(draggedObject);
        foreach (MergeableObject obj in objectsToMerge)
        {
            obj.MoveToCell(draggedObject.nearestCell);
            grid.SetObjectInCell(obj.currentCell, null);
            obj.transform.DOScale(0, 1);
            obj.highlightOFF();
            Destroy(obj.gameObject, 2);
        }
        int i = 0;
        for (; i < GetNumberOfNewLevelObjects(objectsToMerge.Count); i++)
        {
            MergeableObject nextLevelObject = Instantiate(GetObjectPrefab(draggedObject.ObjectType, draggedObject.ObjectLevel + 1), grid.GetWorldCellPosition(objectsToMerge[i].currentCell), Quaternion.identity).GetComponent<MergeableObject>();
            nextLevelObject.transform.DOScale(0, 0);
            nextLevelObject.transform.DOScale(1, 1);
        }
        for (; i < GetNumberOfOldLevelObjects(objectsToMerge.Count) + GetNumberOfNewLevelObjects(objectsToMerge.Count); i++)
        {
            MergeableObject nextLevelObject = Instantiate(GetObjectPrefab(draggedObject.ObjectType, draggedObject.ObjectLevel), grid.GetWorldCellPosition(objectsToMerge[i].currentCell), Quaternion.identity).GetComponent<MergeableObject>();
            nextLevelObject.transform.DOScale(0, 0);
            nextLevelObject.transform.DOScale(1, 1);
        }
    }
    public GameObject GetObjectPrefab(string objectType, int ObjectLevel)
    {
        foreach (Type type in types)
        {
            if (objectType == type.name && ObjectLevel < type.prefabs.Count)
            {
                return type.prefabs[ObjectLevel];
            }
        }
        return null;
    }
    public int GetNumberOfNewLevelObjects(int numberOfObjects)
    {
        int numberOfNewLevelObjects = numberOfObjects / 3;
        return numberOfObjects % 6 == 5 ? numberOfNewLevelObjects + 1 : numberOfNewLevelObjects;
    }
    public int GetNumberOfOldLevelObjects(int numberOfObjects)
    {
        int numberOfNewLevelObjects = numberOfObjects -  (numberOfObjects / 3) * 3; 
        return numberOfObjects % 6 == 5 ? 0 : numberOfNewLevelObjects;
    }
    public int GetMaxLevelOfType(string objectType)
    {
        foreach (Type type in types)
        {
            if (objectType == type.name)
            {
                Debug.Log(type.prefabs.Count - 1);
                return type.prefabs.Count - 1;
            }
        }
        Debug.LogError("Error! There is no '" + objectType + "' in types list");
        return 0;
    }
    public bool CanMerge(MergeableObject mergeableObject, Vector3Int cell)
    {
        return IsSame(mergeableObject, grid.GetObject(cell)) 
            && GetConnectedSame(grid.GetObject(cell)).Count >= 2 
            && GetMaxLevelOfType(mergeableObject.ObjectType) > mergeableObject.ObjectLevel;
    }
}
