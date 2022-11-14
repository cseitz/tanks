using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TankParticles : MonoBehaviour
{
    public GameObject explosionDirt;
    public GameObject explosionSmall;
    public GameObject explosionLarge;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explosion(string type, Vector3 pos, Quaternion rot = new Quaternion()) {
        if (true) {
            ExplosionManager.Spawn(new ExplosionConfig () {
                type = "Big",
                position = pos,
                damage = 60.0f,
                radius = 10.0f,
            });
            return;
        }
        GameObject explosion = null;
        if (type == "dirt") {
            explosion = Instantiate(explosionDirt, pos, rot);
        } else if (type == "small") {
            explosion = Instantiate(explosionSmall, pos, rot);
        } else if (type == "large") {
            explosion = Instantiate(explosionLarge, pos, rot);
        }
        if (explosion != null) {
            ParticleSystem particle = explosion.GetComponent<ParticleSystem>();
            var main = particle.main;
            main.loop = false;
            float duration = main.duration;
            Destroy(explosion, duration);
        }
    }
}
