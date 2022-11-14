using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float blastRadius = 2f;
    public float scale = 1.0f;
    public float damage = 0.0f;
    public string id;

    private ParticleSystem particles;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = transform.localScale * scale;
        particles = GetComponent<ParticleSystem>();
        Destroy(this.gameObject, particles.main.duration - Time.fixedDeltaTime);
        Explode();
    }

    void Explode() {
        Collider[] hits = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (var hit in hits) {
            print("explosion hit: " + hit.gameObject.ToString());
            EntityHealth entityHealth;
            hit.gameObject.TryGetComponent<EntityHealth>(out entityHealth);
            if (entityHealth != null) {
                float distance = (hit.ClosestPoint(transform.position) - transform.position).magnitude;
                float modifier = Mathf.Clamp01((blastRadius - distance) / blastRadius);
                entityHealth.TakeDamage(damage * modifier);
            }
        }
    }

}
