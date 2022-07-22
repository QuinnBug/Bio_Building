using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PMSettingsUI : MonoBehaviour
{
    public Toggle gridSnap;
    public Slider gridSlider;
    public TMP_InputField gridNumField;
    [Space]
    public Toggle wallSnap;
    public Slider wallSlider;
    public TMP_InputField wallNumField;
    [Space]
    public Toggle angleSnap;
    public Slider angleSlider;
    public TMP_InputField angleNumField;
    [Space]
    public Toggle contPlacement;
    [Space]
    public Toggle showGrid;


    private void Start()
    {
        gridSnap.isOn = WallPlacementManager.Instance.snapToGrid;
        gridSlider.value = WallPlacementManager.Instance.gridSpacing;
        gridNumField.text = gridSlider.value.ToString();

        wallSnap.isOn = WallPlacementManager.Instance.snapToWall;
        wallSlider.value = WallPlacementManager.Instance.snapDistance;
        wallNumField.text = wallSlider.value.ToString();

        angleSnap.isOn = WallPlacementManager.Instance.lockToAngle;
        angleSlider.value = WallPlacementManager.Instance.angleSnap;
        angleNumField.text = angleSlider.value.ToString();

        contPlacement.isOn = WallPlacementManager.Instance.continuousPlacement;

        showGrid.isOn = WallPlacementManager.Instance.showGrids;
    }

    private void Update()
    {
        WallPlacementManager.Instance.snapToGrid = gridSnap.isOn;
        WallPlacementManager.Instance.gridSpacing = gridSlider.value;
        if (!gridNumField.isFocused) gridNumField.text = gridSlider.value.ToString();

        WallPlacementManager.Instance.snapToWall = wallSnap.isOn;
        WallPlacementManager.Instance.snapDistance = wallSlider.value;
        if (!wallNumField.isFocused) wallNumField.text = wallSlider.value.ToString();

        WallPlacementManager.Instance.lockToAngle = angleSnap.isOn;
        WallPlacementManager.Instance.angleSnap = angleSlider.value;
        if (!angleNumField.isFocused) angleNumField.text = angleSlider.value.ToString();

        WallPlacementManager.Instance.continuousPlacement = contPlacement.isOn;

        WallPlacementManager.Instance.showGrids = showGrid.isOn;

    }

    public void UpdateSlidersViaFields(string value) 
    {
        if (float.TryParse(gridNumField.text, out float gridNum)) 
        {
            gridSlider.value = gridNum;
        }

        if (float.TryParse(wallNumField.text, out float wallNum))
        {
            wallSlider.value = wallNum;
        }

        if (float.TryParse(angleNumField.text, out float angleNum))
        {
            angleSlider.value = angleNum;
        }
    }
}
