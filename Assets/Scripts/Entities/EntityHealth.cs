using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public float health = 0.0f;
    public float maxHealth = 0.0f;

    public void TakeDamage(float damage) {
        print(transform.ToString() + " took damage, " + health + " - " + damage + " = " + (health - damage));
        health -= damage;
        if (health < 0) {
            health = 0;
        }

    }

    float lastHealth = 0.0f;
    void Update() {
        if (health != lastHealth) {
            lastHealth = health;
            print(transform.ToString() + " is at " + (Mathf.Floor((health / maxHealth) * 10000) / 10000) * 100 + "%");
        }
    }
}
