using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MetadataEfficencyAnalyser : Singleton<MetadataEfficencyAnalyser>
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
        [Range(0,100)]
        public int weighting;

        internal float scaledValue;
        internal float scaledWeighting;

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
        adjustableShape = FindObjectOfType<AdjustableShape>();

        foreach (var item in efficencyCategories)
        {
            foreach (var item2 in item.metadataValues)
            {
                item2.CalculateScale();
            }
        }
    }

    public float[] GetItemGroupLevels(Metadata_Plus[] metadatas) 
    {
        float[] levels = new float[] { 0, 0, 0, 0, 0 };

        foreach (Metadata_Plus data in metadatas)
        {
            int i = 0;
            foreach (EfficencyCategory cat in efficencyCategories)
            {
                levels[i] += GetMetadataValue(data, cat.metadataValues);
                i++;
            }
        }

        return levels;
    }

    public float[] GetItemLevels(Metadata_Plus data)
    {
        float[] levels = new float[] { 0, 0, 0, 0, 0 };

        int i = 0;
        foreach (EfficencyCategory cat in efficencyCategories)
        {
            levels[i] += GetMetadataValue(data, cat.metadataValues);
            i++;
        }

        for (int j  = 0; j < levels.Length; j++)
        {
            levels[j] *= 10;
        }

        return levels;
    }

    public void SetShapeValues(Metadata_Plus _metadata)
    {
        for (int i = 0; i < efficencyCategories.Count; i++)
        {
            //float shapeValue = 0;
            //foreach (var _metadataValue in efficencyCategories[i].metadataValues)
            //{
            //    if(_metadata.parameters.ContainsKey(_metadataValue.name))
            //    {
            //        float convertedValue = float.Parse(_metadata.parameters[_metadataValue.name]);

            //        shapeValue += ((convertedValue - _metadataValue.lowerBound)
            //            / _metadataValue.scaledValue) 
            //            * _metadataValue.scaledWeighting;
            //    }

            //}
            //adjustableShape.values[i] = shapeValue;
            adjustableShape.values[i] = GetMetadataValue(_metadata, efficencyCategories[i].metadataValues);
        }
    }

    public float GetMetadataValue(Metadata_Plus _metadata, List<MetadataValues> values) 
    {
        float output = 0;
        foreach (var _metadataValue in values)
        {
            if (_metadata.parameters.ContainsKey(_metadataValue.name))
            {
                float convertedValue = float.Parse(_metadata.parameters[_metadataValue.name]);

                output += ((convertedValue - _metadataValue.lowerBound)
                    / _metadataValue.scaledValue)
                    * _metadataValue.scaledWeighting;
            }

        }

        return output;
    }

}
