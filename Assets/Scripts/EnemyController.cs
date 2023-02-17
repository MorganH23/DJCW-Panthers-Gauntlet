using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private Transform target;
    private bool attack = false, attacktiming=false;
    [SerializeField] private float speed;
    [SerializeField] private float range=1.15f;
    private float timing, stopattack;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        target = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(target.position, transform.position) > range)
        FollowPlayer();
        else
        StopPlayer();
        if (attack == true)
        {
            animator.SetBool("isAttacking", true);
            timing += Time.deltaTime;
            if(timing >= 2)
            {
                attack = false;
                animator.SetBool("isAttacking", false);
                timing = 0;
            }
        }
        if (attacktiming == true)
        {
            stopattack+=Time.deltaTime;
            if(stopattack >= 6)
            {
                stopattack=0;
                attack = false;
                attacktiming=false;
            }
        }
    }
    public void FollowPlayer()
    {
        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", (target.position.x - transform.position.x));
        animator.SetFloat("moveY", (target.position.y - transform.position.y));
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }
    public void StopPlayer()
    {
        animator.SetBool("isMoving", false);
        AttackPlayer();
    }
    public void AttackPlayer()
    {
        if (stopattack == 0)
        {
            attack = true;
            attacktiming = true;
        }
    }
}
