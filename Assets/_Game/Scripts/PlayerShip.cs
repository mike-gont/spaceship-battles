using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerShipPhysics))]
[RequireComponent(typeof(PlayerShipInput))]


public abstract class PlayerShip : NetworkEntity
{
    protected int clientID = -1;

    public bool isPlayer = true;
    protected static PlayerShip activeShip;

    protected PlayerShipInput input;
    protected PlayerShipPhysics physics;

    public int ClientID { get { return clientID; } set { clientID = value; } }
    public static PlayerShip ActiveShip { get { return activeShip; } }
    

    // Getters for external objects
    public bool UsingMouseInput
    {
        get { return input.useMouseInput; }
    }

    public Vector3 Velocity
    {
        get { return physics.GetComponent<Rigidbody>().velocity; }
    }

    public float Throttle
    {
        get { return input.throttle; }
    }
	
	private void Update ()
    {

    }

    protected void Awake() {
        input = GetComponent<PlayerShipInput>();
        physics = GetComponent<PlayerShipPhysics>();
    }
}
