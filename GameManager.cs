using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using conv = System.Convert;

public class GameManager : MonoBehaviour
{
    public static GameManager Instans;
    public AudioSource[] btnSound;
    public SkillBlock[] startSkillArray;
    public List<SkillBlock> allSkillArrayNormal;
    public List<SkillBlock> allSkillArrayRare;
    public List<SkillBlock> allSkillArrayHidden;

    public List<MapManager.DeckData> customedDeck = new List<MapManager.DeckData>();

    public List<Sprite> dmgFontGreen;
    public List<Sprite> dmgFontRed;

    public Monster[] normalMonsters;
    public Monster[] strongMonsters;
    public Monster[] leaderMonsters;
    public Monster lastBoss;
    void Awake()
    {
        if (Instans)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            Instans = this;
            if (!PlayerPrefs.HasKey("BGMvalue"))
            {
                PlayerPrefs.SetFloat("BGMvalue", 0.7f);
                PlayerPrefs.SetFloat("SFXvalue", 0.7f);
            }
        }

    }

    public void getOreCount()
    {
        myOres = new int[5];
        if (PlayerPrefs.HasKey("PlayerOreCount"))
        {
            string[] sData = PlayerPrefs.GetString("PlayerOreCount").Split('#');
            if (sData.Length < 5) return;
            for (int i = 0; i < 5; ++i)
            {
                myOres[i] = conv.ToInt32(sData[i]);
            }
        }
    }
    public void saveOreCount()
    {
        string sTemp = myOres[0].ToString();
        for (int i = 1; i < 5; ++i)
        {
            sTemp += "#" + myOres[i];
        }
        PlayerPrefs.SetString("PlayerOreCount", sTemp);
    }
    public void loadGameProgres()
    {
        MapManager mm = MapManager.instance;

        settingAllSkillSet(PlayerPrefs.GetInt("PlayerDeckElement"));
        mm.checkShildPersentSetting(PlayerPrefs.GetInt("PlayerDeckElement"));
        string[] sData;
        if (PlayerPrefs.HasKey("EquipData"))
        {
            sData = PlayerPrefs.GetString("EquipData").Split('#');
            mm.equipList = new List<MapManager.equipInfo>();
            for (int i = 0; i < sData.Length; ++i)
            {
                string[] sTemp = sData[i].Split(',');
                if (sTemp.Length < 2) continue;
                mm.setEquip((MapManager.equipState)conv.ToInt32(sTemp[0]), conv.ToInt32(sTemp[1]));
            }
        }
        sData = PlayerPrefs.GetString("DeckData").Split('#');
        customedDeck = new List<MapManager.DeckData>();
        SkillBlock sb;
        for (int i = 0; i < sData.Length; ++i)
        {
            string[] sTemp = sData[i].Split(',');
            if (sTemp.Length < 2) continue;
            sb = null;
            switch (conv.ToInt32(sTemp[0]))
            {
                case 0:
                    sb = allSkillArrayNormal.Find(item => item.name == sTemp[1]);
                    break;
                case 1:
                    sb = allSkillArrayRare.Find(item => item.name == sTemp[1]);
                    break;
                case 2:
                    sb = allSkillArrayHidden.Find(item => item.name == sTemp[1]);
                    break;
                default:
                    for (int l = 0; l < startSkillArray.Length; ++l)
                    {
                        if (sTemp[1] == startSkillArray[l].name)
                        {
                            sb = startSkillArray[l];
                            break;
                        }
                    }
                    break;
            }
            if (sb != null)
            {
                MapManager.DeckData newCard = new MapManager.DeckData(sb, 0);
                customedDeck.Add(newCard);
                if (i == 0)
                {
                    Transform tf = mm.firstCard.transform;
                    tf.Find("icon").GetComponent<Image>().sprite = newCard.sb.skillIcon;
                    tf.Find("cost").GetComponent<TextMeshProUGUI>().text = newCard.cost.ToString();
                    tf.Find("name").GetComponent<TextMeshProUGUI>().text = newCard.sb.skillName;
                    tf.Find("info").GetComponent<TextMeshProUGUI>().text = mm.getInfoText(newCard.sb.info, newCard.sb.skillVal[0].y, 0);
                }
                else
                {
                    mm.addCardListUI(newCard);
                }

            }
        }


        sData = PlayerPrefs.GetString("GolemData").Split('#');
        for (int i = 0; i < sData.Length; ++i)
        {
            string[] sTemp = sData[i].Split(',');
            if (sTemp.Length < 2) continue;
            golemPart[i].x = conv.ToInt32(sTemp[0]);
            golemPart[i].y = conv.ToInt32(sTemp[1]);
        }

        sData = PlayerPrefs.GetString("PlayerData").Split(',');
        mm.player.hp = conv.ToInt32(sData[0]);

        mm.player.golemHp = conv.ToInt32(sData[1]);
        mm.player.golemMaxHp = conv.ToInt32(sData[2]);
        mm.oreCount = conv.ToInt32(sData[3]);
        mm.floorLev = conv.ToInt32(sData[4]);
        mm.floorText.text = (mm.floorLev + 1).ToString();
        mm.floorImg.sprite = mm.floorSprites[mm.floorLev];
        mm.nowProcessNum = conv.ToInt32(sData[5]);

        mm.actionCountText.text = mm.nowProcessNum + "/" + mm.maxProcessNum;

        mm.monster.mTier = conv.ToInt32(sData[6]);
        mm.monster.name = sData[7];
        switch (mm.monster.mTier)
        {
            case 0:
                for (int i = 0; i < normalMonsters.Length; ++i)
                {
                    if (mm.monster.name == normalMonsters[i].name)
                    {
                        mm.monster = normalMonsters[i];
                    }
                }
                if (mm.floorLev > 0) mm.changeBGM(Mathf.Clamp(mm.floorLev - 1, 0, 2));
                break;
            case 1:
                for (int i = 0; i < strongMonsters.Length; ++i)
                {
                    if (mm.monster.name == strongMonsters[i].name)
                    {
                        mm.monster = strongMonsters[i];
                    }
                }
                if (mm.floorLev > 0) mm.changeBGM(Mathf.Clamp(mm.floorLev - 1, 0, 2));
                break;
            case 2:
                for (int i = 0; i < leaderMonsters.Length; ++i)
                {
                    if (mm.monster.name == leaderMonsters[i].name)
                    {
                        mm.monster = leaderMonsters[i];
                    }
                }
                mm.actionCountText.text = "대장"; 
                mm.changeBGM(Mathf.Clamp(mm.floorLev - 2, 0, 2));
                mm.sm.isBoss = true;
                break;
            default:
                mm.monster = lastBoss;
                mm.actionCountText.text = "광가의 골렘";
                mm.changeBGM(2);
                mm.sm.isBoss = true;
                break;
        }
        if (PlayerPrefs.GetInt("savePoint") == 1) mm.sm.MonsterInit();
        else mm.reset();

        sData = PlayerPrefs.GetString("NodeData").Split('#');
        int j = 0;
        for (int i = 0; i < mm.next2MapData.Length; ++i)
        {
            for (int l = 0; l < 4; ++l)
            {
                string[] sTemp = sData[j].Split(',');
                mm.next2MapData[i].myNode[l].x = conv.ToInt32(sTemp[0]);
                mm.next2MapData[i].myNode[l].y = conv.ToInt32(sTemp[1]);
                ++j;
            }
            mm.setMapNodeShow(i, mm.next2MapData[i]);
        }
        for (int i = 0; i < mm.next4MapData.Length; ++i)
        {
            for (int l = 0; l < 4; ++l)
            {
                string[] sTemp = sData[j].Split(',');
                mm.next4MapData[i].myNode[l].x = conv.ToInt32(sTemp[0]);
                mm.next4MapData[i].myNode[l].y = conv.ToInt32(sTemp[1]);
                ++j;
            }
            mm.setMapNodeShow(2 + i, mm.next4MapData[i]);
        }

        mm.bossRandList.Clear();

        sData = PlayerPrefs.GetString("BossClearData").Split(',');
        for (int i = 0; i < sData.Length; ++i)
        {
            if (sData[i] != "") mm.bossRandList.Add(conv.ToInt32(sData[i]));
        }

        mm.anCounter = PlayerPrefs.GetInt("anCounter");
        mm.youngCounter = PlayerPrefs.GetInt("YoungCounter");
    }
    public void saveGameProgress(int timeing)
    {
        switch (timeing)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                PlayerPrefs.SetInt("PlayerDeckElement", timeing);
                break;
            case 4:
                PlayerPrefs.SetInt("savePoint", 1); //몬스터와 조우
                break;
            case 5:
                PlayerPrefs.SetInt("savePoint", 2); //노드 선택지
                break;
            case 6:
                PlayerPrefs.SetInt("savePoint", 3); //미지 이벤트 시작
                break;

        }
        MapManager mm = MapManager.instance;
        //저장해야될것 2.사용속성
        //0.유물 1.카드 3.골램색상
        //3.유저체력 4.골램체력/최대체력 5.광물
        //6.진척도 수치 7. 조우한 적 8.노드정보 9.해치운보스정보
        string data = string.Empty;
        for (int i = 0; i < mm.equipList.Count; ++i)
        {
            data += conv.ToInt32(mm.equipList[i].buffSort) + "," + mm.equipList[i].count + "#";
        }
        if (data.Length > 0)
        {
            data.Substring(0, data.Length - 1);
            PlayerPrefs.SetString("EquipData", data);
        }
        else
        {
            PlayerPrefs.DeleteKey("EquipData");
        }

        data = string.Empty;
        for (int i = 0; i < customedDeck.Count; ++i)
        {
            data += customedDeck[i].sb.skillTier + "," + customedDeck[i].sb.name + "#";
        }
        data.Substring(0, data.Length - 1);
        PlayerPrefs.SetString("DeckData", data);

        data = string.Empty;
        for (int i = 0; i < golemPart.Length; ++i)
        {
            data += golemPart[i].x + "," + golemPart[i].y + "#";
        }
        data.Substring(0, data.Length - 1);
        PlayerPrefs.SetString("GolemData", data);

        PlayerPrefs.SetString("PlayerData", mm.player.hp + "," + mm.player.golemHp + "," + mm.player.golemMaxHp + "," +
            mm.oreCount + "," + mm.floorLev + "," + mm.nowProcessNum + "," + mm.monster.mTier + "," + mm.monster.name);

        data = string.Empty;
        for (int i = 0; i < mm.next2MapData.Length; ++i)
        {
            string sTemp = string.Empty;
            for (int l = 0; l < 4; ++l)
            {
                sTemp += mm.next2MapData[i].myNode[l].x + "," + mm.next2MapData[i].myNode[l].y + "#";
            }
            data += sTemp;
        }
        for (int i = 0; i < mm.next4MapData.Length; ++i)
        {
            string sTemp = string.Empty;
            for (int l = 0; l < 4; ++l)
            {
                sTemp += mm.next4MapData[i].myNode[l].x + "," + mm.next4MapData[i].myNode[l].y + "#";
            }
            data += sTemp;
        }
        PlayerPrefs.SetString("NodeData", data);

        data = string.Empty;
        for (int i = 0; i < mm.bossRandList.Count; ++i)
        {
            if (i != 0) data += ",";
            data += mm.bossRandList[i].ToString();
        }
        PlayerPrefs.SetString("BossClearData", data);


        PlayerPrefs.SetInt("anCounter", mm.anCounter);
        PlayerPrefs.SetInt("YoungCounter", mm.youngCounter);
    }
    [ContextMenu("스킬 셋팅")]
    public void settingAllSkillSet(int golemType)
    {
        // startSkillArray = Resources.LoadAll<SkillBlock>("Skill/allSkill/startSkill");
        allSkillArrayNormal = new List<SkillBlock>();
        allSkillArrayRare = new List<SkillBlock>();
        allSkillArrayHidden = new List<SkillBlock>();
        SkillBlock[] sd;
        switch (golemType)
        {
            case 0:
                sd = Resources.LoadAll<SkillBlock>("Skill/allSkill/R_Fire");
                break;
            case 1:
                sd = Resources.LoadAll<SkillBlock>("Skill/allSkill/R_wind");
                break;
            case 2:
                sd = Resources.LoadAll<SkillBlock>("Skill/allSkill/R_earth");
                break;
            default:
                sd = Resources.LoadAll<SkillBlock>("Skill/allSkill/R_water");
                break;
        }

        for (int i = 0; i < sd.Length; ++i)
        {
            if (sd[i].skillTier == 0)
            {
                allSkillArrayNormal.Add(sd[i]);
            }
            else if (sd[i].skillTier == 1)
            {
                allSkillArrayRare.Add(sd[i]);
            }
            else
            {
                allSkillArrayHidden.Add(sd[i]);
            }
        }
    }


    public int[] myOres;

    public string[] elementSort;

    public List<int> headSet;
    public List<int> leftArmSet;
    public List<int> bodySet;
    public List<int> rightArmSet;

    public Vector2Int[] golemPart;
    public string[] categorySets;
    public void golemImgSetting(Transform t)
    {
        List<int> lTemp;
        for (int i = 0; i < 4; ++i)
        {
            switch (i)
            {
                case 1:
                    lTemp = leftArmSet;
                    break;
                case 2:
                    lTemp = bodySet;
                    break;
                case 3:
                    lTemp = rightArmSet;
                    break;
                default:
                    lTemp = headSet;
                    break;
            }
            for (int l = 0; l < lTemp.Count; ++l)
            {
                SpriteResolver sr = t.GetChild(lTemp[l]).GetComponent<SpriteResolver>();
                sr.SetCategoryAndLabel(sr.GetCategory(), elementSort[golemPart[i].x]);
            }
        }
    }


    public Sprite[] StartBuffImg;
    public Sprite[] BuffImg;

    public unknownEventST[] events;

    public void continueGame(bool isContinue)
    {
        if (isContinue)
        {
            PlayerPrefs.SetInt("continue", 0);
        }
        else
        {
            PlayerPrefs.DeleteKey("continue");
        }
    }
    public string returnAddBuffEffectInfo(int a)
    {
        //버프는 걸리는 당장처리x 스킬블럭쓸때마다 버프창 탐색
        //위에서부터 0, 1, 2 ...
        //유저 : 힘(공격수치), 민첩(방어도수치), 지능(치유), 순환(마나회복), 마력(마나최대치) 
        //골렘 : 방어도(최대 체력변동), 갑옷(피해변동), 건강(지속 체력회복,감소), 가시(피해반사)
        //특수 :<<파워업카드가 생길때 마다 추가

        //불씨(화염류 카드를 뽑을때 마다 카드를 n장더 뽑습니다)
        //화염 친화(화염류 카드가 n의 피해를 더 줍니다.)
        //제련(카드를 뽑을 때 마다 방어도를 n 얻습니다.)
        switch (a)
        {
            case 0:
                return "힘 : 공격 수치가 변합니다";
            case 1:
                return "민첩 : 방어도 수치가 변합니다";
            case 2:
                return "지능 : 치유 수치가 변합니다";
            case 3:
                return "순환 : 마나 회복 수치가 변합니다";
            case 4:
                return "총명 : 매번 마나량이 변합니다";
            case 5:
                return "방어도 : 최대체력이 변합니다";
            case 6:
                return "갑옷 : 입는 피해가 변합니다";
            case 7:
                return "건강 : 매번 체력량이 변합니다";
            case 8:
                return "가시 : 피해를 입을 경우 적에게 피해를 줍니다";
            case 9:
                return "불씨 : 화염 카드를 뽑을 때 카드를 더 뽑습니다";
            case 10:
                return "불길 : 화염 카드가 피해를 더 줍니다";
            case 11:
                return "재련 : 카드를 뽑을 때 마다 방어도를 얻습니다";
            case 12:
                return "연소 : 방어도를 쌓을 때 마다 피해를 줍니다";
            case 13:
                return "의지 : 다음 스킬의 코스트가 0이 됩니다";
            case 14:
                return "저주 : 공격이 적의 건강수치를 감소시킵니다";
            case 15:
                return "옹달샘 : 피해를 입을 때마다 마나를 회복합니다";
            case 16:
                return "축복 : 카드를 사용할 때마다 방어도를 얻습니다";
            case 17:
                return "오답 : 5미만의 피해를 입을 경우 피해를 줍니다";
            case 18:
                return "폭풍의 눈 : 피해를 입으면 '바람' 카드를 얻습니다";
            case 19:
                return "잠깐 : 단 한번 체력변화를 0으로 바꿉니다";
            case 100:
                return "특수 : 이 카드는 전투에서 제거됩니다";
            case 101:
                return "복사 : 사용시 복사가 됩니다";
            case 102:
                return "생성 : 해당 카드를 얻습니다";
            default:
                return "";
        }
    }
}
