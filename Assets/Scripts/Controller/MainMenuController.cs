using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMenuController : MonoBehaviour
{
    public SpriteRenderer gameCover;
    public TMP_Text highscoreText;

    bool cover;
    DataStorage dataStorage;

    AudioSource source;
    public AudioClip selectClip;

    void Start(){
        dataStorage = GameObject.FindObjectOfType<DataStorage>();
        if(dataStorage.highscore == 0){
            dataStorage.highscore = PlayerPrefs.GetFloat("Highscore");
        }
        highscoreText.text = "Highscore - " + dataStorage.highscore.ToString("F2");
        GameObject.FindObjectOfType<MusicHandler>().songState = SongState.Menu;

        source = GetComponent<AudioSource>();
    }

    void Update(){
        if(cover){
            gameCover.color = Color.Lerp(gameCover.color, new Color(gameCover.color.r, gameCover.color.g, gameCover.color.b, 1f), 15f * Time.deltaTime);
        }
    }

    public void StartGame(){
        StartCoroutine(ChangeLevel("Game"));
        source.clip = selectClip;
        source.Play();
    }

    public void StartTutorial(){
        StartCoroutine(ChangeLevel("Tutorial"));
        source.clip = selectClip;
        source.Play();
    }

    public void QuitApp(){
        Application.Quit();
        source.clip = selectClip;
        source.Play();
    }

    IEnumerator ChangeLevel(string sceneName){
        cover = true;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}
