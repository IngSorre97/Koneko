using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Placeholder : MonoBehaviour
{
    public TextMeshProUGUI levelName;
    public TextMeshProUGUI record;
    public TextAsset level;

    public void OnClicked()
    {
        UIManager.Singleton.OnLevelClicked(this);
    }

    public void SetRecordString(string recordString)
    {
        record.text = recordString;
    }
}
