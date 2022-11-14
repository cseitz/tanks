using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityReplicator : MonoBehaviour
{
    public static EntityReplicator Instance { get; private set; }

    public List<ExplosionConfig> explosions = new List<ExplosionConfig>();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) {
            Instance = this;
        }
    }

    bool Ready() {
        if (Instance == null) {
            Start();
            return false;
        }
        return true;
    }

    public static void Apply(string serializedState)
    {
        var state = JsonUtility.FromJson<EntityReplicator>(serializedState);
        ExplosionManager.Replicate(state.explosions.ToArray());
    }

    public string Serialize()
    {
        return JsonUtility.ToJson(Instance);
    }

    void FixedUpdate() {
        // Remove explosions that have been destroyed
        explosions = explosions.FindAll(el => ExplosionManager.Instance.transform.Find(el.id) != null);
    }

    void Update() {
        Ready();
        // if (explosions.Count > 0) {
        //     print("explosions: " + Serialize());
        // }
    }
}
