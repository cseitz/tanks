using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EntityReplicator : MonoBehaviour
{
    public static EntityReplicator Instance { get; private set; }

    public List<ExplosionConfig> explosions = new List<ExplosionConfig>();
    [System.NonSerialized] public List<TankControllerState> tanks = new List<TankControllerState>();

    [System.NonSerialized] private float syncRate = 1 / 10;
    [System.NonSerialized] private float deltaSync = 0;

    [System.Serializable]
    public struct EntitySyncState {
        public List<ExplosionConfig> explosions;
        public TankControllerState tank;
    }

    [System.Serializable]
    public struct EntityServerSyncState {
        public List<ExplosionConfig> explosions;
        public List<TankManager.TankServerStateEntry> tanks;
    }

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
        var state = JsonUtility.FromJson<EntityServerSyncState>(serializedState);
        ExplosionManager.Replicate(state.explosions.ToArray());
        TankManager.Replicate(state.tanks);
    }

    public string Serialize()
    {
        string entities = JsonUtility.ToJson(Instance);
        string tank = "null";
        if (TankController.Instance) {
            TankController Tank = TankController.Instance;
            tank = Tank != null ? Tank.GetComponent<TankControllerState>().Serialize() : "null";
        }
        return "{\"tank\":" + tank + "," + entities.Substring(1);
    }

    void FixedUpdate() {
        // Remove explosions that have been destroyed
        explosions = explosions.FindAll(el => ExplosionManager.Instance.transform.Find(el.id) != null);
    }

    void Update() {
        Ready();
        deltaSync += Time.deltaTime;
        if (deltaSync > syncRate) {
            deltaSync -= syncRate;
            StartCoroutine(Sync());
        }
    }

    // https://stackoverflow.com/questions/46003824/sending-http-requests-in-c-sharp-with-unity
    IEnumerator Sync() {
        var req = new UnityWebRequest("http://localhost:6080/entity/sync", "POST");
        byte[] payload = new System.Text.UTF8Encoding().GetBytes(Serialize());
        req.uploadHandler = (UploadHandler) new UploadHandlerRaw(payload);
        req.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        if (!req.isNetworkError) {
            // apply the state returned from the request
            Apply(req.downloadHandler.text);
        } else {
            Debug.Log("Sync Error: " + req.error);
        }
    }


}
