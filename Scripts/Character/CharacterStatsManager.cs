using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    [Header("Status")]
    public bool isDead = false;

    public float maxStamina = 500;
    public float currentStamina = 500;

    public int maxHealth = 100;
    public int currentHealth = 100;

    public CharacterManager character;

    private void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    
    //OnValidate Only runs On Editor
    private void OnValidate()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (!isDead)
            {
                isDead = true;
                if (character != null)
                {
                    StartCoroutine(character.ProcessDeathEvent());
                }
            }
        }

        if (PlayerUIManager.instance != null)
        {
            PlayerUIManager.instance.UpdateStaminaBar(Mathf.RoundToInt(currentStamina));
            PlayerUIManager.instance.UpdateHealthBar(currentHealth);
        }
    }
    
    
    void Update()
    {
        RegenerateStamina(25);
        
    }

    public void CheckHP(int oldValue, int newValue)
    {
        if (currentHealth<=0)
        {
            isDead = true;
            StartCoroutine(character.ProcessDeathEvent());
        }
    }

    public void ConsumeHealth(int amount)
    {
        int oldHealth = currentHealth;

        if (currentHealth - amount < 0)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth -= amount;
        }

        PlayerUIManager.instance.UpdateHealthBar(Mathf.RoundToInt(currentHealth));

        CheckHP(oldHealth, currentHealth);
    }

    public void ConsumeStamina(float amount)
    {
        if (currentStamina - amount < 0)
        {
            currentStamina = 0;
        }
        else
        {
            currentStamina -= amount;
        }
    }

    public void RegenerateStamina(float amountPerSecond)
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += amountPerSecond * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
            PlayerUIManager.instance.UpdateStaminaBar(Mathf.RoundToInt(currentStamina));
            PlayerInputManager.Instance.ForceStateUpdate();
        }
    }
}
