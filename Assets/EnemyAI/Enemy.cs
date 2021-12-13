using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject projectile;
    public Transform projectilePoint;
    public int enemyHP;
    //public GameObject effectPrefab;
    //public AudioClip destroySound;

    public Animator animator;

    public void Shoot()
    {
        Rigidbody rb = Instantiate(projectile, projectilePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 30f, ForceMode.Impulse);
        rb.AddForce(transform.up * 7,ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        // もしもぶつかった相手に「Missile」というタグ（Tag）がついていたら、
        if (other.gameObject.CompareTag("Shell"))
        {
            // エフェクトを発生させる
            //GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

            // 0.5秒後にエフェクトを消す
            //Destroy(effect, 0.5f);

            // 敵のHPを１ずつ減少させる
            enemyHP -= 20;

            //animator.SetTrriger("isDamage");

            // ミサイルを削除する
            Destroy(other.gameObject);

            // 敵のHPが０になったら敵オブジェクトを破壊する。
            if (enemyHP == 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                animator.SetTrigger("death");

                // 親オブジェクトを破壊する（ポイント；この使い方を覚えよう！）
                //Destroy(transform.root.gameObject);

                // 破壊の効果音を出す
                //AudioSource.PlayClipAtPoint(destroySound, transform.position);
            }
        }
    }
}
