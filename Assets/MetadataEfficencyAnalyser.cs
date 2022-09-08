using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MetadataEfficencyAnalyser : MonoBehaviour
{
    [Serializable]
    public class EfficencyCategory
    {
        public string name;
        public List<MetadataValues> metadataValues = new List<MetadataValues>();

        public void CalculateScales()
        {
            foreach (var item in metadataValues)
            {
                item.CalculateScale();
            }
        }
    }

    [Serializable]
    public class MetadataValues
    {
        public string name;        
        public float upperBound;
        public float lowerBound;
        public int weighting;

        public float scaledValue;
        public float scaledWeighting;

        public void CalculateScale()
        {
            scaledValue = upperBound - lowerBound;
            scaledWeighting = (float)weighting / 100;
        }
    }

    [SerializeField]
    public List<EfficencyCategory> efficencyCategories = new List<EfficencyCategory>();
    AdjustableShape adjustableShape;

    public static MetadataEfficencyAnalyser instance;

    private void Start()
    {
        instance = this;
        adjustableShape = GetComponent<AdjustableShape>();
        foreach (var item in efficencyCategories)
        {
            foreach (var item2 in item.metadataValues)
            {
                item2.CalculateScale();
            }
        }
    }

    public void SetShapeValues(Metadata_Plus _metadata)
    {
        for (int i = 0; i < efficencyCategories.Count; i++)
        {
            float shapeValue = 0;
            foreach (var _metadataValue in efficencyCategories[i].metadataValues)
            {
                if(_metadata.parameters.ContainsKey(_metadataValue.name))
                {
                    float convertedValue = float.Parse(_metadata.parameters[_metadataValue.name]);
                    shapeValue += (( (convertedValue - _metadataValue.lowerBound )/ _metadataValue.scaledValue) ) * _metadataValue.scaledWeighting;
                }

            }
            adjustableShape.values[i] = shapeValue;
        }
    }

}
