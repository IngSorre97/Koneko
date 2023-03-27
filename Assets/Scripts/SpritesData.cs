using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesData : MonoBehaviour
{
    public static SpritesData Singleton;

    [Header("Sprites")]
    public Sprite outOfBounds;
    public Sprite wall;
    public Sprite empty;

    public List<Sprite> holes = new List<Sprite>();

    public Sprite noStars;
    public Sprite oneStar;
    public Sprite twoStar;
    public Sprite threeStar;

    void Start()
    {
        if (Singleton == null) Singleton = this;
        else return;
    }

    public Sprite Hole(int capacity)
    {
        Debug.Log("Asked capacity " + capacity);
        capacity = Mathf.Min(capacity, holes.Count - 1);
        capacity = Mathf.Max(capacity, 0);
        return holes[capacity];
    }
}
