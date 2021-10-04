using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPFadeIn : MonoBehaviour
{
    TMP_Text text;
    bool fade = false;

    public bool startOnPlay = true;
    public float startOnPlayDelay = 0f;

    void Start(){
        text = GetComponent<TMP_Text>();

        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        
        if(startOnPlay){
            StartCoroutine(StartDelay());
        }
    }

    IEnumerator StartDelay(){
        yield return new WaitForSeconds(startOnPlayDelay);
        fade = true;
        StartCoroutine(Stop());
    }

    public void FadeIn(){
        StartCoroutine(StartDelay());
    }

    void Update(){
        if(fade){
            text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 1f), 1f * Time.deltaTime);
        }
    }

    IEnumerator Stop(){
        yield return new WaitForSeconds(1.5f);
        fade = false;
        if(GetComponent<TMPBlink>()){
            if(GameObject.FindObjectOfType<GameController>()){
                if(!GameObject.FindObjectOfType<GameController>().playing){
                    GetComponent<TMPBlink>().blink = true;
                }
            } else {
                GetComponent<TMPBlink>().blink = true;
            }
        }
    }
}
