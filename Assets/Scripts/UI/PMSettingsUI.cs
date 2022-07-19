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
