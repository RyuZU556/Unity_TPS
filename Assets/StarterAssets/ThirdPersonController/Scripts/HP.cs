using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HP : MonoBehaviour
{
    private Animator animator;
    public int hitPoint = 100;  //HP
    //public AudioClip shotSound;
    bool isdeath;
    bool isdamage;
    float seconds;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //HPが0になったときに敵を破壊する
        if (hitPoint <= 0)
        {
            animator.SetBool("isdeath",true);
            //AudioSource.PlayClipAtPoint(shotSound, transform.position);
            Destroy(gameObject, 2.5f);
            seconds += Time.deltaTime;
            if (seconds >= 2)
            {
                seconds = 0;
                SceneManager.LoadScene("GameClear");
            }
        }
    }

    //ダメージを受け取ってHPを減らす関数
    public void Damage(int damage)
    {
        //animator.SetBool("isdamage", true);
        //受け取ったダメージ分HPを減らす
        hitPoint -= damage;
    }
}
