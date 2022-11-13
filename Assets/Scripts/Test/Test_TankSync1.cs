using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_TankSync1 : MonoBehaviour
{
    public TankControllerState tank;
    public TankControllerState otherTank;
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        string state = tank.Serialize();
        otherTank.positionOffset = offset;
        otherTank.Apply(state);
    }
    
}
