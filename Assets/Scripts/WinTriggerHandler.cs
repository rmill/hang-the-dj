using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTriggerHandler : MonoBehaviour
{
    public string scene;

    void OnTriggerEnter2D(Collider2D winTrigger)
    {
        SceneManager.LoadScene(scene);
    }
}
