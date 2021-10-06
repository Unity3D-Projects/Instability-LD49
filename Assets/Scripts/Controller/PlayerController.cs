using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Chaser chaser;

    [Header("VFX")]
    public GameObject impactVFX;

    [Header("Settings")]
    public float movementSpeed;
    public float jumpVelocity;
    public float xDrag = 1.2f;
    public float xVeloInterp = 2f;
    public LayerMask movementTraceMask;
    public LayerMask interactionTraceMask;
    public float uprightInterpSpeed;

    public Rope currentRope;

    public Rigidbody2D rigidBody;
    float targetXVelo;
    bool canJump = true;
    float lastJumpTime = -1f;
    bool fellOff = false;
    float fallOffTime = -1f;
    public bool onRope = false;

    bool dead = false;

    bool onGround;

    bool lockedRigid = false;

    GameObject scalingStandin;

    public bool canMove;

    public AudioClip deathSFX;
    public List<AudioClip> jumpSFX = new List<AudioClip>();
    public List<AudioClip> landSFX = new List<AudioClip>();
    public List<AudioClip> collideSFX = new List<AudioClip>();
    public AudioClip grabRopeSFX;
    public AudioClip releaseRopeSFX;

    Platform currentGround;
    
    // Start is called before the first frame update
    void Start()
    {
       rigidBody = GetComponent<Rigidbody2D>(); 
       StartCoroutine(EnableMovement());
    }

    IEnumerator EnableMovement(){
        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space) && canJump && canMove){
            if(chaser){
                if(!chaser.move){
                    if(GameObject.FindObjectOfType<GameController>()){
                        GameObject.FindObjectOfType<GameController>().StartMovement();
                    }
                }
            }
            rigidBody.velocity = new Vector2(targetXVelo, jumpVelocity);
            rigidBody.constraints = RigidbodyConstraints2D.None;
            rigidBody.AddTorque(-targetXVelo * 0.004f, ForceMode2D.Impulse);
            canJump = false;
            lastJumpTime = Time.time;

            SFXHandler.PlaySound(jumpSFX[Random.Range(0, jumpSFX.Count)]);
        }

        if(Input.GetKey(KeyCode.Space) && canJump == false && !onRope && !onGround){
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, 0.4f, interactionTraceMask);
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, 0.4f, interactionTraceMask);
            RaycastHit2D upHit = Physics2D.Raycast(transform.position, Vector2.up, 0.4f, interactionTraceMask);

            if(rightHit){
                if(rightHit.collider.transform.parent.GetComponent<Rope>())
                {
                    rightHit.collider.transform.parent.GetComponent<Rope>().JumpedOn();
                    gameObject.AddComponent<FixedJoint2D>().connectedBody = rightHit.rigidbody;
                    onRope = true;
                    currentRope = rightHit.collider.transform.parent.GetComponent<Rope>();
                    SFXHandler.PlaySound(grabRopeSFX);
                } 
            } else {
                if(leftHit){
                    if(leftHit.collider.transform.parent.GetComponent<Rope>())
                    {
                        leftHit.collider.transform.parent.GetComponent<Rope>().JumpedOn();
                        gameObject.AddComponent<FixedJoint2D>().connectedBody = leftHit.rigidbody;
                        onRope = true;
                        currentRope = leftHit.collider.transform.parent.GetComponent<Rope>();
                        SFXHandler.PlaySound(grabRopeSFX);
                    }
                } else {
                    if(upHit){
                        if(upHit.collider.transform.parent.GetComponent<Rope>())
                        {
                            upHit.collider.transform.parent.GetComponent<Rope>().JumpedOn();
                            gameObject.AddComponent<FixedJoint2D>().connectedBody = upHit.rigidbody;
                            onRope = true;
                            currentRope = upHit.collider.transform.parent.GetComponent<Rope>();
                            SFXHandler.PlaySound(grabRopeSFX);
                        }
                    }
                }
            }
        }
        
        if(Input.GetKeyUp(KeyCode.Space)){
            if(gameObject.GetComponent<FixedJoint2D>()){
                Destroy(gameObject.GetComponent<FixedJoint2D>());
                onRope = false;
                currentRope = null;
                SFXHandler.PlaySound(releaseRopeSFX);
            }
        }

        bool extendRay = rigidBody.velocity.y > -4 && !onRope;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, Vector2.down, extendRay ? 2f : 1f, movementTraceMask);
        //Debug.DrawLine(transform.position, transform.position + (Vector3.down * (extendRay ? 2f : 1f)), Color.red);
        if (raycastHit){
            // ON GROUND

            currentGround = raycastHit.transform.gameObject.GetComponent<Platform>();

            if(!onGround){
                onGround = true;
                if(raycastHit.collider.gameObject.GetComponent<Platform>()){
                    if(raycastHit.collider.gameObject.GetComponent<Platform>().damaged){
                        GameObject newVFX = Instantiate(impactVFX, raycastHit.point, Quaternion.Euler(-90f, 0f, 0f));
                    }
                }
                SFXHandler.PlaySound(landSFX[Random.Range(0, landSFX.Count)]);
            }

            fellOff = false;
            if(Time.time - lastJumpTime >= 0.2){
                canJump = true;
            }

            if(raycastHit.transform.gameObject.GetComponent<Platform>()){
                raycastHit.transform.gameObject.GetComponent<Platform>().SteppedOn();
            }
        } else {
            // IN AIR
            onGround = false;
            currentGround = null;

            rigidBody.constraints = RigidbodyConstraints2D.None;
            lockedRigid = false;

            if(scalingStandin){
                transform.parent = null;
                Destroy(scalingStandin);
            }

            if(!fellOff){
                fellOff = true;
                fallOffTime = Time.time;
            } else {
                if(Time.time - fallOffTime >= 0.15){
                    canJump = false;
                } else {
                    Debug.DrawLine(transform.position, transform.position + (Vector3.down * 1f), Color.red, 0f);
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!dead && canMove){
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
                if(onRope){
                    rigidBody.AddForce(Vector2.right * (movementSpeed/2f));
                } else {
                    targetXVelo = movementSpeed;
                }
                if(chaser){
                    if(!chaser.move){
                        if(GameObject.FindObjectOfType<GameController>()){
                            GameObject.FindObjectOfType<GameController>().StartMovement();
                        }
                    }
                }
            } else {
                if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
                    if(onRope){
                        rigidBody.AddForce(Vector2.left * (movementSpeed/2f));
                    } else {
                        targetXVelo = -movementSpeed;
                    }
                    if(chaser){
                        if(!chaser.move){
                            if(GameObject.FindObjectOfType<GameController>()){
                                GameObject.FindObjectOfType<GameController>().StartMovement();
                            }
                        }
                    }
                } else {
                    targetXVelo = 0;
                }
            }
        }

        if (transform.position.y < -7){
            if(GameObject.FindObjectOfType<TutorialController>()){
                GameObject.FindObjectOfType<TutorialController>().FellOff(transform.position.x);
            } else {
                if(!dead){
                    if(transform.position.x - chaser.transform.position.x > 33){
                        chaser.transform.position = new Vector3(transform.position.x - 33f, chaser.transform.position.y, chaser.transform.position.z);
                    }
                    if(GameObject.FindObjectOfType<GameController>()){
                        GameObject.FindObjectOfType<GameController>().Died();
                    }
                    SFXHandler.PlaySound(deathSFX);
                    dead = true;
                }
                rigidBody.isKinematic = true;
                rigidBody.velocity = Vector2.zero;
                rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        if(dead){
            chaser.transform.position = new Vector3(Mathf.Lerp(chaser.transform.position.x, transform.position.x, 1f * 0.03f), chaser.transform.position.y, chaser.transform.position.z);
        }

        if(chaser){
            if(transform.position.x - chaser.transform.position.x < 6.6f){
                if(!dead){
                    if(transform.position.x - chaser.transform.position.x > 33){
                        chaser.transform.position = new Vector3(transform.position.x - 33f, chaser.transform.position.y, chaser.transform.position.z);
                    }
                    if(GameObject.FindObjectOfType<GameController>()){
                        GameObject.FindObjectOfType<GameController>().Died();
                    }
                    SFXHandler.PlaySound(deathSFX);
                    dead = true;
                    canMove = false;
                }
            }
        }

        if (onGround){
            // ON GROUND

            if(currentGround.gameObject.GetComponent<Platform>()){

                if(!scalingStandin){
                    scalingStandin = new GameObject();
                    scalingStandin.transform.parent = currentGround.transform;
                    transform.parent = scalingStandin.transform;
                }

                float target = 0;
                if(transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270){
                    target = 180;
                }
                if(transform.rotation.eulerAngles.z > 270){
                    target = 360;
                }
                if(transform.rotation.eulerAngles.z < -90 && transform.rotation.eulerAngles.z > -270){
                    target = -180;
                }
                if(transform.rotation.eulerAngles.z < -270){
                    target = -360;
                }
                if(currentGround.transform.eulerAngles.z != 0){
                    float offsetAngle = -(360-currentGround.transform.eulerAngles.z);
                    target += offsetAngle;
                }

                if(!lockedRigid)
                {
                    lockedRigid = true;
                    rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;    
                }
                
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(transform.rotation.eulerAngles.z,  target, uprightInterpSpeed * 0.02f));
            }

        } else {
            // IN AIR
            if(Physics2D.Raycast(transform.position, Vector2.right, 1f, movementTraceMask)){
                targetXVelo = 0;
            }
            if(Physics2D.Raycast(transform.position, Vector2.left, 1f, movementTraceMask)){
                targetXVelo = 0;
            }
        }

        if(!onRope && !dead){
            if(targetXVelo == 0){
                float xVelo = rigidBody.velocity.x;
                xVelo /= xDrag;
                rigidBody.velocity = new Vector2(xVelo, rigidBody.velocity.y);
            } else {
                rigidBody.velocity = new Vector2(Mathf.Lerp(rigidBody.velocity.x, targetXVelo, xVeloInterp * 0.03f), rigidBody.velocity.y);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.transform.parent){
            if(!collision.collider.transform.parent.GetComponent<Rope>()){
                if(!onGround){
                    SFXHandler.PlaySound(collideSFX[Random.Range(0, collideSFX.Count)]);
                }
            }
        } else {
            if(!onGround){
                SFXHandler.PlaySound(collideSFX[Random.Range(0, collideSFX.Count)]);
            }
        }
    }
}
