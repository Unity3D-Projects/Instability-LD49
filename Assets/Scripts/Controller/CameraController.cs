using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;

    [Space]

    [Header("Settings")]
    public float cameraSpeed;
    public float offsetMargin;
    public float cameraRotSpeed;

    public float globalOffset;

    float targetX;

    void FixedUpdate()
    {
        //Debug.DrawLine(new Vector3(playerController.transform.position.x-globalOffset, playerController.transform.position.y, playerController.transform.position.z), new Vector3(playerController.transform.position.x-globalOffset, playerController.transform.position.y+1f, playerController.transform.position.z), Color.blue);
        //Debug.DrawLine(new Vector3(playerController.transform.position.x-globalOffset, playerController.transform.position.y, playerController.transform.position.z), new Vector3(targetX+offsetMargin, playerController.transform.position.y, playerController.transform.position.z), Color.green);
        if(playerController.transform.position.x > targetX+offsetMargin){
            targetX = playerController.transform.position.x - offsetMargin;
        }
        if(playerController.transform.position.x < targetX - offsetMargin){
            targetX = playerController.transform.position.x + offsetMargin;
        }

        gameObject.transform.position = new Vector3(Mathf.Lerp(gameObject.transform.position.x-globalOffset, targetX, cameraSpeed*Time.deltaTime)+globalOffset, gameObject.transform.position.y, gameObject.transform.position.z);
    }

}
