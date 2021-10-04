using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Gameplay References")]
    public PlayerController player;
    public SpriteRenderer gameCover;
    public Transform highestLine;
    public Transform currentLine;

    [Space]
    [Header("Text References")]
    public TMP_Text ingameScoreText;
    public TMP_Text beginText;

    // State
    public bool playing = false;

    // Data
    float score;
    bool cover;
    Vector3 highestLineOffset;
    Camera mainCamera;

    void Start(){
        mainCamera = Camera.main;

        if(highestLine && currentLine){
            highestLineOffset = highestLine.position - currentLine.position;
        }

        GameObject.FindObjectOfType<MusicHandler>().songState = SongState.Game;
    }

    void FixedUpdate(){
        if(highestLine && currentLine){
            if(highestLine.position.x < currentLine.position.x){
                highestLine.position = currentLine.position + highestLineOffset;
            }
        }
        if(playing){
            if(ingameScoreText){
                score = Mathf.Max(score, (player.transform.position.x + 6)/3f);
                ingameScoreText.text = score.ToString("F1");
            }

            beginText.color = Color.Lerp(beginText.color, new Color(1f, 1f, 1f, 0f), 4f * Time.deltaTime);
        }
        if(cover){
            gameCover.color = Color.Lerp(gameCover.color, new Color(gameCover.color.r, gameCover.color.g, gameCover.color.b, 1f), 5f * Time.deltaTime);
        }
    }

    public void Died(){
        playing = false;
        StartCoroutine(Death());
        SetScores();
        GameObject.FindObjectOfType<MusicHandler>().songState = SongState.Dead;
    }

    void SetScores(){
        DataStorage dataStorage = GameObject.FindObjectOfType<DataStorage>();
        dataStorage.prevGameScore = score;
        dataStorage.highscore = Mathf.Max(dataStorage.highscore, score);
    }

    public void StartMovement(){
        GameObject.FindObjectOfType<Chaser>().move = true;
        playing = true;
        beginText.GetComponent<TMPBlink>().blink = false;
    }

    IEnumerator Death(){
        //yield return new WaitForSeconds(2f);
        cover = true;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("DeathMenu");
    }
}
