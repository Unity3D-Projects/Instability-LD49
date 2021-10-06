using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class DeathMenuController : MonoBehaviour
{
    public TMP_Text highscoreText;
    public TMP_Text scoreText;
    public TMPBlink newHighscore;
    public RectTransform retryButton;
    public RectTransform menuButton;

    public SpriteRenderer gameCover;

    DataStorage dataStorage;

    bool showRetryButton;
    bool showMenuButton;

    public AudioClip selectClip;

    AudioSource source;

    bool cover;

    bool transitioning;

    void Start(){
        dataStorage = GameObject.FindObjectOfType<DataStorage>();

        highscoreText.text = "Highscore - " + dataStorage.highscore.ToString("F2");
        scoreText.text = "Score - " + dataStorage.prevGameScore.ToString("F2");

        StartCoroutine(NewHighscoreCheck());
        StartCoroutine(ButtonsShow());

        source = GetComponent<AudioSource>();
    }

    void Update(){
        if(showRetryButton){
            retryButton.localPosition = new Vector3(Mathf.Lerp(retryButton.localPosition.x, 743.3038f, 4f * Time.deltaTime), retryButton.localPosition.y, retryButton.localPosition.z);
        }
        if(showMenuButton){
            menuButton.localPosition = new Vector3(Mathf.Lerp(menuButton.localPosition.x, 743.3038f, 4f * Time.deltaTime), menuButton.localPosition.y, menuButton.localPosition.z);
        }
        if(cover){
            gameCover.color = Color.Lerp(gameCover.color, new Color(gameCover.color.r, gameCover.color.g, gameCover.color.b, 1f), 15f * Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            Restart();
        }
    }

    IEnumerator ButtonsShow(){
        yield return new WaitForSeconds(0.3f);
        showMenuButton = true;
        yield return new WaitForSeconds(0.1f);
        showRetryButton = true;
    }

    IEnumerator NewHighscoreCheck(){
        yield return new WaitForSeconds(0.35f);
        if(dataStorage.prevGameScore == dataStorage.highscore){
            newHighscore.blink = true;
        }
    }

    public void Restart(){
        if(!transitioning){
            StartCoroutine(ChangeLevel("Game"));
            source.clip = selectClip;
            source.Play();
            transitioning = true;
        }
    }

    public void ReturnToMenu(){
        if(!transitioning){
            StartCoroutine(ChangeLevel("Menu"));
            source.clip = selectClip;
            source.Play();
            transitioning = true;
        }
    }

    IEnumerator ChangeLevel(string sceneName){
        cover = true;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}
