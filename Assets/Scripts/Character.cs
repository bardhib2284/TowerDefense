using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class Character : MonoBehaviour
{
    [Serialize]
    public string Name;
    [Serialize]
    public string Description;
    public string Skill_1_Name { get; set; }

    Rigidbody2D rigidbody2D;
    public GameManager Manager;



    //PREFABS

    public GameObject Projectile;
    public Transform PlaceholderForProjectile;
    public Transform MergeHolder;



    //STATS
    public float Damage;
    public float AttackSpeed;
    public float AttackRate;
    float distance = 10f;
    public int MergeLevel;
    private void Awake() {
        Manager = FindObjectOfType<GameManager>();
        rigidbody2D = GetComponentInParent<Rigidbody2D>();
        rigidbody2D.isKinematic = true;
    }
    Vector3 newDir;
    private void Start() {
        //MergeLevel = 2;
        AttackSpeed = AttackSpeed / MergeLevel;
        MergeHolder.GetChild(MergeLevel-1).gameObject.SetActive(true);
        Damage += UnityEngine.Random.Range(20, 120);
    }
    public EnemyAI CurrentTarget;
    private void Update() {
       
        if (Manager.EnemyList.Count > 0) {

            //there are enemies alive
            var enemy = Manager.EnemyList.FirstOrDefault();
            if (enemy == null) { return; }
            if (enemy.GetComponent<EnemyAI>().IsDead()) {
                return;
            }
            Vector3 targ = enemy.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            angle += 90;
            //this.transform.rotation.SetLookRotation(enemy.transform.position);
            //this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            this.transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            //this.transform.position = new Vector3(0.12f,0.04f, 0);
            AttackRate -= Time.deltaTime;
            if (AttackRate <= 0) {
                AttackRate = AttackSpeed;
                CurrentTarget = enemy.GetComponent<EnemyAI>();
                AttackEnemy(CurrentTarget);
            }
        }
    }

    private void AttackEnemy(EnemyAI enemy) {
        GetComponentInChildren<Animator>().SetTrigger("Attack");
        GetComponentInChildren<Animator>().SetFloat("AttackSpeed", MergeLevel);
    }

    public void ShootProjectile() {
        var projectile = Instantiate(Projectile, PlaceholderForProjectile);
        projectile.GetComponent<Projectile>().Target = CurrentTarget;
        projectile.GetComponent<Projectile>().Parent = this;
    }
}
