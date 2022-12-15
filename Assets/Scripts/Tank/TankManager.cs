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
        if (Instance == null) {
            Instance = this;
        }
        Transform tankSpawns = GameObject.Find("Spawns").transform.Find("Tanks").transform;
        foreach (Transform child in tankSpawns) {
            TankSpawns.Add(child);
        }
        Spawn();
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }


    private float destroyCooldown = 0;

    // Update is called once per frame
    void Update()
    {
        if (destroyCooldown > 0) {
            destroyCooldown -= Time.deltaTime;
            if (destroyCooldown < 0) {
                destroyCooldown = 0;
            }
        }
    }

    public static void Replicate(List<TankServerStateEntry> replicatedTanks) {
        var ids = new List<string>();
        for (int i = 0; i < replicatedTanks.Count; ++i) {
            var e = replicatedTanks[i];
            ids.Add("tank_" + e.id);
            if (tanks.ContainsKey(e.id)) {
                tanks[e.id].Apply(e.state);
            } else {
                // spawn tank
                Debug.Log("need to create tank: " + e.id);
                var tank = Instantiate(Instance.PrefabTank, Vector3.zero, Quaternion.identity, Instance.transform);
                var state = tank.GetComponent<TankControllerState>();
                tanks.Add(e.id, state);
                state.Apply(e.state);
            }
        }
        if (Instance && Instance.currentTank && Instance.destroyCooldown <= 0) {
            foreach (Transform child in Instance.transform) {
                if (child.name.StartsWith("tank_") && !ids.Contains(child.name)) {
                    if (!child.GetComponent<TankController>().enabled && child.name != Instance.currentTank.name) {
                        if (child.GetComponent<EntityHealth>().health <= 0) {
                            print("destroy " + child.name);
                            Destroy(child.gameObject);
                        }
                    }
                }
            }
        }
    }

    private GameObject currentTank;
    public static void Spawn() {
        Instance.destroyCooldown += 5;
        var spawn = TankSpawns[Random.Range(0, TankSpawns.Count)];
        var tank = Instantiate(Instance.PrefabTank, spawn.position, spawn.rotation, Instance.transform);
        tank.GetComponent<TankController>().enabled = true;
        Instance.currentTank = tank;
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
