﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkEntity : MonoBehaviour {

    protected Queue<NetMsg> incomingQueue = new Queue<NetMsg>();

    protected GameObject networkControllerObj;
    protected Server serverController;
    protected Client clientController;
    protected int entityID = -1;
    public bool isServer;

    public enum ObjType : byte {
        Player,
        Missile,
    }
    protected byte objectType;
    public byte ObjectType {
        get { return objectType; }
        set { objectType = value; }
    }

    // History
    private class ShipSnapshot {
        public float time;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;

        public ShipSnapshot(float time, Vector3 position, Quaternion rotation, Vector3 velocity) {
            this.time = time;
            this.position = position;
            this.rotation = rotation;
            this.velocity = velocity;
        }
    }
    private List<ShipSnapshot> History = new List<ShipSnapshot>();
    private static int historySize = 200;

    public int EntityID {
        get { return entityID; }
        set { entityID = value; }
    }

    public void Start() {
        Debug.Log("START OF NETWORK ENTITY!");
        // setting up serverController / clientController referecnce 
        networkControllerObj = GameObject.Find("ServerNetworkController");
        if (networkControllerObj != null) {
            isServer = true;
            serverController = networkControllerObj.GetComponent<Server>();
            if (serverController == null) {
                Debug.LogError("server controller wasn't found for network entity = " + entityID);
            }
        }
        else {
            networkControllerObj = GameObject.Find("ClientNetworkController");
            if (networkControllerObj != null) {
                isServer = false;
                clientController = networkControllerObj.GetComponent<Client>();
                if (clientController == null) {
                    Debug.LogError("client controller wasn't found for network entity = " + entityID);
                }
            }
            else {
                Debug.LogError("ERROR! networkController not found for network entity = " + entityID);
            }
        }
            

    }

    private void FixedUpdate() {
        if (incomingQueue.Count > 100) {
            Debug.LogWarning("Warning: incomingQueue size > 100 for network entity = " + entityID);
        }
    }

    public void AddRecMessage(NetMsg msg) {
        incomingQueue.Enqueue(msg);
       // Debug.Log("incoming message to NetworkEntity");
    }

    private void AddSnapshotToHistory() {

        History.Add(new ShipSnapshot(Time.time, transform.position, transform.rotation, GetVelocity() ));

        // let's limit thje History size
        if (History.Count > historySize) {
            History.RemoveAt(0);
        }
    }

    public virtual Vector3 GetVelocity() {
        return GetComponent<Rigidbody>().velocity; // overridden in RemotePlayerShip to return last received velocity (because it's rigid body has 0 velocity)
    }

    private void OnDestroy() {
        if (isServer) {
            //serverController.netEntities.Remove(entityID);
        }
    }


}
