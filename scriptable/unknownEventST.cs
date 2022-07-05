using UnityEngine.Events;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "eventblock", menuName = "Scriptable Object/event")]
public class unknownEventST : ScriptableObject
{
    // Start is called before the first frame update
    public Sprite sprite;
    [TextArea(5, 20)]
    public string situation; //상황 텍스트
    public string selectA; //선택지 텍스트
    public string selectB;
    public string selectC;
    public int[] selectValA; //선택지 별 적용될 값 
    public int[] selectValB; //선택지 별 적용될 값
    public UnityEvent<int,int>[] selectAResult; //선택지 이벤트
    public UnityEvent<int,int>[] selectBResult;
    public unknownEventST[] nextEvent; //선택지 별 다음 이벤트
    [TextArea(5, 20)]
    public string[] resultString; // 선택한 이벤트 결과 텍스트
    public Sprite[] resultSprite;

    public void playerHpChange(int n,int m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getPlayerAddHpText.text = (m>0)?"+":"" + m;
        Mapm.getPlayerAddHpAnim.SetTrigger("start");
        Mapm.player.hpAdd(m);
    }
    public void golemHpChange(int n, int m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getGolemAddHpText.text = (m > 0) ? "+" : "" + m;
        Mapm.getGolemAddHpAnim.SetTrigger("start");
        Mapm.player.golemHpAdd(m);
    }
    public void golemFixHp(int n, int m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.player.golemMaxHp = m;
        Mapm.showGolemHpUI();
    }
    public void exchangeCard(int n, int m)
    {
        MapManager Mapm = MapManager.instance;
        if (Mapm.oreCount > 9)
        {
            Mapm.oreCount -= 10;
            Mapm.showOreUIText();
            getSpecificTaro(n, m);
            eventEndGetTaro(n, m);
        }
    }
    public void exchangeCardByGolemHp(int n, int m)
    {
        MapManager Mapm = MapManager.instance;
        if (Mapm.player.golemHp > 20)
        {
            Mapm.player.golemHp -= 20;
            Mapm.showGolemHpUI();
            getSpecificTaro(n, m);
            eventEndGetTaro(n, m);
        }
    }
    public void exchangeHeal(int n, int m)
    {
        MapManager Mapm = MapManager.instance;
        if (Mapm.oreCount >= m)
        {
            Mapm.oreCount -= m;
            Mapm.showOreUIText();
            Mapm.player.hpAdd(m);
            eventEnd(n, m);
        }

     }
    public void getRandomTaro(int n, int m)
    {
        GameManager GM = GameManager.Instans;
        int ran = Random.Range(0, GM.StartBuffImg.Length);
        MapManager.instance.getEquipState(ran);
        eventEndGetTaro(n, ran);
    }
    public void ifManyOreHalfPercentGetSpecificTaro(int n,int m)
    {
        MapManager Mapm = MapManager.instance;
        if (Mapm.oreCount > 29)
        {
            Mapm.oreCount -= 30;
            Mapm.showOreUIText();
            if (Random.Range(0, 2) == 0)
            {
                getSpecificTaro(n, m);
                eventEndGetTaro(0, m);
            }
            else eventEnd(1, m);
        }
    }
    public void getOre(int n, int m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.oreCount += m;
        Mapm.getOreAnim.SetTrigger("start");
    }
    public void getSpecificTaro(int n,int m)
    {
        MapManager.instance.getEquipState(m);
        //eventEndGetTaro(n, m);
    }
    public void getRandCard(int n, int m)
    {
        SkillBlock sb;
        GameManager GM = GameManager.Instans;
        switch (m)
        {
            case 0: //fire
                sb = Resources.LoadAll<SkillBlock>("Skill/allSkill/R_Fire")[Random.Range(0,49)];
                break;
            case 1: //wind
                sb = Resources.LoadAll<SkillBlock>("Skill/allSkill/R_wind")[Random.Range(0, 47)];
                break;
            case 2: //earth
                sb = Resources.LoadAll<SkillBlock>("Skill/allSkill/R_earth")[Random.Range(0, 42)];
                break;
            default: //water
                sb = Resources.LoadAll<SkillBlock>("Skill/allSkill/R_water")[Random.Range(0, 47)];
                break;
        }
        MapManager Mapm = MapManager.instance;
        MapManager.DeckData newCard = new MapManager.DeckData(sb, 0);
        GM.customedDeck.Add(newCard);
        Mapm.addCardListUI(newCard);
        //eventEndGetCard(n, sb);
    }
    public void eventEndGetCard(int a, SkillBlock sb)
    {
        MapManager Mapm = MapManager.instance;
        GameManager GM = GameManager.Instans;
        if (nextEvent[a] == null)
        {
            Transform temp = Mapm.eventUI.GetChild(2);
            temp.GetChild(0).gameObject.SetActive(false);
            temp.GetChild(1).gameObject.SetActive(false);
            temp.GetChild(2).gameObject.SetActive(true);
            Mapm.eventUI.GetChild(0).GetComponent<TextMeshProUGUI>().text = resultString[a];
            if (resultSprite != null) Mapm.eventUI.GetChild(1).GetComponent<Image>().sprite = resultSprite[a];
            Mapm.eventUI.GetChild(0).GetComponent<TextMeshProUGUI>().text += "\n\n" + sb.skillName + "\n" + sb.info;
            temp.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "확인";
        }
        else
        {
            Mapm.eventsSetting(nextEvent[a]);
        }
    }
    public void eventEndGetTaro(int a, int taroCardNum)
    {
        MapManager Mapm = MapManager.instance;
        GameManager GM = GameManager.Instans;
        if (nextEvent[a] == null) {
            Transform temp = Mapm.eventUI.GetChild(2);
            temp.GetChild(0).gameObject.SetActive(false);
            temp.GetChild(1).gameObject.SetActive(false);
            temp.GetChild(2).gameObject.SetActive(true);
            Mapm.eventUI.GetChild(0).GetComponent<TextMeshProUGUI>().text = resultString[a];
            if (taroCardNum > -1)
            {
                Mapm.eventUI.GetChild(1).GetComponent<Image>().sprite = GM.StartBuffImg[taroCardNum];
                Mapm.eventUI.GetChild(1).GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 250);
                string infoText, name;
                switch (taroCardNum)
                {
                    case 0:
                        name = "근원";
                        infoText = "시작시 카드를 뽑습니다.";
                        break;
                    case 1:
                        name = "마나";
                        infoText = "시작시 마나를 획득합니다.";
                        break;
                    case 2:
                        name = "완드";
                        infoText = "시작시 골램이 체력을 얻습니다.";
                        break;
                    case 3:
                        name = "소드";
                        infoText = "시작시 적이 더 오래 딜레이를 가집니다.";
                        break;
                    case 4:
                        name = "펜타클";
                        infoText = "시작시 광물을 얻습니다.";
                        break;
                    case 5:
                        name = "컵";
                        infoText = "시작시 의지를 얻습니다.";
                        break;
                    case 6:
                        name = "바보";
                        infoText = "시작시 순환을 얻습니다.";
                        break;
                    case 7:
                        name = "마법사";
                        infoText = "시작시 민첩을 얻습니다.";
                        break;
                    case 8:
                        name = "연인";
                        infoText = "시작시 지능을 얻습니다.";
                        break;
                    case 9:
                        name = "세계";
                        infoText = "시작시 힘, 민첩, 지능을 얻습니다.";
                        break;
                    default:
                        name = "";
                        infoText = "";
                        break;
                }
                Mapm.eventUI.GetChild(0).GetComponent<TextMeshProUGUI>().text += "\n\n"+ name + "\n" + infoText;
            }
           
            temp.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "확인";
        }
        else
        {
            Mapm.eventsSetting(nextEvent[a]);
        }
    }
    public void eventEnd(int a,int b)
    {
        MapManager Mapm = MapManager.instance;
        if (nextEvent[a] == null)
        {
            Transform temp = Mapm.eventUI.GetChild(2);
            temp.GetChild(0).gameObject.SetActive(false);
            temp.GetChild(1).gameObject.SetActive(false);
            temp.GetChild(2).gameObject.SetActive(true);
            Mapm.eventUI.GetChild(0).GetComponent<TextMeshProUGUI>().text = resultString[a];
            if (resultSprite != null) Mapm.eventUI.GetChild(1).GetComponent<Image>().sprite = resultSprite[a];
            temp.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "확인";
        }
        else
        {
            Mapm.eventsSetting(nextEvent[a]);
        }
    }
    public void eventEndAndFightMon(int a, int b)
    {
        MapManager Mapm = MapManager.instance;
        GameManager GM = GameManager.Instans;
        Mapm.PopupUISetting(0);
        Time.timeScale = 1;
        Mapm.Curtain.gameObject.SetActive(false);
        Mapm.monster = GM.normalMonsters[Random.Range(0, GM.normalMonsters.Length)];
        Mapm.sm.MonsterInit();
        Mapm.newSkillTierPercent[0] = Mapm.skillTierPercentCluster[0, 0];
        Mapm.newSkillTierPercent[1] = Mapm.skillTierPercentCluster[0, 1];
        Mapm.newSkillTierPercent[2] = Mapm.skillTierPercentCluster[0, 2];
    }
}
