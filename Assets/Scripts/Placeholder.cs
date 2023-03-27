using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Placeholder : MonoBehaviour
{
    public TextMeshProUGUI levelName;
    public Image stars;
    public UIManager.LevelFile level;
    public int index;

    public void OnClicked()
    {
        UIManager.Singleton.OnLevelClicked(this);
    }
}
