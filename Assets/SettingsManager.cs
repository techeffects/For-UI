using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Text GraphicsText;

    // Start is called before the first frame update
    void Start()
    {
        int qualityLevel = QualitySettings.GetQualityLevel();

        if (qualityLevel == 0)
        {
            GraphicsText.text = "Low";
        }

        if (qualityLevel == 1)
        {
            GraphicsText.text = "Medium";
        }

        if (qualityLevel == 2)
        {
            GraphicsText.text = "High";
        }
    }

    // Function to change graphics quality to Low
    public void LowGraphics()
    {
        QualitySettings.SetQualityLevel(0);
        GraphicsText.text = "Low";
    }

    // Function to change graphics quality to Medium
    public void MediumGraphics()
    {
        QualitySettings.SetQualityLevel(1);
        GraphicsText.text = "Medium";
    }

    // Function to change graphics quality to High
    public void HighGraphics()
    {
        QualitySettings.SetQualityLevel(2);
        GraphicsText.text = "High";
    }
}
