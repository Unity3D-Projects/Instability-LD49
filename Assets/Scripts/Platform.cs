using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Platform : MonoBehaviour
{   
    [Header("Settings")]
    public bool damaged;
    public float fallAnimSpeed;

    public Vector3 fallAnimOffset;
    public float fallAnimRotOffset;

    public Vector3 stutterAnimOffset;
    public float stutterAnimSpeed;

    [Space]
    [Header("Debug")]
    public float fallDelay;
    public bool fallAnim;
    public float fallAnimAlpha = 0.01f;
    public Vector3 startLoc;
    public bool steppedOn;
    public bool stutter;

    SpriteRenderer sRenderer;

    public AudioClip fall;
    public AudioClip stutterClip;
    AudioSource source;

    void Start()
    {
        fallDelay = Random.Range(0.2f, 0.5f);

        startLoc = transform.position;

        sRenderer = GetComponent<SpriteRenderer>();

        if(damaged){
            //sRenderer.color = new Color(1f, 0.75f, 0.75f);
        }

        source = GetComponent<AudioSource>();
    }

    public void Reset(){
        StopAllCoroutines();
        GetComponent<BoxCollider2D>().enabled = true;
        fallAnimAlpha = 0.001f;
        fallAnim = false;
        stutter = false;
        steppedOn = false;
        sRenderer.color = new Color(1f, 1f, 1f);
        gameObject.transform.position = startLoc;
        gameObject.transform.rotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        if(fallAnim){
            fallAnimAlpha += fallAnimAlpha * (fallAnimSpeed * Time.deltaTime);
            gameObject.transform.position = Vector3.Lerp(startLoc+stutterAnimOffset, startLoc + fallAnimOffset, fallAnimAlpha);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, fallAnimRotOffset, fallAnimAlpha));
        }
        if(stutter){
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, startLoc + stutterAnimOffset, stutterAnimSpeed * Time.deltaTime);
        }
        if(steppedOn){
            if(damaged){
                sRenderer.color = Color.Lerp(sRenderer.color, new Color(1f, 0.2f, 0.2f), 30f * Time.deltaTime);
            }
        }
    }

    public void SteppedOn(){
        if(!steppedOn && damaged){
            steppedOn = true;
            StartCoroutine(Stutter());
            StartCoroutine(Fall());
        }
    }

    IEnumerator Stutter(){
        yield return new WaitForSeconds(0f);
        stutter = true;
        source.clip = stutterClip;
        source.Play();
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        stutter = false;
        fallAnim = true;
        yield return new WaitForSeconds(0.8f);
        source.clip = fall;
        source.Play();
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
