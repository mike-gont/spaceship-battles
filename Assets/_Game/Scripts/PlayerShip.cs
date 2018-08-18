using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerShipPhysics))]
[RequireComponent(typeof(PlayerShipInput))]


public abstract class PlayerShip : NetworkEntity {

    public GameObject ShipExplosion;

    protected PlayerShipInput input;
    protected PlayerShipPhysics physics;

    public int PlayerID { get { return EntityID; } }
    public int Health { get; set; }
    public static readonly int initialHealth = 100;
    public int Score { get; set; }
    public int Deaths { get; set; }

    protected static LocalPlayerShip activeShip;
    
    public static LocalPlayerShip ActiveShip { get { return activeShip; } }
    public virtual Vector3 Velocity { get { return physics.Rigidbody.velocity; } }
    public float Throttle { get { return input.throttle; } }
    public float Boost { get; set; }
    public string PlayerName { get; set; }
    public byte ShipType { get; set; }

    protected bool isDead = false;
    public bool IsDead { get { return isDead; } }

    protected void Awake() {
        input = GetComponent<PlayerShipInput>();
        physics = GetComponent<PlayerShipPhysics>();
        Health = initialHealth;
    }

    public void SetInitShipData(int entityID, int clientID, string playerName, byte shipType) {
        ObjectType = (byte)NetworkEntity.ObjType.Player;
        EntityID = entityID;
        ClientID = clientID;
        PlayerName = playerName;
        ShipType = shipType;
    }

    public void RespawnOnClientStart() {
        if (isServer) {
            Debug.LogWarning("this shouldnt be called on server");
            return;
        }
        ShipShootingClient shipShooting = ActiveShip.GetComponent<ShipShootingClient>();
        if (shipShooting.lockTargetID == PlayerID) { // disable locking in hud
            shipShooting.lockTargetID = -1;
        }
        if (isDead)
            return;
        Debug.Log("respawn start");
        input.DisableInput();
        Destroy(Instantiate(ShipExplosion, transform.position, Quaternion.identity), 5);/// dosnt work?>
       
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers){
            r.enabled = false; 
        }
        
        foreach (Collider c in GetComponentsInChildren<Collider>()) { // BUG: with this adde we see engine particles after ship explodes??
            c.enabled = false;
        }

        GetComponentInChildren<ParticleSystem>().Pause(); //BUG: this does not work....

        isDead = true;
  }

    public void RespawnOnClientEnd() {
        if (isServer) {
          Debug.LogWarning("this shouldnt be called on server");
          return;
        }
        Debug.Log("respawn end coroutine call");
        StartCoroutine("DelayedRespawnEnd");

    }

    IEnumerator DelayedRespawnEnd() {
        Debug.Log("respawn end before");
        yield return new WaitForSeconds(1.0f);
        Debug.Log("respawn end after");
        input.EnableInput();

        clientController.gameManager.clientUI.DisableRespawnScreen();

        GetComponentInChildren<ParticleSystem>().Play();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) {
            r.enabled = true;
        }
      
        foreach (Collider c in GetComponentsInChildren<Collider>()) {
            c.enabled = true;
        }

        isDead = false;
        yield return null;
    }

}
