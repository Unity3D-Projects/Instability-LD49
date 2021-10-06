using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    public bool damaged;
    public float amountOfJoints;
    public float collisionRadius;
    public float distanceBetween;
    public float stretchDistance;

    public Vector2Int snapIndexRange;

    int snapIndex;

    public List<Rigidbody2D> joints = new List<Rigidbody2D>();
    List<Rigidbody2D> snappedJoints = new List<Rigidbody2D>();
    public LineRenderer lineRenderer;
    public LineRenderer snappedLineRenderer;

    public MenuMouseFollower menuMouseFollower;

    Camera mainCamera;

    float stretchTime;
    float snapTime;

    bool stretched;
    bool jumpedOn;

    bool shownAlready;

    bool frozen;
    bool started = false;

    public AudioClip stretchClip;
    public AudioClip snapClip;

    PlayerController player;

    void Start(){
        player = GameObject.FindObjectOfType<PlayerController>();

        snapIndex = Random.Range(snapIndexRange.x, snapIndexRange.y);

        stretchTime = Random.Range(1f, 2f);
        snapTime = Random.Range(1f, 2f);

        for(int i = 0; i < amountOfJoints; i++){
            GameObject newJoint = new GameObject();
            newJoint.transform.parent = transform;
            Vector3 offset = (Vector3.down*distanceBetween*i);
            // if(i>5){
            //     offset += (Vector3.right * distanceBetween * (i-5));
            // }
            newJoint.transform.position = (transform.position + offset);

            if (i != 0)
            {
                newJoint.AddComponent<DistanceJoint2D>().connectedBody = joints[i-1];
                newJoint.AddComponent<CircleCollider2D>().radius = collisionRadius;
                newJoint.GetComponent<DistanceJoint2D>().distance = distanceBetween;
                newJoint.GetComponent<Rigidbody2D>().mass = 0.05f;
                newJoint.GetComponent<Rigidbody2D>().gravityScale = 2f;
                newJoint.layer = gameObject.layer;
                joints.Add(newJoint.GetComponent<Rigidbody2D>());

            } else {
                newJoint.AddComponent<Rigidbody2D>().isKinematic = true;
                joints.Add(newJoint.GetComponent<Rigidbody2D>());
            }
        }

        if(menuMouseFollower){
            menuMouseFollower.ConnectMouse(this);
        }

        lineRenderer = GetComponent<LineRenderer>();

        Vector3[] positions = new Vector3[joints.Count];
        lineRenderer.positionCount = joints.Count;
        int j = 0;
        foreach(Rigidbody2D joint in joints){
            positions[j] = joint.transform.position;
            j += 1;
        }
        lineRenderer.SetPositions(positions);

        mainCamera = Camera.main;
    }

    void Update(){
        bool canSee = mainCamera.IsObjectVisible(lineRenderer);
        if(canSee){
            shownAlready = true;
        }
        if(canSee || !shownAlready){
            Vector3[] positions = new Vector3[joints.Count];
            int j = 0;
            foreach(Rigidbody2D joint in joints){
                positions[j] = joint.transform.position;
                j += 1;
            }
            lineRenderer.SetPositions(positions);
        }
        if(snappedLineRenderer){
            if(mainCamera.IsObjectVisible(snappedLineRenderer)){
                Vector3[] newPositions = new Vector3[snappedJoints.Count];
                int j2 = 0;
                foreach(Rigidbody2D joint in snappedJoints){
                    newPositions[j2] = joint.transform.position;
                    j2 += 1;
                }
                snappedLineRenderer.SetPositions(newPositions);
            }
        }

        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        if(distance < 24f && !started){
            foreach(Rigidbody2D joint in joints){
                joint.AddForce(new Vector2(Random.Range(0.4f,0f), Random.Range(-1f, 1f)), ForceMode2D.Impulse);
            }
            started = true;
        }
        if(distance > 25f){
            if(!frozen){
                foreach(Rigidbody2D joint in joints){
                    joint.isKinematic = true;
                    joint.velocity = Vector2.zero;
                }
                frozen = true;
            }
        } else {
            if(frozen){
                int j = 0;
                foreach(Rigidbody2D joint in joints){
                    if(j != 0){
                        joint.isKinematic = false;
                        joint.velocity = Vector2.zero;
                    }
                    j += 1;
                }
                frozen = false;
            }
        }

        if(jumpedOn){
            if(damaged){
                Color newCol = Color.Lerp(lineRenderer.colorGradient.colorKeys[0].color, new Color(1f, 0.2f, 0.2f), 30f * Time.deltaTime);
                GradientColorKey[] colorArr = new GradientColorKey[1];
                colorArr[0] = new GradientColorKey(newCol, 0f);
                Gradient gra = new Gradient();
                gra.colorKeys = colorArr;
                lineRenderer.colorGradient = gra;
            }
        }
    }

    public void JumpedOn(){
        jumpedOn = true;
        if(damaged){
            StartCoroutine(BreakAnimate());
        }
    }

    IEnumerator BreakAnimate(){
        yield return new WaitForSeconds(stretchTime);
        if(player.currentRope == this){
            SFXHandler.PlaySound(stretchClip);
            Stretch();
            yield return new WaitForSeconds(snapTime);
            if(true){
                SFXHandler.PlaySound(snapClip);
                Snap();
            }
        } else {
            jumpedOn = false;
        }
    }

    //[EasyButtons.Button]
    void Stretch(){
        joints[snapIndex].GetComponent<DistanceJoint2D>().distance = stretchDistance;
        stretched = true;
    }

    [EasyButtons.Button]
    void Snap(){
        Vector3[] oldPositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(oldPositions);

        Vector3[] newPositions = new Vector3[lineRenderer.positionCount-snapIndex];
        for(int i = 0; i < lineRenderer.positionCount; i++){
            if(i > snapIndex-1){
                newPositions[i-snapIndex] = oldPositions[i-snapIndex];
            }
        }
        lineRenderer.positionCount = lineRenderer.positionCount-snapIndex;
        lineRenderer.SetPositions(newPositions);

        snappedJoints = joints.GetRange(0, snapIndex);

        joints.RemoveRange(0, snapIndex);

        Destroy(joints[0].GetComponent<DistanceJoint2D>());

        GameObject newObject = new GameObject();
        newObject.transform.parent = transform;
        newObject.transform.position = transform.position;

        snappedLineRenderer = newObject.AddComponent<LineRenderer>();
        snappedLineRenderer.materials = lineRenderer.materials;
        snappedLineRenderer.colorGradient = lineRenderer.colorGradient;
        snappedLineRenderer.widthCurve = lineRenderer.widthCurve;

        Vector3[] snappedPositions = new Vector3[snappedJoints.Count];
        snappedLineRenderer.positionCount = snappedJoints.Count;
        int j = 0;
        foreach(Rigidbody2D joint in snappedJoints){
            snappedPositions[j] = joint.transform.position;
            j += 1;
        }
        snappedLineRenderer.SetPositions(snappedPositions);
    }
}
