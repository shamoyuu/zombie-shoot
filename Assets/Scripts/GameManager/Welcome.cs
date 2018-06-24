using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Welcome : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(GoToNextLevel());
    }

    IEnumerator GoToNextLevel()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }
}
