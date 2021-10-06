using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public List<Transform> respawnPoints = new List<Transform>();
    public List<Platform> damagedPlatforms = new List<Platform>();
    public List<Rope> damagedRopes = new List<Rope>();

    public SpriteRenderer gameCover;

    public GameObject ropePrefab;

    public BoxCollider2D chaserReturnPrevent;
    public BoxCollider2D chaserPreventJump;
    public List<TMPFadeIn> chaserTutMessages = new List<TMPFadeIn>();

    PlayerController player;
    Chaser chaser;

    bool chaserTut;
    bool cover;

    void Start(){
        player = GameObject.FindObjectOfType<PlayerController>();
        chaser = GameObject.FindObjectOfType<Chaser>();

        GameObject.FindObjectOfType<MusicHandler>().songState = SongState.Game;
    }
    void FixedUpdate(){
        if(player.transform.position.x > 132f){
            if(!chaserTut){
                chaserTut = true;
                chaserReturnPrevent.enabled = true;
                foreach(TMPFadeIn fade in chaserTutMessages){
                    fade.FadeIn();
                }
                chaserPreventJump.enabled = true;
                StartCoroutine(AllowMove());
            }
        }
        if(chaserTut){
            chaser.transform.position = new Vector3(Mathf.Lerp(chaser.transform.position.x, 119f, 4f * Time.deltaTime), chaser.transform.position.y, chaser.transform.position.z);
        }
        if(cover){
            gameCover.color = Color.Lerp(gameCover.color, new Color(gameCover.color.r, gameCover.color.g, gameCover.color.b, 1f), 15f * Time.deltaTime);
        }
    }

    IEnumerator AllowMove(){
        yield return new WaitForSeconds(2f);
        chaserPreventJump.enabled = false;
    }

    public void FellOff(float x){
        foreach(Platform platform in damagedPlatforms){
            platform.Reset();
        }

        List<Rope> newRopes = new List<Rope>();
        foreach(Rope rope in damagedRopes){
            Vector3 pos = rope.transform.position;
            Destroy(rope.gameObject);
            GameObject newRope = Instantiate(ropePrefab, pos, Quaternion.identity);
            newRope.GetComponent<Rope>().damaged = true;
            newRopes.Add(newRope.GetComponent<Rope>());
        }
        damagedRopes = newRopes;

        for(int i = 0; i < respawnPoints.Count; i++){
            if(x < respawnPoints[i].position.x){
                if(i-1 < 0){
                    StartCoroutine(ChangeLevel("Menu"));
                    return;
                }
                player.transform.position = respawnPoints[i-1].position;
                return;
            }
        }
        StartCoroutine(ChangeLevel("Menu"));
    }

    IEnumerator ChangeLevel(string sceneName){
        cover = true;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}
