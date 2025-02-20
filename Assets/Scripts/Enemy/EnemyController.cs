using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
public class EnemyController : EnemyValues
{
    [SerializeField] private UnityEvent EnemyKOEvent;
    private Rigidbody2D enemyRB;

    public Slider slider; // Slider for enemy health
    private Animator animator;
    private Transform target; // The target for the enemy
    private bool attack = false, attacktiming=false, enemyMove;
    private float timing, stopattack, attackDelay;
    public bool testingTools = false;

    void Start()
    {
        enemyHealth = enemyMaxHealth;
        SetMaxHealth(enemyMaxHealth);

        enemyRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        target = FindObjectOfType<PlayerMovement>().transform;
        FollowPlayer();
        enemyMove = true;
    }

    void Update()
    {
        if (testingTools)
            if (Input.GetKeyDown(KeyCode.G)) // Testing for health
                TakeDamage(10);
        if (Combat.animator.GetBool("isGrapple"))
        {
            animator.SetBool("isGrapple", true);
            enemyRB.velocity = Vector3.zero;
            StopPlayer();
        }
        if (!Combat.animator.GetBool("isGrapple"))
        {
            animator.SetBool("isGrapple",false);
        }
        if (enemyHealth <= 0)
        {
            animator.SetBool("isKnocked", true);
        }
        if (!animator.GetBool("isKnocked") && !animator.GetBool("isGrapple"))
        {
            if (enemyMove == true)
            {
                FollowPlayer();
            }
            if (attackDelay > 0 && attack == false)
            {
                attackDelay -= Time.deltaTime;
            }
            if (attack == true)
            {

                animator.SetBool("isAttacking", true);
                timing += Time.deltaTime;
                if (timing >= 1)
                {
                    attack = false;
                    animator.SetBool("isAttacking", false);
                    timing = 0;
                }
            }
            if (attacktiming == true)
            {
                stopattack += Time.deltaTime;
                if (stopattack >= 4)
                {
                    stopattack = 0;
                    attack = false;
                    attacktiming = false;
                }
            }
        }
    }
    public void DevTools()
    {
        testingTools = true;
    }
    public void TakeDamage(int damage)
    {

        enemyHealth -= damage;
        Debug.Log(enemyHealth);
        if (enemyHealth <= 0)
        {
            Debug.Log("Knockout");
            //Will set the Enemyknockout panel active once 
            //the enemy's KOBar reaches bellow zero and will destroy
            //the enemy object
            Destroy(gameObject);
            EnemyKOEvent.Invoke();

        }
        SetHealth(enemyHealth);
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
    public void FollowPlayer()
    {
        if (!animator.GetBool("isKnocked") && !animator.GetBool("isGrapple"))
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", (target.position.x - transform.position.x));
            animator.SetFloat("moveY", (target.position.y - transform.position.y));
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }
    public void StopPlayer()
    {
        if (!animator.GetBool("isKnocked"))
        {
            animator.SetBool("isMoving", false);
            AttackPlayer();
        }
    }
    public void AttackPlayer()
    {
        if (!animator.GetBool("isKnocked"))
        {
            if (stopattack == 0)
            {
                attack = true;
                attacktiming = true;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!animator.GetBool("isKnocked"))
        {
            if (collider.gameObject.tag == "Player")
            {
                PlayerHealth player = collider.GetComponent<PlayerHealth>();
                if (attack == true)
                    if (attackDelay <= 0)
                    {
                        player.TakeDamage(enemyDamage);
                        attackDelay = 2;
                    }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {if (collision.gameObject.tag == "Player")
        {
            FollowPlayer();
            enemyMove = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            StopPlayer();
            enemyMove = false;
        }
    }
}
