using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ExplosionConfig {
    public string id;
    public string type;
    public Vector3 position;
    public float radius;
    public float damage;
}


public class ExplosionManager : MonoBehaviour
{
    [System.Serializable]
    public struct ExplosionEntry {
        public string name;
        public GameObject prefab;
    }

    public ExplosionEntry[] types;
    private Dictionary<string, ExplosionEntry> _types;

    public static ExplosionManager Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        _types = new Dictionary<string, ExplosionEntry>();
        for (int i = 0; i < types.Length; ++i) {
            _types.Add(types[i].name, types[i]);
        }
    }

    bool Ready() {
        if (Instance == null) {
            Start();
            return false;
        }
        return true;
    }

    void Update() {
        Ready();
    }

    public static GameObject Spawn(ExplosionConfig config) {
        config.id = config.id ?? System.Guid.NewGuid().ToString();
        return Instance._spawn(config, true);
    }

    public GameObject _spawn(ExplosionConfig config, bool replicate = false) {
        GameObject prefab = _types[config.type].prefab;
        GameObject obj = Instantiate(prefab, config.position, Quaternion.identity, transform);
        obj.name = config.id;

        ExplosionController controller = obj.GetComponent<ExplosionController>();
        controller.scale = config.radius / controller.blastRadius;
        controller.blastRadius *= controller.scale;
        controller.damage = config.damage;
        controller.id = config.id;

        if (replicate) {
            EntityReplicator.Instance.explosions.Add(config);
        }

        return obj;
    }

    public static void Replicate(ExplosionConfig[] replicatedExplosions) {
        for (int i = 0; i < replicatedExplosions.Length; ++i) {
            ExplosionConfig e = replicatedExplosions[i];
            if (Instance.transform.Find(e.id) == null) {
                Instance._spawn(e);
            }
        }
    }
}
