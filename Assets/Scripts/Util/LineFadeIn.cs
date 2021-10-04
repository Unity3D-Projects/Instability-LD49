using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFadeIn : MonoBehaviour
{
    LineRenderer lineRenderer;
    bool fade = true;

    void Start(){
        lineRenderer = GetComponent<LineRenderer>();

        GradientAlphaKey[] alphaArr = new GradientAlphaKey[1];
        alphaArr[0] = new GradientAlphaKey(0f, 0f);
        Gradient gra = new Gradient();
        gra.alphaKeys = alphaArr;
        gra.colorKeys = lineRenderer.colorGradient.colorKeys;
        lineRenderer.colorGradient = gra;

        StartCoroutine(Stop());
    }

    void Update(){
        if(fade){
            GradientAlphaKey[] alphaArr = new GradientAlphaKey[1];
            alphaArr[0] = new GradientAlphaKey(Mathf.Lerp(lineRenderer.colorGradient.alphaKeys[0].alpha, 1f, 1f * Time.deltaTime), 0f);
            Gradient gra = new Gradient();
            gra.alphaKeys = alphaArr;
            gra.colorKeys = lineRenderer.colorGradient.colorKeys;
            lineRenderer.colorGradient = gra;
        }
    }

    IEnumerator Stop(){
        yield return new WaitForSeconds(2f);
        fade = false;
    }

}
