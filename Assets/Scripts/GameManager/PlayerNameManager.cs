using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField]
    private InputField nameInput;
    public string currentName;

    private string[] names = {
        "天振",
        "琛祥",
        "成运",
        "星家",
        "振炳",
        "骏弘",
        "良震",
        "龙铭",
        "晓逸",
        "运振",
        "栋诚",
        "畅鸿",
        "濡骏"
    };
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        currentName = names[Random.Range(0, names.Length)];
        nameInput.text = currentName;
    }
}
