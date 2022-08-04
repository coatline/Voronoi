using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome")]

public class Biome : ScriptableObject
{
    public Color color;
    public int height;
    public float scale;
}
