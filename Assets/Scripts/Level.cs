using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level", order = 1)]
public class Level : ScriptableObject
{
    public string levelName;
    public int threeStars;
    public int twoStars;
    public Tutorials tutorial;
    public TextAsset levelFile;
    public string menuName;
    [TextAreaAttribute]
    public string description;
}