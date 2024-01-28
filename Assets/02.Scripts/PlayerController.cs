using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    Vector3 moveDir;
    Vector3 rotateDir;
    Rigidbody rigid;
    public float playerCurrentHP;
    public float playerMaxHP;
    public float moveSpeed;
    public float rotSpeed;
    public GameObject playerCrossHair;
    public ParticleSystem bloodParticle;
    public GameObject[] playerWeaponIcons;
    public GameObject[] playerWeapons;
    public GameObject bloodScreen;
    public ParticleSystem[] ejectParticle;
    public ParticleSystem[] fireParticle;
    public ParticleSystem sparkParticle;
    public ParticleSystem sparkAndHoleParticle;
    public GameObject ItemUI;
    public Image playerHPBar;
    public Image playerBulletBar;
    public Text playerBulletText;
    public GameObject ItemText;
    public Transform firePos;
    public int maxBullet;
    public float currentBullet;
    float crosshairPower;
    bool[] getWeapon;
    int currentWeapon;
    bool isFire;
    bool isReload;
    bool isMove;
    bool isSwap;
    bool isAim;
    Animator animator;
  

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        getWeapon = new bool[playerWeapons.Length];
        getWeapon[0] = true;
        ItemUI.SetActive(false);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Locked;
        playerCurrentHP = playerMaxHP;
        playerHPBar.color = Color.green;
        crosshairPower = 1f;
        playerBulletText.text = maxBullet.ToString();
        for(int i = 0;i<getWeapon.Length;i++)
        {
            if (!getWeapon[i])
                playerWeaponIcons[i].SetActive(false);
        }
        playerWeaponIcons[0].GetComponent<Image>().color = Color.gray;

    }
    // Update is called once per frame
    void Update()
    {

        PlayerMove();
        if (crosshairPower > 1)
            crosshairPower -= Time.deltaTime;

        playerCrossHair.transform.localScale = Vector3.one * crosshairPower;
        if (!isFire && !isReload && !isSwap)
        {
            PlayerMoveAnimation();
            ShotItemRay();
            if(Input.GetMouseButtonDown(1))
                StartCoroutine(PlayerAimCorutine());

            if (Input.GetMouseButtonDown(0) && currentBullet > 0)
                StartCoroutine(PlayerFireCorutine());

            if (Input.GetKeyDown(KeyCode.R) && maxBullet > 0 && currentBullet < 10|| (Input.GetMouseButtonDown(0) && currentBullet == 0 && maxBullet > 0 && currentBullet < 10))
                StartCoroutine(PlayerReloadCorutine());

            if (Input.GetKeyDown(KeyCode.Alpha1) && currentWeapon != 0)
            {
                StartCoroutine(PlayerSwapCorutine(0));
                currentWeapon = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && currentWeapon != 1 && getWeapon[1])
            {
                StartCoroutine(PlayerSwapCorutine(1));
                currentWeapon = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && currentWeapon != 2 && getWeapon[2])
            {
                StartCoroutine(PlayerSwapCorutine(2));
                currentWeapon = 2;
            }
;        }
    }

   void ShotItemRay()
    {
        RaycastHit target;
        bool hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out target, 2.5f, LayerMask.GetMask("Item"));
        if (hit)
        {
            ItemUI.SetActive(true);
            switch(target.transform.gameObject.name)
            {
                case "Item_Rifle":
                    ItemText.GetComponent<Text>().text = "공격력이 강화된 소총입니다.";
                    if(Input.GetKeyDown(KeyCode.G))
                    {
                        Camera.main.transform.DOShakeRotation(0.5f, 1f);
                        getWeapon[1] = true;
                        playerWeaponIcons[1].SetActive(true);
                        Destroy(target.transform.gameObject);
                    }
                    break;

                case "Item_Shotgun":
                    ItemText.GetComponent<Text>().text = "여러발의 총알이 발사되는 샷건입니다.";
                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        Camera.main.transform.DOShakeRotation(0.5f, 1f);
                        getWeapon[2] = true;
                        playerWeaponIcons[2].SetActive(true);
                        Destroy(target.transform.gameObject);
                    }
                    break;

                case "Item_Bullet":
                    ItemText.GetComponent<Text>().text = "총알을 10발 획득합니다.";
                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        Camera.main.transform.DOShakeRotation(0.5f, 1f);
                        maxBullet += 10;
                        playerBulletText.text = maxBullet.ToString();

                        Destroy(target.transform.gameObject);
                    }
                    break;
            }
            
        }
        else
            ItemUI.SetActive(false);

    }

    void PlayerMove()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");
        var h = Input.GetAxis("Mouse X");
        var v = Input.GetAxis("Mouse Y");


        moveDir = (moveDir.z * Vector3.forward + Vector3.right * moveDir.x).normalized;
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

        rotateDir.x += h * Time.deltaTime * rotSpeed;
        rotateDir.y += v * Time.deltaTime * rotSpeed;
        rotateDir.y = Mathf.Clamp(rotateDir.y, -45f, 45f);
        transform.eulerAngles = new Vector3(-rotateDir.y, rotateDir.x, 0f);

        if (moveDir.magnitude > 0)
        {
            isMove = true;
            Camera.main.DOShakeRotation(0.1f, 0.1f);
        }
        else if (moveDir.magnitude == 0)
            isMove = false;


    }

    IEnumerator PlayerAimCorutine()
    {
        isAim = true;
        while(true)
        {
            Camera.main.transform.localPosition = new Vector3(0.136f, 1.75f, 0.16f);
            Camera.main.fieldOfView = 41f;
            if(Input.GetMouseButtonUp(1) || isReload || isSwap)
            {
                Camera.main.transform.localPosition = new Vector3(-0.2f, 1.78f, 0.07f);
                Camera.main.fieldOfView = 58f;
                isAim = false;
                break;
            }
            yield return null;
        }
    }
    void PlayerMoveAnimation()
    {
        if (moveDir.magnitude > 0)
            animator.SetBool("Walk", true);
        else
            animator.SetBool("Walk", false);

    }
    IEnumerator PlayerReloadCorutine()
    {
        isReload = true;
        if (currentWeapon == 2)
            animator.SetTrigger("ReloadShotgun");
        else
            animator.SetTrigger("ReloadRifle");

        Camera.main.transform.DOShakeRotation(0.5f, 1f);

        yield return new WaitForSeconds(1f);
        if (maxBullet >= 10)
            currentBullet = 10;
        else if(maxBullet < 10)
        {
            var save = 10 - maxBullet;
            currentBullet = save;
        }
        playerBulletBar.fillAmount = currentBullet * 0.1f;
        playerBulletText.text = maxBullet.ToString();
        isReload = false;
    }

    IEnumerator PlayerFireCorutine()
    {
        currentBullet--;
        maxBullet--;
        crosshairPower += 0.5f;
        playerBulletBar.fillAmount = currentBullet * 0.1f;
        playerBulletText.text = maxBullet.ToString();
        isFire = true;
        RaycastHit target;
        bool hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out target, 30f, LayerMask.GetMask("Enemy"));
        if (hit)
        {

            target.transform.GetComponent<EnemyController>().SetHP((currentWeapon + 1) * -10);
            var blood = Instantiate(bloodParticle, target.transform);
            blood.transform.position = target.point;
            blood.transform.LookAt(firePos.position);
            Destroy(blood.gameObject, 0.5f);
            var spark = Instantiate(sparkParticle, target.transform);
            spark.transform.position = target.point;
            spark.transform.localScale = new Vector3(3f, 3f, 3f);
            spark.transform.LookAt(firePos.position);
            Destroy(spark.gameObject,0.5f);



        }
        hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out target, 30f, LayerMask.GetMask("Obstacle"));
        if (hit)
        {
            var spark = Instantiate(sparkAndHoleParticle, target.transform);
            spark.transform.position = target.point;
            spark.transform.LookAt(firePos.position);
            spark.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Destroy(spark.gameObject, 0.5f);


        }
        Camera.main.transform.DOShakeRotation(0.3f, 0.3f);
        if (moveDir.magnitude > 0)
        {
            if (currentWeapon == 2)
                animator.SetTrigger("WalkFireShotgun");
            else
                animator.SetTrigger("WalkFireRifle");
        }
        else
        {
            if (currentWeapon == 2)
                animator.SetTrigger("IdleFireShotgun");
            else
                animator.SetTrigger("IdleFireRifle");
        }

        ejectParticle[currentWeapon].Play();
        fireParticle[currentWeapon].Play();
        yield return new WaitForSeconds(0.7f);
        isFire = false;
    }

    IEnumerator PlayerSwapCorutine(int index)
    {
        isSwap = true;
        if (currentWeapon == 2)
            animator.SetTrigger("ReloadShotgun");
        else
            animator.SetTrigger("ReloadRifle");
        Camera.main.transform.DOShakeRotation(0.5f, 1f);

        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < playerWeapons.Length; i++)
        {
            if (i != index)
            {
                playerWeapons[i].SetActive(false);
                playerWeaponIcons[i].GetComponent<Image>().color = new Color(0.07f, 0.44f, 0.36f);

            }
            else
            {
                playerWeapons[i].SetActive(true);
                playerWeaponIcons[i].GetComponent<Image>().color = Color.gray;
            }

        }
        yield return new WaitForSeconds(1f);


 
        isSwap = false;
    }
    void SetHP(float hp)
    {
        playerCurrentHP -= hp;
        var hpp = playerCurrentHP / playerMaxHP;
        if (hpp > 0.5f)
            playerHPBar.color = Color.green;
        else if (hpp > 0.25f && hpp <= 0.5f)
            playerHPBar.color = new Color(0.9f, 0.37f, 0.15f, 1.0f);
        else
            playerHPBar.color = Color.red;

        playerHPBar.fillAmount = hpp;

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ATTACK"))
        {
            SetHP(10);
            StartCoroutine(BloodCorutine());
        }
    }

    IEnumerator BloodCorutine()
    {
        float alpha = 1;
        bloodScreen.SetActive(true);
        while(true)
        {
            alpha -= Time.deltaTime;
            bloodScreen.GetComponent<Image>().color = new Color(255f, 0f, 0f, alpha);
            yield return null;

            if(alpha < 0)
            {
                bloodScreen.SetActive(false);
                break;
            }    
        }
    }


}
