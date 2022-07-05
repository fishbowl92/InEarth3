using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using TMPro;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public GameManager GM;
    private void Awake()
    {
        Screen.SetResolution(1600, 720, true);
        eqi = new MapManager.equipInfo();
    }

    public Transform settingPenal;
    public void settingsettingPenal()
    {
        settingPenal.GetChild(0).GetChild(2).GetComponent<Slider>().value = PlayerPrefs.GetFloat("BGMvalue");
        settingPenal.GetChild(1).GetChild(2).GetComponent<Slider>().value = PlayerPrefs.GetFloat("SFXvalue");
        settingPenal.GetChild(2).GetChild(2).GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("SettingData") == 0;
    }
    public void saveSettingData()
    {
        PlayerPrefs.SetFloat("BGMvalue", settingPenal.GetChild(0).GetChild(2).GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("SFXvalue", settingPenal.GetChild(1).GetChild(2).GetComponent<Slider>().value);
        PlayerPrefs.SetInt("SettingData", settingPenal.GetChild(2).GetChild(2).GetComponent<Toggle>().isOn ? 0 : 1);
    }

    public Transform showPopupUI;
    public void PopupUISetting(int a)
    {
        showPopupUI.gameObject.SetActive(a > 0);
        if (a > 0)
        {
            GM.btnSound[0].Play();
            for (int i = 1; i < showPopupUI.childCount; ++i)
            {
                showPopupUI.GetChild(i).gameObject.SetActive(i == a);
            }
        }
        else
        {
            GM.btnSound[1].Play();
        }
    }

    public TextMeshProUGUI helpInfoBox;
    public GameObject prebSeletHelpBox;
    public Transform helpBoxContens;
    public void settingHelpBox()
    {
        Transform tTemp = helpBoxContens.GetChild(0);
        for (int i = 0; i < GM.BuffImg.Length; ++i)
        {
            string sTemp = GM.returnAddBuffEffectInfo(i);
            if (sTemp == "") break;
            if (i != 0)
            {
                tTemp = Instantiate(tTemp.gameObject, helpBoxContens).transform;
            }
            tTemp.name = "help";
            tTemp.GetChild(0).GetComponent<Image>().sprite = GM.BuffImg[i];
            Vector3 vTemp = tTemp.position;
            vTemp.z = i;
            tTemp.position = vTemp;
        }
    }
    public void checkHelpBoxInfo(Transform t)
    {
        int num = (int)(t.position.z + 0.3f);
        helpInfoBox.text = GM.returnAddBuffEffectInfo(num);
        if (prebSeletHelpBox != null)
        {
            prebSeletHelpBox.SetActive(false);
        }
        prebSeletHelpBox = t.GetChild(1).gameObject;
        prebSeletHelpBox.SetActive(true);
    }
    public TextMeshProUGUI helpInfoBox2;
    public GameObject prebSeletHelpBox2;
    MapManager.equipInfo eqi;
    public Transform helpBoxContens2;
    public void settingHelpBox2()
    {
        Transform tTemp = helpBoxContens2.GetChild(0);
        for (int i = 0; i < GM.StartBuffImg.Length; ++i)
        {
            string sTemp = GM.returnAddBuffEffectInfo(i);
            if (sTemp == "") break;
            if (i != 0)
            {
                tTemp = Instantiate(tTemp.gameObject, helpBoxContens2).transform;
            }
            tTemp.name = "help";
            tTemp.GetChild(0).GetComponent<Image>().sprite = GM.StartBuffImg[i];
            Vector3 vTemp = tTemp.position;
            vTemp.z = i;
            tTemp.position = vTemp;
        }
    }
    public void checkHelpBoxInfo2(Transform t)
    {
        int num = (int)(t.position.z + 0.3f);
        eqi.buffSort = (MapManager.equipState)num;
        helpInfoBox2.text = eqi.getName() + " : " + eqi.getInfo();
        if (prebSeletHelpBox2 != null)
        {
            prebSeletHelpBox2.SetActive(false);
        }
        prebSeletHelpBox2 = t.GetChild(1).gameObject;
        prebSeletHelpBox2.SetActive(true);
    }

    void Start()
    {
        GM = GameManager.Instans;
        GM.getOreCount();

        for (int i = 0; i < 5; ++i)
        {
            oreCount[i].text = GM.myOres[i].ToString();
        }
        if(PlayerPrefs.GetInt("PlayerDeckElement") != -1 && PlayerPrefs.HasKey("PlayerDeckElement"))
        {
            uitransform.gameObject.SetActive(true);
        }
        else
        {
            uitransform.gameObject.SetActive(false);
        }
        settingsettingPenal();
        settingHelpBox();
        settingHelpBox2();
    }
    public void startGame()
    {
        LoadingSceneManager.LoadScene("Play");
    }
    public void continueGame(bool isContinue)
    {
        if (isContinue)
        {
            LoadingSceneManager.LoadScene("Play");
        }
        else
        {
            PlayerPrefs.SetInt("PlayerDeckElement", -1);
            for (int i = 0; i < 4; ++i)
            {
                GM.golemPart[i] = Vector2Int.zero;
            }
            uitransform.gameObject.SetActive(false);
        }
    }
    public Transform uitransform;
    public void UIOff()
    {
        uitransform.gameObject.SetActive(false);
    }
    public int selectPart;
    public Transform golemTransform;
    public Image golemPartImage;
    public Sprite[] golemPartSprites;
    public TextMeshProUGUI[] oreCount;
    public void changeSelectPart(bool up)
    {
        selectPart = (up) ? selectPart + 1 : selectPart - 1;
        selectPart = (selectPart + 4) % 4;
        golemPartImage.sprite = golemPartSprites[selectPart];
    }
    public void upgreadGolemPart(int a)
    {
        if(GM.myOres[a] > 0)
        {
            GM.myOres[a]--;
            GM.golemPart[selectPart].x = a;
            GM.golemPart[selectPart].y = (Random.Range(0, 2) == 0) ? 1 : a + 2;
            GM.golemImgSetting(golemTransform);
            oreCount[a].text = GM.myOres[a].ToString();
            checkStartBuff();
        }
    }
    public Transform startBuffSettingPenal;
    public void checkStartBuff()
    {
        for(int i = 0; i < 4; ++i)
        {
            Transform tTemp = startBuffSettingPenal.GetChild(i);
            int num = GM.golemPart[i].y;
            if (num < 1)
            {
                tTemp.gameObject.SetActive(false);
            }
            else
            {
                tTemp.gameObject.SetActive(true);
                tTemp.GetComponent<Image>().sprite = GM.StartBuffImg[num - 1];
            }
        }
    }


    public SpriteResolver[] head;
    public string[] elementSort;
}
