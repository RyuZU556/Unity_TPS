using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    private Animator animator;
    public int hitPoint = 100;  //HP
    //bool is_death;

    void Start()
    {
        //animator = GetComponent<Animator>();
    }

    void Update()
    {
        //HPが0になったときに敵を破壊する
        if (hitPoint <= 0)
        {
            //animator.SetTrigger(is_death);
            Destroy(gameObject);
        }
    }

    //ダメージを受け取ってHPを減らす関数
    public void Damage(int damage)
    {
        //受け取ったダメージ分HPを減らす
        hitPoint -= damage;
    }
}
