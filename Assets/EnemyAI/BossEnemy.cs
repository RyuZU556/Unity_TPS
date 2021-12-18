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
        // 弾に当たったら
        if (other.gameObject.CompareTag("Shell"))
        {
            // エフェクトを発生
            //GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

            // 0.5秒後にエフェクトを消す
            //Destroy(effect, 0.5f);

            // 敵のHPを20ずつ減少
            BossEnemyHP -= 20;

            //animator.SetTrriger("isDamage");

            // ミサイルを削除
            Destroy(other.gameObject);

            // 敵のHPが０になったら敵オブジェクトを破壊
            if (BossEnemyHP == 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                animator.SetTrigger("death");
                // 4.5秒後に実行
                Invoke("DeleteEnemy", 4.5f);

                // 破壊の効果音
                //AudioSource.PlayClipAtPoint(destroySound, transform.position);
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // プレイヤーの弾が当たったら
        if (hit.gameObject.tag == "Shell")
        {
            //スライダー(HP)を-5
            slider.value -= 5.0f;
            // ログを表示
            Debug.Log("Hit");
            // HPが0になったら
            if (slider.value <= 0f)
            {
                // Deathアニメーション再生
                animator.SetTrigger("death");
                // 4秒後に実行
                Invoke("SceneChange", 4.0f);
            }
        }
    }

    void DeleteEnemy()
    {
        // 親オブジェクトを破壊
        Destroy(transform.root.gameObject);
        // ゲームクリア画面移行
        SceneManager.LoadScene("GameClear");
    }
}
