using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public string playGameLevel;
    public Button btLamAnh;
    public Button btTinh;
    public Button btAnhVu;
    [Header("Checkmarks")]
    public GameObject checkLamAnh;
    public GameObject checkTinh;
    public GameObject checkAnhVu;
    int selectedCharacter = -1;
    void Start()
    {
        // Gán sự kiện click
        btLamAnh.onClick.AddListener(() => SelectCharacter(0));
        btTinh.onClick.AddListener(() => SelectCharacter(1));
        btAnhVu.onClick.AddListener(() => SelectCharacter(2));

        // Tắt hết checkmark lúc đầu
        HideAllCheckmarks();
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
        PlayerPrefs.Save();
        Application.LoadLevel(playGameLevel);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    void SelectCharacter(int index)
    {
        selectedCharacter = index;

        HideAllCheckmarks();

        switch (index)
        {
            case 0:
                checkLamAnh.SetActive(true);
                break;
            case 1:
                checkTinh.SetActive(true);
                break;
            case 2:
                checkAnhVu.SetActive(true);
                break;
        }

        Debug.Log("Selected character: " + selectedCharacter);
    }

    void HideAllCheckmarks()
    {
        checkLamAnh.SetActive(false);
        checkTinh.SetActive(false);
        checkAnhVu.SetActive(false);
    }
}
