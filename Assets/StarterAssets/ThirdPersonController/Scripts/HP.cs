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
        //HP��0�ɂȂ����Ƃ��ɓG��j�󂷂�
        if (hitPoint <= 0)
        {
            //animator.SetTrigger(is_death);
            Destroy(gameObject);
        }
    }

    //�_���[�W���󂯎����HP�����炷�֐�
    public void Damage(int damage)
    {
        //�󂯎�����_���[�W��HP�����炷
        hitPoint -= damage;
    }
}
