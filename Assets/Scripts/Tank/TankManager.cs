using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankManager : MonoBehaviour
{
    public GameObject PrefabTank;
    public static TankManager Instance { get; private set; }

    public static Dictionary<string, TankControllerState> tanks = new Dictionary<string, TankControllerState>();

    [System.Serializable]
    public struct TankServerStateEntry {
        public string id;
        public string state;
    }

    private static List<Transform> TankSpawns = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null) {
            Instance = this;
        }
        Transform tankSpawns = GameObject.Find("Spawns").transform.Find("Tanks").transform;
        foreach (Transform child in tankSpawns) {
            TankSpawns.Add(child);
        }
        Spawn();
    }

    void Awake() {
        if (Instance == null || Instance.gameObject == null) {
            Instance = this;
        }
    }

    private float destroyCooldown = 0;
    private float replicateElapsed = 0;

    // Update is called once per frame
    void Update()
    {
        Awake();
        replicateElapsed += Time.deltaTime;
        if (destroyCooldown > 0) {
            destroyCooldown -= Time.deltaTime;
            if (destroyCooldown < 0) {
                destroyCooldown = 0;
            }
        }
    }

    private Dictionary<string, float> pendingDeletes = new Dictionary<string, float>();
    private List<string> replicatedTankIds = new List<string>();
    public void Replicate(List<TankServerStateEntry> replicatedTanks) {
        if (gameObject == null) return;
        float elapsed = replicateElapsed + 0;
        replicateElapsed = 0;
        var ids = new List<string>();
        // print("replicated tank count: " + replicatedTanks.Count);
        for (int i = 0; i < replicatedTanks.Count; ++i) {
            var e = replicatedTanks[i];
            Transform found = Instance.transform.Find("tank_" + e.id);
            ids.Add(e.id);
            // print("running tank " + e.id);
            if (found != null) {
                if (found.gameObject.activeInHierarchy) {
                    TankControllerState state;
                    if (found.TryGetComponent(out state)) {
                        state.Apply(e.state);
                        state.id = e.id;
                    }
                }
            } else {
                if (replicatedTankIds.Contains(e.id)) continue;
                // spawn tank
                Debug.Log("need to create tank: " + e.id);
                replicatedTankIds.Add(e.id);
                var tank = Instantiate(PrefabTank, Vector3.zero, Quaternion.identity, transform);
                var state = tank.GetComponent<TankControllerState>();
                // tanks.Add(e.id, state);
                state.Apply(e.state);
                state.id = e.id;
            }
            float value;
            if (pendingDeletes.TryGetValue(e.id, out value)) {
                pendingDeletes.Remove(e.id);
            }
            // if (tanks.ContainsKey(e.id)) {
            //     Transform found = Instance.transform.Find("tank_" + e.id);
            //     if (found != null && found.gameObject.activeInHierarchy) {
            //         TankControllerState state;
            //         if (found.TryGetComponent(out state)) {
            //             state.Apply(e.state);
            //             ids.Add(e.id);
            //         }
            //     }
            // } else {
            //     // spawn tank
            //     Debug.Log("need to create tank: " + e.id);
            //     var tank = Instantiate(Instance.PrefabTank, Vector3.zero, Quaternion.identity, Instance.transform);
            //     var state = tank.GetComponent<TankControllerState>();
            //     tanks.Add(e.id, state);
            //     state.Apply(e.state);
            //     ids.Add(e.id);
            // }
        }
        if (Instance != null && Instance.currentTankId != null) {
            ids.Add(Instance.currentTankId);
            float value;
            if (pendingDeletes.TryGetValue(Instance.currentTankId, out value)) {
                pendingDeletes.Remove(Instance.currentTankId);
            }
        }
        foreach (Transform child in transform) {
            if (!child.name.StartsWith("tank_")) continue;
            var __name = new List<string>(child.name.Split('_'));
            string id = __name[__name.Count - 1];
            float value;
            // print(pendingDeletes.ContainsKey(id) + ": " + id);
            if (!ids.Contains(id)) {
                if (pendingDeletes.TryGetValue(id, out value)) {
                    // print("oh no incrementing " + id);
                    pendingDeletes[id] += elapsed;
                    if (pendingDeletes[id] > 20) {
                        TankController controller;
                        bool ok = true;
                        if (child.gameObject.TryGetComponent(out controller)) {
                            if (controller.isActiveAndEnabled) {
                                ok = false;
                            }
                        }
                        if (ok) {
                            print("destroy " + child.name);
                            Destroy(child.gameObject);
                        }
                    }
                } else {
                    pendingDeletes.Add(id, elapsed);
                }
            }
        }
        // foreach (string id in ids) {
        //     pendingDeletes.Remove(id);
        // }
        // if (Instance && Instance.currentTank && Instance.destroyCooldown <= 0) {
        //     foreach (Transform child in Instance.transform) {
        //         if (child.name.StartsWith("tank_") && !ids.Contains(child.name)) {
        //             if (!child.GetComponent<TankController>().enabled && child.name != Instance.currentTank.name) {
        //                 if (child.GetComponent<EntityHealth>().health <= 0) {
        //                     print("destroy " + child.name);
        //                     Destroy(child.gameObject);
        //                 }
        //             }
        //         }
        //     }
        // }
    }

    public GameObject currentTank;
    private string currentTankId;
    public static void Spawn() {
        Instance.destroyCooldown += 5;
        var id = System.Guid.NewGuid().ToString();
        var spawn = TankSpawns[Random.Range(0, TankSpawns.Count)];
        var tank = Instantiate(Instance.PrefabTank, spawn.position, spawn.rotation, Instance.transform);
        tank.GetComponent<TankController>().enabled = true;
        tank.GetComponent<TankControllerState>().id = id;
        Instance.currentTank = tank;
        Instance.currentTankId = id;
        // Instance.transform.SetParent(Instance.transform);
    }

    public void RequestRespawn(float delay) {
        Invoke("Respawn", delay);
    }
    
    public void Respawn() {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (currentTank) {
            Destroy(currentTank.gameObject);
        }
        Spawn();
    }
}
