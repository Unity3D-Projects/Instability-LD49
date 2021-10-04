using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SongType {
    Slow,
    Fast
}

[System.Serializable]
public enum SongState{
    Menu,
    Dead,
    Game
}

[System.Serializable]
public class SongLoops{
    public AudioSource menuLoop;
    public AudioSource gameLoop;
    public AudioSource deadLoop;

    public SongLoops(AudioSource _menuLoop, AudioSource _gameLoop, AudioSource _deadLoop){
        menuLoop = _menuLoop;
        gameLoop = _gameLoop;
        deadLoop = _deadLoop;
    }
}

public class MusicHandler : MonoBehaviour
{
    public static MusicHandler instance = null;  

    public float fastSPB;
    public float slowSPB;

    public int beat;

    [SerializeField] public SongType songType;
    [SerializeField] public SongState songState;

    [SerializeField] public SongLoops slowClips;
    [SerializeField] public SongLoops fastClips;

    public AudioSource currentSource;

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
        StartCoroutine(Beat());
    }

    IEnumerator Beat(){
        yield return new WaitForSeconds(songType == SongType.Fast ? fastSPB : slowSPB);
        if(currentSource){
            currentSource.volume = 0;
        }
        switch(songState){
            case SongState.Menu:
                currentSource = (songType == SongType.Fast ? fastClips.menuLoop : slowClips.menuLoop);
                break;
            case SongState.Game:
                currentSource = (songType == SongType.Fast ? fastClips.gameLoop : slowClips.gameLoop);
                break; 
            case SongState.Dead:
                currentSource = (songType == SongType.Fast ? fastClips.deadLoop : slowClips.deadLoop);
                break; 
            default:
                currentSource = (songType == SongType.Fast ? fastClips.menuLoop : slowClips.menuLoop);
                break;
        }
        currentSource.volume = 1;
        beat += 1;
        StartCoroutine(Beat());
    }
}
