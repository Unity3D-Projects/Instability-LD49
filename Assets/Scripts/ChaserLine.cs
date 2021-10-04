using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserLine : MonoBehaviour
{
    public Chaser chaser;

    public Vector3 lineStart;

    void LateUpdate(){
        if(chaser.transform.position.x < -22.37f){
            transform.localPosition = chaser.transform.InverseTransformPoint(lineStart);
        }
    }
}
