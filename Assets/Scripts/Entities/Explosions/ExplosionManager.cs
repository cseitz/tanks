using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    [System.Serializable]
    public struct ExplosionEntry {
        public string name;
        public GameObject prefab;
    }

    public ExplosionEntry[] explosions;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
