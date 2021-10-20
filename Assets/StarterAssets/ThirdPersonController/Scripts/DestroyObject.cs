using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public int damage;          //�����������ʖ��̃_���[�W��
    private GameObject enemy;   //�G�I�u�W�F�N�g
    private HP hp;              //HP�N���X

    void Start()
    {
        enemy = GameObject.Find("Enemy");   //�G�����擾
        hp = enemy.GetComponent<HP>();      //HP�����擾
    }

    void OnTriggerEnter(Collider other)
    {

        //�Ԃ������I�u�W�F�N�g��Tag��Shell�Ƃ������O�������Ă������Ȃ�΁i�����j.
        if (other.CompareTag("Shell"))
        {
            //HP�N���X��Damage�֐����Ăяo��
            hp.Damage(damage);

            Debug.Log("�G�ƒe���Փ˂��܂���!!");

            //�Ԃ����Ă����I�u�W�F�N�g��j�󂷂�.
            Destroy(other.gameObject);
        }
    }
}
