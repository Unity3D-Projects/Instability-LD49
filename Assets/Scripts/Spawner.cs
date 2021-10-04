using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SpawnObjectType{
    Platform,
    Rope
}

[System.Serializable]
public class SpawnObject{
    public Vector2 pos;
    [SerializeField] public SpawnObjectType spawnObjectType;
    public float platformSizeX;

    public bool damaged;

    public SpawnObject(Vector2 _pos, SpawnObjectType _spawnObjectType, float _platformSizeX, bool _damaged){
        pos = _pos;
        spawnObjectType = _spawnObjectType;
        platformSizeX = _platformSizeX;
        damaged = _damaged;
    }
}

public class Spawner : MonoBehaviour
{   
    [Header("Controller Settings")]
    public float startX;

    [Space]
    [Header("Spawn Settings")]
    public float ropeHeight;
    public int platformChance; // Out of 100
    public int chanceDamaged; // Out of 100
    
    [Space]
    public Vector2 platformHeightMinMax;
    public Vector2 platformSizeXMinMax;
    
    [Space]
    public float spawnXPer;
    public Vector2 spawnOffsetXMinMax;

    [Space]
    [Header("References")]
    public GameObject ropePrefab;
    public GameObject platformPrefab;
    public GameObject spawnObject;

    [Space]
    [Header("Debug")]
    public int current;
    public int currentSpawned;

    [SerializeField]
    public List<SpawnObject> spawnList = new List<SpawnObject>();

    void GenerateItem(){
        float x = startX + (currentSpawned * spawnXPer) + Random.Range(spawnOffsetXMinMax.x, spawnOffsetXMinMax.y);

        int objectType = Random.Range(0, 100);
        SpawnObjectType spawnObjectType;
        if(objectType < platformChance){
            spawnObjectType = SpawnObjectType.Platform;
        } else {
            spawnObjectType = SpawnObjectType.Rope;
        }

        float y = spawnObjectType == SpawnObjectType.Platform ? Random.Range(platformHeightMinMax.x, platformHeightMinMax.y) : ropeHeight;
        float platformSizeX = Random.Range(platformSizeXMinMax.x, platformSizeXMinMax.y);
        bool damaged = Random.Range(0, 100) < chanceDamaged;

        spawnList.Add(new SpawnObject(new Vector2(x, y), spawnObjectType, platformSizeX, damaged));

        currentSpawned += 1;
    }

    void Start()
    {
        GenerateItem();
    }

    void Update(){
        if(current < spawnList.Count){
            if(transform.position.x > spawnList[current].pos.x){
                GameObject newObject = Instantiate(spawnList[current].spawnObjectType == SpawnObjectType.Platform ? platformPrefab : ropePrefab, spawnList[current].pos, Quaternion.identity, spawnObject.transform);
                if(spawnList[current].spawnObjectType == SpawnObjectType.Platform){
                    newObject.transform.localScale = new Vector3(spawnList[current].platformSizeX, newObject.transform.localScale.y, newObject.transform.localScale.z);
                    newObject.GetComponent<Platform>().damaged = spawnList[current].damaged;
                } else {
                    newObject.GetComponent<Rope>().damaged = spawnList[current].damaged;
                }
                current += 1;
                GenerateItem();
            }
        }
    }
}
