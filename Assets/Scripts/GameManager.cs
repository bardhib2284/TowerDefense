using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public List<GameObject> CharacterPrefabs = new List<GameObject>();

    [Serialize]
    public List<SpawnPoint> SpawnPoints;
    public GameObject Trail;
    public List<Transform> Trails;

    //ENEMIES
    public SpawnPoint EnemiesSpawnPoint;
    public GameObject GreenMonster;
    public GameObject RedMonster;
    public List<GameObject> EnemyList;





    public List<GameObject> CharacterList;
    // Start is called before the first frame update
    void Start()
    {
        var spawnPointsFound = FindObjectsOfType<SpawnPoint>();
        foreach(var spawnPoint in spawnPointsFound) {
            if (spawnPoint.transform.tag == "EnemySpawnPoint")
                continue;
            SpawnPoints.Add(spawnPoint);
        }
        
        for(int i = 0; i < Trail.transform.childCount; i++) 
        {
            Trails.Add(Trail.transform.GetChild(i));
        }


        EnemiesSpawnPoint = GameObject.FindWithTag("EnemySpawnPoint").GetComponent<SpawnPoint>();
    }

    public float LastDistanceKnownForChildren;
    public float CurrentLastTrail;
    // Update is called once per frame
    void Update()
    {
        var enemyListTemp = new List<GameObject>(EnemyList);
        foreach(var enemy in enemyListTemp) {
            if(enemy != null) {
                if (LastDistanceKnownForChildren > 0) {
                    var enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI.DistanceToLastPoint == 0) continue;
                    if (enemyAI.DistanceToLastPoint < LastDistanceKnownForChildren) {
                        LastDistanceKnownForChildren = enemyAI.DistanceToLastPoint;
                        EnemyList.Remove(enemy);
                        EnemyList.Insert(0, enemy);
                        LastDistanceKnownForChildren = 0;
                    }
                }
                else
                    LastDistanceKnownForChildren = enemy.GetComponent<EnemyAI>().DistanceToLastPoint;
            }
        }

        if(Input.GetKeyDown(KeyCode.S)) {
            foreach(var character in CharacterList) {
                if (character != null) {
                    var characterComponent = FindObjectOfType<Character>();
                    characterComponent.AttackSpeed -= characterComponent.AttackSpeed * 0.20f;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            StartCoroutine(SpawnMonsters());
        }

    }

    private void FixedUpdate() {
        
    }

    public IEnumerator SpawnMonsters() {
        yield return new WaitForSeconds(1.5f);
        int i = 0;
        while (true) {
            i++;
            if(i % 2 == 0) {
                Instantiate(RedMonster, EnemiesSpawnPoint.transform);
            }
            else
                Instantiate(GreenMonster, EnemiesSpawnPoint.transform);
            yield return new WaitForSeconds(1.5f);
        }
    }

    public void SpawnCharacter() 
    {
        if(SpawnPoints.Count > 0) {
            if(CharacterList.Count < SpawnPoints.Count) {
                var unOccupiedSpawnPoints = new List<SpawnPoint>(SpawnPoints.Where(x => x.Occupied == false));
                var currentRandomSpawnNumber = UnityEngine.Random.Range(0, unOccupiedSpawnPoints.Count);
                var currChar = Instantiate(CharacterPrefabs[UnityEngine.Random.Range(0,CharacterPrefabs.Count)], unOccupiedSpawnPoints[currentRandomSpawnNumber].spawnPoint.transform);
                unOccupiedSpawnPoints[currentRandomSpawnNumber].Occupied = true;
                CharacterList.Add(currChar);
            }
        }
    }

    public void MergeCharacters(GameObject current, GameObject target,Transform whereToSpawn,int levelToSpawn) {
        CharacterList.Remove(current);
        CharacterList.Remove(target);
        current.gameObject.GetComponentInParent<SpawnPoint>().Occupied = false;
        Destroy(current);
        Destroy(target);
        var currChar = Instantiate(CharacterPrefabs[UnityEngine.Random.Range(0, CharacterPrefabs.Count)], whereToSpawn);
        currChar.GetComponentInChildren<Character>().MergeLevel = levelToSpawn;
        CharacterList.Add(currChar);

    }
}
