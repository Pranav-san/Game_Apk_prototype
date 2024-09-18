using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/ Instant Effects/ Take Damage ")]
public class TakeDamageEffect : InstantCharacterEffect
{

    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float firelDamage = 0;
    public float lightininglDamage = 0;

    [Header("Final Damage")]
    private int finalDamageDealt = 0;

    [Header("Poise")]
    public float poise = 0;
    public bool poiseIsBroken = false;

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool willPlayDamageSfx = true;
    public AudioClip elementalSounfFX;

    [Header("Direction Damage Taken from")]
    public float angleHitFrom;
    public Vector3 contactPoint;
    public override void ProcessEffect(CharacterStatsManager character)
    {
        base.ProcessEffect(character);

        if(character.isDead)
        {
            return;
        }
        CalculateDamage(character);

    }
    private void CalculateDamage(CharacterStatsManager character)
    {


        finalDamageDealt= Mathf.RoundToInt(physicalDamage + magicDamage + firelDamage + lightininglDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt =1;
        }
        Debug.Log("FinalDamage: "+ finalDamageDealt); 
        character.ConsumeHealth(finalDamageDealt);


    }

}
