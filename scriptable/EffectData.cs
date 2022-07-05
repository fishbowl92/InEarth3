using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "skillBox", menuName = "Scriptable Object/effect")]
public class EffectData : ScriptableObject
{
    public Sprite[] spriteData;
    public float delay;
    public int banbok;
    public float startDelay;
    public EffectData(EffectData m, int repeat)
    {
        spriteData = m.spriteData;
        delay = m.delay;
        banbok = repeat;
        startDelay = m.startDelay;
    }
}
