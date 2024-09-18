using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Character Effects/ Instant Effects/ Stamina Damage effect")]
public class TakeStaminaDamageEffect : InstantCharacterEffect
{
    public int staminaDamage;

    playerManager player;

    public override void ProcessEffect(CharacterStatsManager character)
    {
        CalculateStaminaDamage(character);
    }
    public void CalculateStaminaDamage(CharacterStatsManager character)
    {
        character.ConsumeStamina(staminaDamage);
        
    }




}


