using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPBlink : MonoBehaviour
{
    TMP_Text text;
    public bool blink;

    float targetAlpha = 0f;

    void Start(){
        text = GetComponent<TMP_Text>();
        StartCoroutine(Toggle());
    }

    void Update(){
        if(blink){
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(text.color.a, targetAlpha, 40f * Time.deltaTime));
        }
    }

    IEnumerator Toggle(){
        if(blink){
            targetAlpha = targetAlpha == 0 ? 1 : 0;
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(Toggle());
    }   

}
