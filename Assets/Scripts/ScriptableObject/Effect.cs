using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect")]
public class Effect : ScriptableObject
{
    public string effectName;
    public bool isEffectCharacterUp;
    public RuntimeAnimatorController effect;
    public AudioClip effectSE;
}
