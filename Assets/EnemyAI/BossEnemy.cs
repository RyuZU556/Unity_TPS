using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour
{
    public GameObject projectile;
    public Transform projectilePoint;
    public int BossEnemyHP;
    //public GameObject effectPrefab;
    //public AudioClip destroySound;

    public Animator animator;

    Slider slider;

    private void Start()
    {
        slider = GameObject.Find("BossHP").GetComponent<Slider>();
        slider.value = 1;
    }

    public void Shoot()
    {
        Rigidbody rb = Instantiate(projectile, projectilePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 30f, ForceMode.Impulse);
        rb.AddForce(transform.up * 7, ForceMode.Impulse);
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
            BossEnemyHP -= 20;

            //animator.SetTrriger("isDamage");

            // ミサイルを削除する
            Destroy(other.gameObject);

            // 敵のHPが０になったら敵オブジェクトを破壊する。
            if (BossEnemyHP == 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                animator.SetTrigger("death");

                Invoke("DeleteEnemy", 4.5f);

                // 破壊の効果音を出す
                //AudioSource.PlayClipAtPoint(destroySound, transform.position);
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Shell")
        {
            slider.value -= 5.0f;
            Debug.Log("Hit"); // ログを表示する
        }
    }

    void DeleteEnemy()
    {
        // 親オブジェクトを破壊する（ポイント；この使い方を覚えよう！）
        Destroy(transform.root.gameObject);
        SceneManager.LoadScene("GameClear");
    }
}
