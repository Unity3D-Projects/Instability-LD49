using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    public float slowSpeed;
    public float fastSpeed;
    public float speed;
    public float interpSpeed;

    public bool move;

    public float fastSpeedIncreaseAmount;

    public float targetSpeed;

    PlayerController player;
    Camera mainCamera;

    void Start(){
        player = GameObject.FindObjectOfType<PlayerController>();
        mainCamera = Camera.main;
    }

    void FixedUpdate(){
        if(move){
            float alphaOnPlayerSpeed = player.GetComponent<Rigidbody2D>().velocity.x/8f;
            float alphaOnVisible = (mainCamera.IsObjectVisible(GetComponent<Renderer>()) ? 0f : 1f);
            float alphaOnPlayerRope = player.onRope ? 0.4f : 1f;
            float alpha = Mathf.Min(alphaOnPlayerRope, Mathf.Max(alphaOnPlayerSpeed, alphaOnVisible));
            targetSpeed = Mathf.Lerp(slowSpeed, fastSpeed, alpha);

            speed = Mathf.Lerp(speed, targetSpeed, interpSpeed * Time.deltaTime);

            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);

            fastSpeed += fastSpeedIncreaseAmount * Time.deltaTime;
        }
    }
}
