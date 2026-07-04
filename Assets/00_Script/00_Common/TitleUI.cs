using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TitleUI : MonoBehaviour
{
    void Awake()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => SceneLoader.Instance.LoadScene("InGame"));
        buttons[1].onClick.AddListener(() => SceneLoader.Instance.LoadScene("Tutorial"));
        buttons[3].onClick.AddListener(() => SceneLoader.Instance.QuitGame());
    }
}
