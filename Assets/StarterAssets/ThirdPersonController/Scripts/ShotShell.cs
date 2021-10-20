using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ���ǉ�
using UnityEngine.UI;

public class ShotShell : MonoBehaviour
{
    [SerializeField] private BulletProjectile Ammo;
    public GameObject shellPrefab;
    //public float shotSpeed;
    //public AudioClip shotSound;
    public int shotCount;
    // ���ǉ�
    public Text AmmoText;

    private float timeBetweenShot = 0.35f;
    private float timer;

    private StarterAssetsInputs starterAssetsInputs;

    // ���ǉ�
    // Start�́uS�v�͑啶���Ȃ̂Œ��ӁI
    void Start()
    {
        AmmoText.text = "Ammo�F" + shotCount;
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (starterAssetsInputs.shoot)
        {
            if (shotCount < 1)
            {
                return;
            }

            shotCount -= 1;

            // ���ǉ�
            AmmoText.text = "Ammo�F" + shotCount;

        //    timer = 0.0f;

        //    GameObject shell = Instantiate(shellPrefab, transform.position, Quaternion.identity);
        //    Rigidbody shellRb = shell.GetComponent<Rigidbody>();
        //    shellRb.AddForce(transform.forward * shotSpeed);
        //    Destroy(shell, 3.0f);
        //    AudioSource.PlayClipAtPoint(shotSound, transform.position);
        }
    }
}
