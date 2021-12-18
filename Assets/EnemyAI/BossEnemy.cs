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
        // �e�ɓ���������
        if (other.gameObject.CompareTag("Shell"))
        {
            // �G�t�F�N�g�𔭐�
            //GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

            // 0.5�b��ɃG�t�F�N�g������
            //Destroy(effect, 0.5f);

            // �G��HP��20������
            BossEnemyHP -= 20;

            //animator.SetTrriger("isDamage");

            // �~�T�C�����폜
            Destroy(other.gameObject);

            // �G��HP���O�ɂȂ�����G�I�u�W�F�N�g��j��
            if (BossEnemyHP == 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                animator.SetTrigger("death");
                // 4.5�b��Ɏ��s
                Invoke("DeleteEnemy", 4.5f);

                // �j��̌��ʉ�
                //AudioSource.PlayClipAtPoint(destroySound, transform.position);
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // �v���C���[�̒e������������
        if (hit.gameObject.tag == "Shell")
        {
            //�X���C�_�[(HP)��-5
            slider.value -= 5.0f;
            // ���O��\��
            Debug.Log("Hit");
            // HP��0�ɂȂ�����
            if (slider.value <= 0f)
            {
                // Death�A�j���[�V�����Đ�
                animator.SetTrigger("death");
                // 4�b��Ɏ��s
                Invoke("SceneChange", 4.0f);
            }
        }
    }

    void DeleteEnemy()
    {
        // �e�I�u�W�F�N�g��j��
        Destroy(transform.root.gameObject);
        // �Q�[���N���A��ʈڍs
        SceneManager.LoadScene("GameClear");
    }
}
