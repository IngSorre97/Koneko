using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Singleton;
    [SerializeField] private GameObject basic;
    [SerializeField] private GameObject reset;
    [SerializeField] private GameObject zoom;
    [SerializeField] private GameObject fast;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject heavy;

    void Start()
    {
        if (Singleton == null) Singleton = this;
        else Destroy(this);
    }

    public GameObject GetTutorial(Tutorials tutorialType)
    {
        switch (tutorialType)
        {
            case Tutorials.Basic: return basic;
            case Tutorials.Reset: return reset;
            case Tutorials.Zoom: return zoom;
            case Tutorials.Fast: return fast;
            case Tutorials.Ball: return ball;
            case Tutorials.Heavy: return heavy;
            default: return null;

        }
    }
}
