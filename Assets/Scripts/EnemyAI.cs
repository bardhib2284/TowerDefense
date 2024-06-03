using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour 
{
    // Adjust the speed for the application.
    public float speed = 1.0f;
    public List<Transform> Trails;
    public Transform TargetPoint;
    public int CurrentPoint;
    public bool Dead;

    public float Health;
    public float MaxHealth;

    public float DistanceToLastPoint;



    public GameObject TextDamagePrefab;
    private void Awake() {
        FindObjectOfType<GameManager>().EnemyList.Add(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        Trails = FindObjectOfType<GameManager>().Trails;
        Health = MaxHealth;
        CurrentPoint = 0;
        GameManager = FindObjectOfType<GameManager>();
    }

    public void TakeDamage(float dmg) {
        var textDmgPrefat = Instantiate(TextDamagePrefab, transform.position,Quaternion.identity);
        if (dmg > 100) {
            textDmgPrefat.transform.GetComponentInChildren<TextMeshPro>().enableVertexGradient = true;
            textDmgPrefat.transform.GetComponentInChildren<TextMeshPro>().fontStyle = FontStyles.Bold;
            textDmgPrefat.transform.GetComponentInChildren<TextMeshPro>().colorGradient = new VertexGradient(new Color(204, 93, 0),new Color(255,250,0),new Color(241, 65, 65), new Color(188, 108, 27));
            textDmgPrefat.transform.position = new Vector3(textDmgPrefat.transform.position.x + Random.Range(0, 0.32f), textDmgPrefat.transform.position.y + Random.Range(0, 0.32f), -3);
            textDmgPrefat.transform.transform.localScale = new Vector3(2f, 2f, 2f);
        }
        else {
            textDmgPrefat.transform.position = new Vector3(textDmgPrefat.transform.position.x + Random.Range(0, 0.12f), textDmgPrefat.transform.position.y + Random.Range(0, 0.12f), -1);
            textDmgPrefat.transform.GetComponentInChildren<TextMeshPro>().color = Color.white;
        }
        textDmgPrefat.transform.GetComponentInChildren<TextMeshPro>().text = dmg.ToString();
        if (Dead) return;
        Health -= dmg;
        StartCoroutine(HealthUI());
        if(Health < 0 ) Dead = true;
    }

    public IEnumerator HealthUI() {
        if (Dead) yield return null;
        if(transform.GetChild(1) != null) {
            if(transform.GetChild(1).GetChild(1) != null) {
                this.transform.GetChild(1).GetChild(1).transform.localScale = new Vector3(Health / MaxHealth, 1, 1);
                yield return new WaitForSeconds(0.2f);
                this.transform.GetChild(1).GetChild(2).transform.localScale = new Vector3(Health / MaxHealth, 1, 1);
            }
        }
    }

    public bool IsDead() {
        return Dead;
    }
    GameManager GameManager;
    // Update is called once per frame
    void Update() {
        if (Dead) {
            FindObjectOfType<GameManager>().EnemyList.Remove(this.gameObject);
            Destroy(gameObject);
        }
        else {
            if(CurrentPoint >= GameManager.CurrentLastTrail) {
                GameManager.CurrentLastTrail = CurrentPoint;
                DistanceToLastPoint = Vector3.Distance(this.transform.position, Trails[CurrentPoint].transform.position);
            }

        }
        if(Trails.Count > 0) {
            TargetPoint = Trails[CurrentPoint];
            // Move our position a step closer to the target.
            var step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, TargetPoint.position, step);

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, TargetPoint.position) < 0.001f) {
                // Swap the position of the cylinder.
                CurrentPoint++;
                if(CurrentPoint >= Trails.Count) {
                    FindObjectOfType<GameManager>().EnemyList.Remove(this.gameObject);
                    Destroy(gameObject);
                    return;
                }
                TargetPoint = Trails[CurrentPoint];

            }
        }
        
    }
}
