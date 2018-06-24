using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(AudioSource))]
public class Player_Shoot : NetworkBehaviour
{
    public int damage = 0;
    public string gunName = "";
    public float shootSpeed = 0;
    public float reloadTime = 1;
    private float nowReloadTime;
    public int maxAmmo = 0;
    private int nowAmmo = 0;
    public int audioClipIndex = 0;
    public bool canEnlarge;
    public int enlargeType; //开镜后的级别（0无，1不改变准星，2改变准星近，3改变准星远）
    public bool canContinuous;
    public bool haveCrosshair;

    [SerializeField]
    private Gun defaultGun;

    [SerializeField]
    private GameObject gunPoint;
    private GameObject gunGo;

    private bool isReloading = false;

    private Text ammoText;
    private GameObject reloadGo;
    private Slider reloadSlider;


    private float nextShootTime = 0;


    private float range = 500;

    [SerializeField]
    private Transform cameraTransform;

    private RaycastHit hit;

    private bool isEnlarge = false;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip enlargeClip;
    [SerializeField]
    private AudioClip warningClip;

    private AudioSync audioSync;

    [SerializeField]
    private GameObject crosshairGo;
    [SerializeField]
    private GameObject enlargeGo;

    private FirstPersonController firstPersonController;
    private Camera firstPersonCamera;



    [SerializeField]
    private GameObject bloodFX;

    private Player_Score scoreScript;

    [SerializeField]
    private GameObject shootLine;
    [SerializeField]
    private GameObject shootLight;
    //[SerializeField]
    //private Transform gunPointTransform;



    void Start()
    {
        if(isLocalPlayer)
        {
            ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
            crosshairGo = GameObject.Find("Crosshair Image");
            enlargeGo = GameObject.Find("EnlargePanel");
            enlargeGo.SetActive(false);
            reloadGo = GameObject.Find("ReloadSlider");
            reloadGo.SetActive(false);
            reloadSlider = reloadGo.GetComponent<Slider>();
            ChangeGun(defaultGun);
        }
    }

    public override void PreStartClient()
    {
        audioSource = GetComponent<AudioSource>();
        audioSync = GetComponent<AudioSync>();
        firstPersonController = GetComponent<FirstPersonController>();
        firstPersonCamera = cameraTransform.GetComponent<Camera>();
        scoreScript = GetComponent<Player_Score>();
    }


    void Update()
    {
        CheckIfShooting();
    }

    void CheckIfShooting()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(canEnlarge && Input.GetKeyDown(KeyCode.Mouse1))
        {
            audioSource.PlayOneShot(enlargeClip);
            ChangeEnlarge();
        }

        if(!isReloading)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
            if(canContinuous)
            {
                if(Input.GetKey(KeyCode.Mouse0))
                {
                    ShootBefore();
                }
            }
            else
            {
                if(Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ShootBefore();
                }
            }
        }
        else
        {
            nowReloadTime += Time.deltaTime;
            reloadSlider.value = nowReloadTime;
            if(nowReloadTime >= reloadTime)
            {
                isReloading = false;
                reloadGo.SetActive(false);
                nowAmmo = maxAmmo;
                ShowAmmoText();
            }
        }
    }

    void ShootBefore()
    {
        if(nowAmmo > 0)
        {
            if(Time.time >= nextShootTime)
            {
                if(damage == 0 || gunName == "")
                {
                    audioSource.PlayOneShot(warningClip);
                }
                else
                {
                    nextShootTime = Time.time + shootSpeed;
                    Shoot();
                }
            }
        }
        else
        {
            Reload();
        }
    }

    void Shoot()
    {
        nowAmmo--;
        if(nowAmmo <= 0)
        {
            Reload();
        }
        ShowAmmoText();
        audioSync.PlaySound(audioClipIndex);
        if(Physics.Raycast(cameraTransform.TransformPoint(0, 0, 0.5f), cameraTransform.forward, out hit, range))
        {
            if(hit.transform.tag == "Player")
            {
                scoreScript.score += 30;
                CreateBlood();
                string uIdentity = hit.transform.name;
                CmdTellServerWhoWasShoot(uIdentity, damage);
            }
            else if(hit.transform.tag == "Zombie")
            {
                scoreScript.score += 10;
                CreateBlood();
                string uIdentity = hit.transform.name;
                CmdTellServerWhichZombieWasShoot(uIdentity, damage);
            }
            //CreateShootLine(gunPointTransform.position, Quaternion.LookRotation(hit.point - gunPointTransform.position));
        }
    }

    void Reload()
    {
        reloadGo.SetActive(true);
        reloadSlider.maxValue = reloadTime;
        isReloading = true;
        nowReloadTime = 0;
    }
    

    void ShowAmmoText()
    {
        ammoText.text = nowAmmo + "";
    }

    void ChangeEnlarge()
    {
        //如果瞄准镜没有开启
        if(!isEnlarge)
        {
            isEnlarge = true;
            if(enlargeType == 1)
            {
                firstPersonCamera.fieldOfView = 42;
                firstPersonController.m_MouseLook.XSensitivity = 0.9f;
                firstPersonController.m_MouseLook.YSensitivity = 0.9f;
            }
            else if(enlargeType == 2)
            {
                crosshairGo.SetActive(false);
                enlargeGo.SetActive(true);
                firstPersonCamera.fieldOfView = 12;
                firstPersonController.m_MouseLook.XSensitivity = 0.2f;
                firstPersonController.m_MouseLook.YSensitivity = 0.2f;
                firstPersonController.m_MouseLook.smooth = true;
            }
            else if(enlargeType == 3)
            {
                crosshairGo.SetActive(false);
                enlargeGo.SetActive(true);
                firstPersonCamera.fieldOfView = 7;
                firstPersonController.m_MouseLook.XSensitivity = 0.15f;
                firstPersonController.m_MouseLook.YSensitivity = 0.15f;
                firstPersonController.m_MouseLook.smooth = true;
            }
        }
        else
        {
            ResetEnalrge();
        }
    }

    public void ChangeGun(Gun gun)
    {
        if(!isReloading)
        {
            Debug.Log(gun.gunName);
            gunName = gun.gunName;
            damage = gun.damage;
            shootSpeed = gun.shootSpeed;
            maxAmmo = gun.maxAmmo;
            nowAmmo = maxAmmo;
            reloadTime = gun.reloadTime;
            canEnlarge = gun.canEnlarge;
            enlargeType = gun.enlargeType;
            canContinuous = gun.canContinuous;
            haveCrosshair = gun.haveCrosshair;
            if(gunGo != null)
            {
                Destroy(gunGo);
            }

            gunGo = Instantiate(gun.modal, gunPoint.transform.position, gunPoint.transform.rotation) as GameObject;
            gunGo.transform.parent = gunPoint.transform;
            gunGo.transform.localPosition = Vector3.zero;
            gunGo.transform.localRotation = Quaternion.identity;

            if(haveCrosshair)
            {
                crosshairGo.SetActive(true);
            }
            else
            {
                crosshairGo.SetActive(false);
            }
            ResetEnalrge();
            ShowAmmoText();
        }
    }

    void ResetEnalrge()
    {
        isEnlarge = false;
        if(haveCrosshair)
        {
            crosshairGo.SetActive(true);
        }
        enlargeGo.SetActive(false);
        firstPersonCamera.fieldOfView = 60;
        firstPersonController.m_MouseLook.XSensitivity = 1f;
        firstPersonController.m_MouseLook.YSensitivity = 1f;
        firstPersonController.m_MouseLook.smooth = false;
    }

    [Command]
    void CmdTellServerWhoWasShoot(string uniqueID, int dmg)
    {
        GameObject go = GameObject.Find(uniqueID);
        go.GetComponent<Player_Health>().DeductHealth(dmg);
    }

    [Command]
    void CmdTellServerWhichZombieWasShoot(string uniqueID, int dmg)
    {
        GameObject go = GameObject.Find(uniqueID);
        go.GetComponent<Zombie_Health>().DeductHealth(dmg);
    }

    void CreateBlood()
    {
        CmdTellServerSpawnBlood(hit.point - cameraTransform.forward);
    }

    [Command]
    void CmdTellServerSpawnBlood(Vector3 pos)
    {
        RpcSpawnBlood(pos);
    }

    [ClientRpc]
    void RpcSpawnBlood(Vector3 pos)
    {
        GameObject.Instantiate(bloodFX, pos, Quaternion.identity);
    }

    void CreateShootLine(Vector3 pos, Quaternion dire)
    {
        //CmdTellServerCreateShootLine(pos, dire);
    }

    [Command]
    void CmdTellServerCreateShootLine(Vector3 pos, Quaternion dire)
    {
        RpcTellServerCreateShootLine(pos, dire);
    }

    [ClientRpc]
    void RpcTellServerCreateShootLine(Vector3 pos, Quaternion dire)
    {
        GameObject.Instantiate(shootLine, pos, dire);
        GameObject.Instantiate(shootLight, pos, Quaternion.identity);
    }
}
