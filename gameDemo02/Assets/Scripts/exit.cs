using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class exit : MonoBehaviour
{
    public void Quit()
    {
  //       #if UNITY_EDITOR
		// UnityEditor.EditorApplication.isPlaying = false;
		// #endif
        Application.Quit();
    }

}
