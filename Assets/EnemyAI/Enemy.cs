using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //public int enemyHP = 100;
    public GameObject projectile;
    public Transform projectilePoint;

    public Animator animator;

    public void Shoot()
    {
        Rigidbody rb = Instantiate(projectile, projectilePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 30f, ForceMode.Impulse);
        rb.AddForce(transform.up * 7,ForceMode.Impulse);
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "Ground")
    //    {
    //        Destroy(gameObject);
    //    }
    //    else if (other.gameObject.tag == "Player")
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //public void TakeDamage(int damageAmount)
    //{
    //    enemyHP -= damageAmount;
    //    if (enemyHP <= 0) 
    //    {
    //        animator.SetTrigger("death");
    //        GetComponent<CapsuleCollider>().enabled = false;
    //    }
    //    else
    //    {
    //        animator.SetTrigger("damage");
    //    }
    //}
}
