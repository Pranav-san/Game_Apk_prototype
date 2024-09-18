using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    // This Script Process Damage, Block Damage, Poision Damage Healing...And
    // Anything That Affects character is processed Here


    CharacterStatsManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterStatsManager>();
    }
    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);

    }
    
   
}
