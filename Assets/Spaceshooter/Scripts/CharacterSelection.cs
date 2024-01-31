using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour {

    [SerializeField]
    PlayerStats playerStats;

    GameObject[] characters;
    int index;

    void Start() {
        index = PlayerPrefs.GetInt("SelectedCharacter");
        setChoosenShip();
        characters = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            characters[i] = transform.GetChild(i).gameObject;
            characters[i].SetActive(false);
        }
        if (characters[index]) {
            characters[index].SetActive(true);
        }
    }
    void setChoosenShip()
    {
        switch (index)
        {
            case 0:
                PlayerPrefs.SetString("choosenShip", "default");
                break;
            case 1:
                PlayerPrefs.SetString("choosenShip", "UpgradedShip");
                break;
            case 2:
                PlayerPrefs.SetString("choosenShip", "BlueGemSkin");
                break;
            case 3:
                PlayerPrefs.SetString("choosenShip", "SapphireSkin");
                break;
        }
        Debug.Log(PlayerPrefs.GetString("choosenShip"));
    }
    public void LogOut()
    {
        SceneManager.LoadScene("Login");
        PlayFabClientAPI.ForgetAllCredentials();
    }
    public void toggleLeft() {

        characters[index].SetActive(false);
        if (index == 0) {
            index = transform.childCount - 1;
        } else {
            index--;
        }
        setChoosenShip();
        PlayerPrefs.SetInt("SelectedCharacter", index);
        characters[index].SetActive(true);
    }

    public void toggleRight() {

        characters[index].SetActive(false);
        if(index == transform.childCount-1){
            index = 0;
        }
        else{
            index++;
        }
        setChoosenShip();
        PlayerPrefs.SetInt("SelectedCharacter", index);
        characters[index].SetActive(true);
    }

    public void selectCharacterAndStart(){
        if(PlayerPrefs.GetInt("shipExist") == 1)
        {
            PlayerPrefs.SetInt("SelectedCharacter", index);
            SceneManager.LoadScene("Game");
        }

    }
    public void RunSocialTab()
    {
        SceneManager.LoadScene("SocialTab");

    }
    public void RunClanTab()
    {
        SceneManager.LoadScene("ClanTab");

    }
    public void RunTradeTab()
    {
        SceneManager.LoadScene("Trading Hub");

    }
    public int getIndex(){
        return index;
    }

}
