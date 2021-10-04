using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFadeIn : MonoBehaviour
{
    SpriteRenderer rendererRef;
    bool fade = true;

    public bool invert;

    void Start(){
        rendererRef = GetComponent<SpriteRenderer>(); 
        rendererRef.color = new Color(rendererRef.color.r,rendererRef.color.g,rendererRef.color.b, invert ? 1f : 0f);
        StartCoroutine(Stop());
    }

    void Update(){
        if(fade){
            rendererRef.color = Color.Lerp(rendererRef.color, new Color(rendererRef.color.r,rendererRef.color.g,rendererRef.color.b, invert ? 0f : 1f), 1.5f * Time.deltaTime);
        }
    }

    IEnumerator Stop(){
        yield return new WaitForSeconds(1.5f);
        fade = false;
    }


}
