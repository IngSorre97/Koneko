using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedUI : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    [SerializeField] private float frameDuration = 0.5f;

    private int _current = 0;
    private Coroutine _coroutine;

    // Update is called once per frame
    void OnEnable()
    {
        _coroutine = StartCoroutine(Animation());
    }

    private void OnDisable()
    {
        StopCoroutine(Animation());
        _coroutine = null;
    }

    private IEnumerator Animation()
    {
        while (true)
        {
            _current = (_current + 1) % sprites.Count;
            gameObject.GetComponent<Image>().sprite = sprites[_current];
            
            yield return new WaitForSeconds(frameDuration);
        }
    }
}
