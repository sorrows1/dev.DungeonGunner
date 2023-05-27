using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    int startingHealth;
    int currentHealth;


    public void SetStartingHealth(int health)
    {
        startingHealth = health;
        currentHealth = health;
    }

    public int GetStartingHealth()
    {
        return startingHealth;
    }
}
