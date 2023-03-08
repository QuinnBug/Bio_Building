using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionInfoBox : Singleton<SelectionInfoBox>
{
    public GameObject core;
    public TMP_Text title;
    public TMP_Text description;
    public Image thumbnail;
    public Image[] indicatorIcons;
    [Space]
    //public Color[] colours;
    public Gradient colourGradient;
    public Range gradientTimeRange = new Range(0,9);
    private Metadata_Plus metadata;
    private string focusedName;

    public void Update()
    {
        core.SetActive(metadata != null);
    }

    public void SetObject(GameObject newObject)
    {
        if (focusedName == newObject.name) return;

        title.text = focusedName = newObject.name;
        metadata = newObject.GetComponentInChildren<Metadata_Plus>();
        if(metadata.parameters.TryGetValue("description", out string desc)) description.text = desc;

        thumbnail.sprite = ResourceManager.Instance.GetThumbnail(focusedName);

        float[] levels = MetadataEfficencyAnalyser.Instance.GetItemLevels(metadata);

        int i = 0;
        foreach (Image icon in indicatorIcons)
        {
            //Need to set the value from the metadata evaluator

            float value = levels[i];
            //metadata.parameters.TryGetValue("id", out string strValue);
            //if (int.TryParse(strValue[i].ToString(), out int x)) value = x;
            value = gradientTimeRange.NormaliseToRange(value);
            icon.color = colourGradient.Evaluate(value);
            Debug.Log(levels[i] + " to " + value);
            i++;
        }
    }

    public void ClearObject(GameObject testObject) 
    {
        if (focusedName != testObject.name) return;

        focusedName = "";
        metadata = null;
    }
}
