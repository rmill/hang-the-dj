using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToStart : MonoBehaviour
{
  public string scene;

  void Update(){
    if (Input.GetMouseButtonDown(0)){
      SceneManager.LoadScene(scene);
    }
  }
}
