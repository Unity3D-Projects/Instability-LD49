using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage : MonoBehaviour
{
    public float highscore;
    public float prevGameScore;

    public static DataStorage instance = null;  

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

    void Start(){
        highscore = PlayerPrefs.GetFloat("Highscore");
    }

    void OnApplicationQuit(){
        PlayerPrefs.SetFloat("Highscore", highscore);
    }
}
