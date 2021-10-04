using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSillouhette : MonoBehaviour
{
    public Rope target;

    LineRenderer selfLineRenderer;
    Chaser chaser;

    void Start(){
        selfLineRenderer = GetComponent<LineRenderer>();
        chaser = GameObject.FindObjectOfType<Chaser>();
        if(!chaser){
            this.enabled = false;
        }
    }

    void FixedUpdate(){
        Vector3[] positions = new Vector3[target.lineRenderer.positionCount];
        target.lineRenderer.GetPositions(positions);
        for(int i = 0; i < target.lineRenderer.positionCount; i++){
            float z;
            if(chaser.GetComponent<Renderer>().bounds.Contains(new Vector3(positions[i].x, positions[i].y, chaser.transform.position.z))){
                z = -6f;
            } else {
                z = -20f;
            }
            positions[i] = new Vector3(positions[i].x, positions[i].y, z);
        }
        selfLineRenderer.positionCount = target.lineRenderer.positionCount;
        selfLineRenderer.SetPositions(positions);
    }
}
