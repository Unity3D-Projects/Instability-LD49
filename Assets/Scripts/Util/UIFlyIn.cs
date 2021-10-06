using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFlyIn : MonoBehaviour
{
    public float targetX;
    public float speed;
    public float delay;

    bool flyIn;

    void Start(){
        StartCoroutine(FlyIn());
    }

    void Update(){
        if(flyIn){
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, targetX, speed * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
        }
    }

    IEnumerator FlyIn(){
        yield return new WaitForSeconds(delay);
        flyIn = true;
    }
}
