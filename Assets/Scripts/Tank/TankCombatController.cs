using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCombatController : MonoBehaviour
{
    private TankControllerState state;
    private TankConfig config;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        state = GetComponent<TankControllerState>();
        config = GetComponent<TankConfig>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
