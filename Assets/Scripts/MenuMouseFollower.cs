using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMouseFollower : MonoBehaviour
{
    Rigidbody2D rigidBody2D;
    Camera mainCamera;

    bool connected;

    void Start(){
        rigidBody2D = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;


    }

    public void ConnectMouse(Rope rope){
        DistanceJoint2D joint = gameObject.AddComponent<DistanceJoint2D>();
        joint.connectedBody = rope.joints[rope.joints.Count-1];
        joint.distance = Vector3.Distance(transform.position, joint.connectedBody.position);
        connected = true;
    }

    void Update(){
        if(connected){
            rigidBody2D.MovePosition(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
