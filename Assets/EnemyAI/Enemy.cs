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
        // �������Ԃ���������ɁuMissile�v�Ƃ����^�O�iTag�j�����Ă�����A
        if (other.gameObject.CompareTag("Shell"))
        {
            // �G�t�F�N�g�𔭐�������
            //GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

            // 0.5�b��ɃG�t�F�N�g������
            //Destroy(effect, 0.5f);

            // �G��HP���P������������
            enemyHP -= 20;

            //animator.SetTrriger("isDamage");

            // �~�T�C�����폜����
            Destroy(other.gameObject);

            // �G��HP���O�ɂȂ�����G�I�u�W�F�N�g��j�󂷂�B
            if (enemyHP == 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                animator.SetTrigger("death");

                // �e�I�u�W�F�N�g��j�󂷂�i�|�C���g�G���̎g�������o���悤�I�j
                //Destroy(transform.root.gameObject);

                // �j��̌��ʉ����o��
                //AudioSource.PlayClipAtPoint(destroySound, transform.position);
            }
        }
    }
}
