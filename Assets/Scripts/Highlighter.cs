using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Highlighter : MonoBehaviour
{
    [SerializeField] private float highlighInSpeed;
    [SerializeField] private float highlighOutSpeed;
    [SerializeField] private float highlighIntensity;
    [SerializeField] private GameObject highlightSpritePrefab;
    private bool isHighlighted;
    private GameObject highlightSprite;
    [SerializeField] MergeableObject objectToHighlight;

    public void Start()
    {
        objectToHighlight.highlightON += HighLightOn;
        objectToHighlight.highlightOFF += HighLightOff;
    }
    public void Update()
    {
        if (highlightSprite != null)
        {
            highlightSprite.transform.position = objectToHighlight.transform.position;
        }
    }
    public void HighLightOn()
    {
        if (isHighlighted == false)
        {
            highlightSprite = Instantiate(highlightSpritePrefab, gameObject.transform.position, Quaternion.identity);
            highlightSprite.GetComponent<SpriteRenderer>().DOColor(new Color(255, 255, 255, 0), 0);
            highlightSprite.GetComponent<SpriteRenderer>().DOColor(new Color(255, 255, 255, highlighIntensity), highlighInSpeed);
            isHighlighted = true;
        }
    }
    public void HighLightOff()
    {
        if (isHighlighted == true)
        {
            highlightSprite.GetComponent<SpriteRenderer>().DOColor(new Color(255, 255, 255, 0f), highlighOutSpeed);
            Destroy(highlightSprite, highlighOutSpeed + 2f);
            isHighlighted = false;
        }
    }
    public static void HighLightObjectsOn(List<MergeableObject> mergeableObjects)
    {
        foreach (MergeableObject obj in mergeableObjects)
        {
            obj.highlightON();
        }
    }
    public static void HighLightObjectsOff(List<MergeableObject> mergeableObjects)
    {
        foreach (MergeableObject obj in mergeableObjects)
        {
            obj.highlightOFF();
        }
    }
}
