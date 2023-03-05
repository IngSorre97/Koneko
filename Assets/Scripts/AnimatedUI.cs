using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedUI : MonoBehaviour
{
    [SerializeField] private Sprite first;
    [SerializeField] private Sprite second;

    private int current = 1;
    private Coroutine _coroutine;

    // Update is called once per frame
    void OnEnable()
    {
        _coroutine = StartCoroutine(animation());
    }

    private void OnDisable()
    {
        StopCoroutine(animation());
        _coroutine = null;
    }

    private IEnumerator animation()
    {
        while (true)
        {
            if (current == 1)
            {
                gameObject.GetComponent<Image>().sprite = first;
                current = 2;
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = second;
                current = 1;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
