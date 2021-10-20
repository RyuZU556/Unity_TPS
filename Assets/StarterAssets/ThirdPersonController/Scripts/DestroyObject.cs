using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public int damage;          //当たった部位毎のダメージ量
    private GameObject enemy;   //敵オブジェクト
    private HP hp;              //HPクラス

    void Start()
    {
        enemy = GameObject.Find("Enemy");   //敵情報を取得
        hp = enemy.GetComponent<HP>();      //HP情報を取得
    }

    void OnTriggerEnter(Collider other)
    {

        //ぶつかったオブジェクトのTagにShellという名前が書いてあったならば（条件）.
        if (other.CompareTag("Shell"))
        {
            //HPクラスのDamage関数を呼び出す
            hp.Damage(damage);

            Debug.Log("敵と弾が衝突しました!!");

            //ぶつかってきたオブジェクトを破壊する.
            Destroy(other.gameObject);
        }
    }
}
