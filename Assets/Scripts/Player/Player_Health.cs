using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Player_Health : NetworkBehaviour
{
    [SyncVar(hook = "OnHealthChanged")]
    public int health = 100;

    private Text healthText;
    private Image healthImage;
    private Image crossHairImage;
    private bool shouldDie = false;
    public bool isDead = false;
    private GameObject respawnButton;
    private GameObject[] playerStartPoint;
    
    private Image hurtRedImage;

    private float currentAlpha;
    
    private FirstPersonController firstPersonController;

    private Player_FallHurt fallHurtScript;

    public override void OnStartLocalPlayer()
    {
        respawnButton = GameObject.Find("Respawn Button");
        respawnButton.GetComponent<Button>().onClick.AddListener(CommenceRespawn);
        respawnButton.SetActive(false);

        healthText = GameObject.Find("Health Text").GetComponent<Text>();
        healthImage = GameObject.Find("Health Image").GetComponent<Image>();
        SetHealthText();

        crossHairImage = GameObject.Find("Crosshair Image").GetComponent<Image>();

        playerStartPoint = GameObject.FindGameObjectsWithTag("PlayerStartPoint");
        hurtRedImage = GameObject.Find("HurtRed Image").GetComponent<Image>();

        firstPersonController = GetComponent<FirstPersonController>();
        fallHurtScript = GetComponent<Player_FallHurt>();
    }

    void Update()
    {
        CheckCondition();
    }

    [Client]
    void SetHealthText()
    {
        if(isLocalPlayer)
        {
            healthText.text = "生命值：" + health;
            healthImage.transform.localScale = new Vector3(health / 100.0f, 0.1f, 1);
        }
    }

    [Server]
    public void DeductHealth(int dmg)
    {
        health -= dmg;
        if(health < 0)
            health = 0;

        SetHealthText();
    }

    void CheckCondition()
    {
        if(health <= 0 && !shouldDie && !isDead)
        {
            shouldDie = true;
        }

        if(health <= 0 && shouldDie)
        {
            DisablePlayer();
            shouldDie = false;
        }

        if(health > 0 && isDead)
        {
            EnablePlayer();
            isDead = false;
        }
    }

    void OnHealthChanged(int hlth)
    {
        health = hlth;
        SetHealthText();
        if(isLocalPlayer)
        {
            StartCoroutine(HurtRed());
        }
    }

    IEnumerator HurtRed()
    {
        currentAlpha = 1;
        hurtRedImage.enabled = true;
        for(;;)
        {
            hurtRedImage.color = new Color(1, 0, 0, currentAlpha);
            currentAlpha -= 0.1f;
            if(currentAlpha <= 0)
            {
                currentAlpha = 0;
                hurtRedImage.enabled = false;
                StopCoroutine(HurtRed());
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ResetHealth()
    {
        health = 100;
        SetHealthText();
    }

    void DisablePlayer()
    {
        if(isLocalPlayer)
        {
            crossHairImage.enabled = false;
            firstPersonController.m_MouseLook.lockCursor = false;
            Cursor.visible = true;
            respawnButton.SetActive(true);
            GetComponent<WeaponManager>().enabled = false;
            GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
        }

        GetComponent<CharacterController>().enabled = false;
        GetComponent<Player_Shoot>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer ren in renderers)
        {
            ren.enabled = false;
        }

        isDead = true;
    }

    void EnablePlayer()
    {
        if(isLocalPlayer)
        {
            crossHairImage.enabled = true;
            firstPersonController.m_MouseLook.lockCursor = true;
            respawnButton.SetActive(false);
            GetComponent<WeaponManager>().enabled = true;
            GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            transform.position = playerStartPoint[Random.Range(0, playerStartPoint.Length)].transform.position;
            fallHurtScript.isTakeOff = false;
        }
        GetComponent<CharacterController>().enabled = true;
        GetComponent<Player_Shoot>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer ren in renderers)
        {
            ren.enabled = true;
        }
    }
    void CommenceRespawn()
    {
        CmdRespawnOnServer();
    }

    [Command]
    void CmdRespawnOnServer()
    {
        ResetHealth();
    }
}
