using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXHandler : MonoBehaviour
{
    public static SFXHandler instance = null; 

    static List<AudioSource> sources = new List<AudioSource>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);   
        }

        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySound(AudioClip clip){
        if(instance){
            bool playedUsingExisting = false;
            foreach(AudioSource source in sources.ToArray()){
                if(source){
                    if(!playedUsingExisting){
                        if(!source.isPlaying){
                            source.clip = clip;
                            source.Play();
                            playedUsingExisting = true;
                        }
                    } else {
                        if(!source.isPlaying){
                            sources.Remove(source);
                            Destroy(source);
                        }
                    }
                } else {
                    sources.Remove(source);
                }
            }
            if(!playedUsingExisting){
                AudioSource newSource = instance.gameObject.AddComponent<AudioSource>();
                newSource.clip = clip;
                newSource.Play();
                sources.Add(newSource);
            }
        }
    }
}
