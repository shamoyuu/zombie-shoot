using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.EventSystems;

public class AdminManager : NetworkBehaviour
{
    [SerializeField]
    private InputField input;
    [SerializeField]
    private GameObject inputGO;

    public string playerIdentity = "";

    private bool isOpen = false;

    void Start()
    {
        input = inputGO.GetComponent<InputField>();
    }
    
    void Update()
    {
        if(!isOpen && Input.GetKeyDown(KeyCode.Minus))
        {
            isOpen = true;
            inputGO.SetActive(true);
            input.ActivateInputField();
            input.text = "";
            GameObject.Find(playerIdentity).GetComponent<FirstPersonController>().enabled = false;
        }
    }

    public void AminEnter()
    {
        isOpen = false;
        inputGO.SetActive(false);
        GameObject.Find(playerIdentity).GetComponent<FirstPersonController>().enabled = true;
        string order = input.text;
        Debug.Log(order);
        if(order.StartsWith("-"))
        {
            order = order.Remove(0, 1).ToLower();
            Debug.Log(order);
            switch(order)
            {
                case "home":
                    GameObject.Find(playerIdentity).transform.position = new Vector3(49.54f, 25.1f, -49.75f);
                    break;
                case "top":
                    GameObject.Find(playerIdentity).transform.position = new Vector3(-1.48f, 27f, 2.12f);
                    break;
                case "mid":
                    GameObject.Find(playerIdentity).transform.position = new Vector3(-1.48f, 14f, 2.12f);
                    break;
            }
        }
    }
}
