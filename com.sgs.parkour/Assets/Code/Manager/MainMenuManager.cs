using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button playBtn;
    [SerializeField] Button onlineBtn;
    [SerializeField] Button exitBtn;

    void Awake()
    {
        playBtn.onClick.AddListener(CharacterSelection);
        exitBtn.onClick.AddListener(OnExit);
    }

    private void CharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

    private void OnExit()
    {
        Application.Quit();
    }


}
