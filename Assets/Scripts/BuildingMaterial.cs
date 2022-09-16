using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New BuildMat", menuName = "Building Material", order = 0)]
public class BuildingMaterial : ScriptableObject
{
    [Header("Details")]
    public string materialName;
    public string uniclassCode;

    [Header("Components")]
    public string[] rawMaterials;

    [Header("Stats")]
    public float strengthNormal;
    public float strengthParallel;
    public float strengthBending;
    public float strengthMeanE;
    public float grossDensity;
    public float thermalConductivity;
    public float vapourDiffusion;
    public float durability;
    public float carbonStorage;
    public float calciumCarbonation;

    [Header("Climate Change across system boundaries")]
    public float[] productGWP;
    public float[] useGWP;
    public float[] endOfLifeGWP;

    [Header("E.O.L outcomes")]
    public bool dismantle;
    public bool reUse;
    public float recycle;
    public float energySource;
    public bool landfill;

    [Header("Other")]
    public string healtAndWellness;
    public string manufacturingImpact;
    public string useImpact;
    public string VOC;
    public string fireResistance;
    public string fireProtection;
    public string burningDroplets;
    public string smokeGasDevelopment;
    public string smokeToxicity;
    public string qualityManagement;
    
}
