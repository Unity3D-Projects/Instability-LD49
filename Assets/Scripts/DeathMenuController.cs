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
            retryButton.localPosition = new Vector3(Mathf.Lerp(retryButton.localPosition.x, 743.3038f, 2f * Time.deltaTime), retryButton.localPosition.y, retryButton.localPosition.z);
        }
        if(showMenuButton){
            menuButton.localPosition = new Vector3(Mathf.Lerp(menuButton.localPosition.x, 743.3038f, 2f * Time.deltaTime), menuButton.localPosition.y, menuButton.localPosition.z);
        }
        if(cover){
            gameCover.color = Color.Lerp(gameCover.color, new Color(gameCover.color.r, gameCover.color.g, gameCover.color.b, 1f), 5f * Time.deltaTime);
        }
    }

    IEnumerator ButtonsShow(){
        yield return new WaitForSeconds(1.6f);
        showMenuButton = true;
        yield return new WaitForSeconds(0.5f);
        showRetryButton = true;
    }

    IEnumerator NewHighscoreCheck(){
        yield return new WaitForSeconds(1.5f);
        if(dataStorage.prevGameScore == dataStorage.highscore){
            newHighscore.blink = true;
        }
    }

    public void Restart(){
        StartCoroutine(ChangeLevel("Game"));
        source.clip = selectClip;
        source.Play();
    }

    public void ReturnToMenu(){
        StartCoroutine(ChangeLevel("Menu"));
        source.clip = selectClip;
        source.Play();
    }

    IEnumerator ChangeLevel(string sceneName){
        cover = true;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }
}
