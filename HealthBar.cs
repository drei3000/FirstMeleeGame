using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float maxHealth = 200f;
    public Slider healthBarSlider;
    public PlayerHealth playerHealth;

    void Start()
    {
        // Find the PlayerHealth script if not assigned
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = maxHealth;
    }

    void Update()
    {
        // Check playerHealth and update slider accordingly
        if (playerHealth != null && healthBarSlider != null)
        {  
            healthBarSlider.value = playerHealth.currentHealth;
        }
    }
}