using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ★追加
using UnityEngine.UI;

public class ShotShell : MonoBehaviour
{
    [SerializeField] private BulletProjectile Ammo;
    public GameObject shellPrefab;
    //public float shotSpeed;
    //public AudioClip shotSound;
    public int shotCount;
    // ★追加
    public Text AmmoText;

    private float timeBetweenShot = 0.35f;
    private float timer;

    private StarterAssetsInputs starterAssetsInputs;

    // ★追加
    // Startの「S」は大文字なので注意！
    void Start()
    {
        AmmoText.text = "Ammo：" + shotCount;
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

            // ★追加
            AmmoText.text = "Ammo：" + shotCount;

        //    timer = 0.0f;

        //    GameObject shell = Instantiate(shellPrefab, transform.position, Quaternion.identity);
        //    Rigidbody shellRb = shell.GetComponent<Rigidbody>();
        //    shellRb.AddForce(transform.forward * shotSpeed);
        //    Destroy(shell, 3.0f);
        //    AudioSource.PlayClipAtPoint(shotSound, transform.position);
        }
    }
}
