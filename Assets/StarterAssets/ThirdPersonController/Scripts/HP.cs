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
        //HP��0�ɂȂ����Ƃ��ɓG��j�󂷂�
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

    //�_���[�W���󂯎����HP�����炷�֐�
    public void Damage(int damage)
    {
        //animator.SetBool("isdamage", true);
        //�󂯎�����_���[�W��HP�����炷
        hitPoint -= damage;
    }
}
