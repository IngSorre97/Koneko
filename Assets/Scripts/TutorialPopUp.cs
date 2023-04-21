using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialPopUp : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Image checkedImage;


    private bool doNotShow = false;

    public void Start()
    {
        Manager.Singleton.tutorial = this;
    }

    public void OnCheckClicked()
    {
        doNotShow = !doNotShow;
        checkedImage.gameObject.SetActive(doNotShow);
    }

    public void OnDoneClicked()
    {
        if (doNotShow) UIManager.Singleton.doNotShowTutorial = true;
        Manager.Singleton.tutorial = null;
        Destroy(this);
    }
}
