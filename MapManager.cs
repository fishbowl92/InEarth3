using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    GameManager GM;
    public static MapManager instance;

    public Monster monster;
    public int floorLev = 0; //층수 0 = 1층, 1 = 2층, 2 = 3층
    public int nowProcessNum = 0; // 진행도
    public int maxProcessNum; //층별 최대 진행도
    public int oreCount;
    public TextMeshProUGUI oreCountUIText;
    public int startMana = 3;
    public List<int> bossRandList;  // 중복되지 않는 보스와 싸우기 위해 싸웠던 보스 저장용
    public TextMeshProUGUI actionCountUIText;
    public Transform mapImg;
    public Sprite[] sandWatchIcon;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        isStart = false;
        changeMap(-1);
        changeMap(-1);
        bossRandList = new List<int> { 0, 1, 2, 3 };
        floorLev = 0;
        oreCount = 0;
        buffListNew[0] = new List<buffInfoNew>();
        buffListNew[1] = new List<buffInfoNew>();
        buffListNew[2] = new List<buffInfoNew>();
        anCounter = 0;

        GM = GameManager.Instans;
        for (int i = 9; i >= 0; --i)
        {
            myBlocks[i] = new DeckData();
            myBlocks[i].cost = -1;
        }
        GameObject[] backImgSet = Resources.LoadAll<GameObject>("BackImg");
        mapImg = Instantiate(backImgSet[Random.Range(0, backImgSet.Length)], Vector3.zero, Quaternion.identity).transform;

        for (int i = 0; i < 3; ++i)
        {
            mapImg.GetChild(i).gameObject.SetActive(i == 0);
        }

        StartCoroutine("checkUpdate");
        if (PlayerPrefs.GetInt("PlayerDeckElement") != -1 && PlayerPrefs.HasKey("PlayerDeckElement"))
        {
            GM.loadGameProgres();
            switch (PlayerPrefs.GetInt("savePoint"))
            {
                case 1:
                    //몹
                    Curtain.gameObject.SetActive(false);
                    break;
                case 2:
                    break;
                case 3:
                    string sTemp = PlayerPrefs.GetString("EventData");
                    for (int i = 0; i < GM.events.Length; ++i)
                    {
                        if (GM.events[i].name == sTemp)
                        {
                            PopupUISetting(4);
                            eventsSetting(GM.events[i]);
                            break;
                        }
                    }
                    break;
            }
        }
        else
        {
            GM.customedDeck.Clear();

            nowProcessNum = 0;
            actionCountText.text = (nowProcessNum) + "/" + maxProcessNum;
            floorText.text = (floorLev + 1).ToString();
            floorImg.sprite = floorSprites[floorLev];
            for (int i = 0; i < GM.startSkillArray.Length; ++i) //테스트 용으로 모든 스킬을 다 대입, random.Range 자리에 스킬 레벨 기입
            {
                DeckData newCard = new DeckData(GM.startSkillArray[i], 0);
                GM.customedDeck.Add(newCard);
                if (i == 0)
                {
                    Transform tf = firstCard.transform;
                    tf.Find("icon").GetComponent<Image>().sprite = newCard.sb.skillIcon;
                    tf.Find("cost").GetComponent<TextMeshProUGUI>().text = newCard.cost.ToString();
                    tf.Find("name").GetComponent<TextMeshProUGUI>().text = newCard.sb.skillName;
                    tf.Find("info").GetComponent<TextMeshProUGUI>().text = getInfoText(newCard.sb.info, newCard.sb.skillVal[0].y, 0);
                }
                else { addCardListUI(newCard); }
            }

            //sm.MonsterInit(GM.normalMonsters[Random.Range(0,GM.normalMonsters.Length)]);
            //SetBasicBlock();

            for (int i = 0; i < 4; ++i)
            {
                int gab = GM.golemPart[i].y;
                if (gab > 0)
                {
                    getEquipState(gab - 1);
                }
            }
            GM.golemImgSetting(player.golemTr);

            anims[1].SetFloat("Blend", 2f);
            settingAbletoHave();
        }
        buffTextSortSetting();
        settingHelpBox();
        showOreUIText();
        showPlayerHpUI();
        showGolemHpUI();
        settingsettingPenal();
    }

    public int checkStartMana()
    {
        for (int i = 0; i < equipList.Count; ++i)
        {
            if (equipList[i].buffSort == equipState.StartMana)
            {
                return startMana + equipList[i].count;
            }
        }
        return startMana;
    }

    public List<ShowDmg> showDmgTextPenalPlayer;
    public List<ShowDmg> showDmgTextPenalGolem;
    public List<ShowDmg> showDmgTextPenalEnemy;
    public int[] dmgTextConter;
    public void showDmgText(int target, int gab)
    {
        List<ShowDmg> sdPenal;
        switch (target)
        {
            case 0:
                sdPenal = showDmgTextPenalPlayer;
                break;
            case 1:
                sdPenal = showDmgTextPenalGolem;
                break;
            default:
                sdPenal = showDmgTextPenalEnemy;
                break;
        }
        sdPenal[dmgTextConter[target]].showDmg(gab);
        dmgTextConter[target] = (dmgTextConter[target] + 1) % 5;
    }



    public List<DeckData> willUseDeckList = new List<DeckData>();
    public List<DeckData> usedDeckList = new List<DeckData>();
    [SerializeField]
    public class DeckData
    {
        public SkillBlock sb;
        public int lev = 0;
        public int cost = -1;
        public DeckData()
        {

        }
        public DeckData(SkillBlock skillBlock, int level)
        {
            sb = skillBlock; 
            lev = level;
            cost = skillBlock.skillVal[lev].x;
        }
        public DeckData(SkillBlock skillBlock, int level, int COST)
        {
            sb = skillBlock; lev = level; cost = COST;
        }
        public DeckData deepCopy()
        {
            return new DeckData(sb, lev, cost);
        }
    }
    public Player player;
    public Transform monsterTr;
    public Image manaImg;
    public Image manaImgBack;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI manaResenText;
    public TextMeshProUGUI actionCountText;
    public TextMeshProUGUI floorText;
    public Image floorImg;
    public Sprite[] floorSprites;
    public int manaForMakeBlock = 10;
    public static WaitForSeconds FixWs = new WaitForSeconds(0.0333f);
    public Transform blockBox;
    [SerializeField]
    public DeckData[] myBlocks = new DeckData[10];

    public SetMonster sm;
    public TextMeshProUGUI monDmgKindText;
    public TextMeshProUGUI monSkillTriggerTimeText;
    public Image monSkillTriggerTimeImage;
    public Image monSkillImage;    // 유저에게 보이게 몬스터 사용 스킬 이미지

    // 몬스터 스킬 발동시간 표시
    public void ShowSkillTriggerTimeUI()
    {
        monSkillTriggerTimeText.text = "" + (int)sm.monSkillTriggerRemainTime;
        monSkillTriggerTimeImage.fillAmount = sm.monSkillTriggerRemainTime / sm.monSkillTriggerTime;
    }


    IEnumerator checkUpdate()
    {
        while (true)
        {
            for (int i = 9; i >= 0; --i)
            {
                if (myBlocks[i].cost < 0 && i > 0)
                {
                    if (myBlocks[i - 1].cost >= 0)
                    {
                        myBlocks[i] = myBlocks[i - 1].deepCopy();
                        myBlocks[i - 1].cost = -1;
                    }
                }
                Transform tTemp = blockBox.GetChild(i).GetChild(0);
                if (myBlocks[i].cost >= 0)
                {

                    tTemp.gameObject.SetActive(true);
                    Image iTemp = tTemp.Find("img").GetComponent<Image>();
                    iTemp.sprite = myBlocks[i].sb.skillIcon;
                    iTemp.color = (player.mp >= myBlocks[i].cost) ? Color.white : Color.gray;
                    tTemp.Find("cost").GetComponent<TextMeshProUGUI>().text = myBlocks[i].cost.ToString();
                    //마나부족하면 회색처리
                    //getchild(0배경, 1스킬이미지, 2레벨(iii), 3코스트
                }
                else
                {
                    tTemp.gameObject.SetActive(false);
                }
            }
            /*
            ++count;
            for (int j = 0; j < 3; ++j)
            {
                for (int i = 0; i < buffList[j].Count; ++i)
                {
                    if (buffList[j][i].buffTime < 3)
                    {

                        if (buffList[j][i].myUI.GetChild(2).localScale.x == 0 && count > 0)
                        {
                            buffList[j][i].myUI.GetChild(2).localScale = new Vector3Int(1, 1, 1);
                        }
                        else if (count < 0)
                        {
                            buffList[j][i].myUI.GetChild(2).localScale = new Vector3Int(0, 0, 0);
                        }

                    }
                }
            }
            if (count > 10) count = -10;*/
            if (sm.monCurHp > 0)
            {
                ShowSkillTriggerTimeUI();
            }

            float manaFill = player.mp / player.checkMaxMana();
            manaImg.fillAmount = Mathf.Lerp(manaImg.fillAmount, manaFill, 0.1f);
            manaImgBack.fillAmount = Mathf.Lerp(manaImgBack.fillAmount, manaFill, 0.1f);

            yield return FixWs;
        }
    }
    public Animator getNewBlockEffect;
    public int checkCostForGetNewBlock()
    {
        return manaForMakeBlock;
    }
    public void getNewBlockBtn()
    {
        StartCoroutine(getNewBlock(1, false, false));
    }
    public IEnumerator getNewBlock(int par = 1, bool addDraw = false, bool noCost = false)
    {
        WaitForSeconds ws = new WaitForSeconds(0.1f);
        int gab;
        for (int cnt = 0; cnt < par; ++cnt)
        {
            yield return ws;
            if ((willUseDeckList.Count == 0 && usedDeckList.Count == 0))
            {
                //Debug.Log("덱이 비었습니다");
                break;
            }
            else if (player.isGolemDeath)
            {
                //Debug.Log("골렘은 죽어 있습니다");
                break;
            }
            if ((player.mp >= checkCostForGetNewBlock() && myBlocks[0].cost < 0) || noCost)
            {
                player.manaAdd(-checkCostForGetNewBlock());
                gab = manageBuff(buffState.Smelting, 1);
                if(gab != 0) getBuffNewVersion(buffState.MaxHp, gab, 1, null, false);
                if (willUseDeckList.Count == 0)
                {
                    List<DeckData> temp = willUseDeckList;
                    willUseDeckList = usedDeckList;
                    usedDeckList = temp;
                    usedDeckList.Clear();
                }
                int rand = Random.Range(0, willUseDeckList.Count);

                myBlocks[0] = new DeckData(willUseDeckList[rand].sb, willUseDeckList[rand].lev, willUseDeckList[rand].cost);

                GM.btnSound[2].Play();
                getNewBlockEffect.SetTrigger("get");


                // 불씨용 추가 드로우
               
                if (!addDraw)
                {
                    gab = manageBuff(buffState.Ember, 0, 0);
                    if(gab>0&& willUseDeckList[rand].sb.isFire) { 
                        StartCoroutine(getNewBlock(gab, true, true));
                    }
                }
                willUseDeckList.RemoveAt(rand);
            }
        }
    }

    public void showCardInfo(Transform t)
    {
        int gab = (int)(t.position.z + 0.3f);
        skillBlockInfoSet[0].text = equipList[gab].getName();
        skillBlockInfoSet[1].text = equipList[gab].getInfo();
        helpInfoBox.text = "";
        for (int i = 0; i < 3; ++i)
        {
            plusSkillInfoPopUp[i].gameObject.SetActive(false);
        }
        Time.timeScale = 0.1f;
        skillblockInfoPenal.SetActive(true);
    }
    public Transform settingPenal;
    public void settingsettingPenal()
    {
        settingPenal.GetChild(0).GetChild(2).GetComponent<Slider>().value = PlayerPrefs.GetFloat("BGMvalue");
        settingPenal.GetChild(1).GetChild(2).GetComponent<Slider>().value = PlayerPrefs.GetFloat("SFXvalue");
        showSkillInfo = PlayerPrefs.GetInt("SettingData") == 0;
        settingPenal.GetChild(2).GetChild(2).GetComponent<Toggle>().isOn = showSkillInfo;
    }
    public void saveSettingData()
    {
        PlayerPrefs.SetFloat("BGMvalue", settingPenal.GetChild(0).GetChild(2).GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("SFXvalue", settingPenal.GetChild(1).GetChild(2).GetComponent<Slider>().value);
        PlayerPrefs.SetInt("SettingData", showSkillInfo ? 0 : 1);
    }



    public GameObject skillblockInfoPenal;
    public TextMeshProUGUI[] skillBlockInfoSet;
    public DeckData prbUseSkillBlock;
    public Transform[] plusSkillInfoPopUp;
    public bool showSkillInfo;
    bool pointOut;
    public void changeSetting(bool a)
    {
        showSkillInfo = a;
    }
    public void skillBlockClickDown(int val)
    {
        if (player.isGolemDeath) return;
        if (myBlocks[val].cost < 0)
        {
            return;
        }
        pointOut = true;
        prbUseSkillBlock = myBlocks[val];
        if (!showSkillInfo) return;
        skillBlockInfoSet[0].text = prbUseSkillBlock.sb.skillName;
        skillBlockInfoSet[1].text = getInfoText(prbUseSkillBlock.sb.info, prbUseSkillBlock.sb.skillVal[0].y, 0);
        int i = 0;
        for (; i < Mathf.Min(prbUseSkillBlock.sb.plusInfoData.Length, 2); ++i)
        {
            plusSkillInfoPopUp[i].GetChild(1).GetComponent<TextMeshProUGUI>().text = GM.returnAddBuffEffectInfo(prbUseSkillBlock.sb.plusInfoData[i]);
            plusSkillInfoPopUp[i].gameObject.SetActive(true);
        }
        for (; i < 3; ++i)
        {
            plusSkillInfoPopUp[i].gameObject.SetActive(false);
        }
        if (prbUseSkillBlock.sb.boksaCard)
        {
            plusSkillInfoPopUp[2].gameObject.SetActive(true);

            SkillBlock skb = prbUseSkillBlock.sb.sb;
            plusSkillInfoPopUp[2].GetComponent<Image>().sprite = cardBackImg[Mathf.Clamp(skb.skillTier, 0, 2)];
            plusSkillInfoPopUp[2].GetChild(1).GetComponent<Image>().sprite = skb.skillIcon;
            plusSkillInfoPopUp[2].GetChild(4).GetComponent<TextMeshProUGUI>().text = skb.skillVal[0].x.ToString();
            plusSkillInfoPopUp[2].GetChild(5).GetComponent<TextMeshProUGUI>().text = skb.skillName;
            plusSkillInfoPopUp[2].GetChild(6).GetComponent<TextMeshProUGUI>().text = getInfoText(skb.info, skb.skillVal[0].y, 0);
        }
        skillblockInfoPenal.SetActive(true);
        Time.timeScale = 0.1f;
    }

    public void enemySkillClickDown()
    {
        skillBlockInfoSet[0].text = sm.nowSelectSkill.msName;
        skillBlockInfoSet[1].text = getInfoText(sm.nowSelectSkill.info, sm.nowSelectSkill.msVal[0].x, 2);


        int i = 0;
        for (; i < Mathf.Min(sm.nowSelectSkill.plusInfoData.Length, 2); ++i)
        {
            plusSkillInfoPopUp[i].GetChild(1).GetComponent<TextMeshProUGUI>().text = GM.returnAddBuffEffectInfo(sm.nowSelectSkill.plusInfoData[i]);
            plusSkillInfoPopUp[i].gameObject.SetActive(true);
        }
        for (; i < 3; ++i)
        {
            plusSkillInfoPopUp[i].gameObject.SetActive(false);
        }
        skillblockInfoPenal.SetActive(true);
        Time.timeScale = 0.1f;
    }
    public string getInfoText(string info, int val, int target)
    {
        // 힘스텟 얻기
        string[] dataArr = info.Split("@");
        string sTemp = "";
        int str = manageBuff(buffState.Atk, target);

        for (int i = 0; i < dataArr.Length; ++i)
        {
            switch (dataArr[i])
            {
                case "dmg":
                    sTemp += "<#00FFFF>" + Mathf.Max(1,val + str) + "</color>";
                    break;
                default:
                    sTemp += dataArr[i];
                    break;
            }
        }
        return sTemp;
    }
    public AudioSource[] atkAudio;
    public int atkAudioCounter;

    public Animator[] anims;
    public void skillBlockClick()
    {
        if (!pointOut || player.isGolemDeath) return;
        pointOut = false;
        skillblockInfoPenal.SetActive(false);
        Time.timeScale = 1;
        int zero = 0;
        int gab = manageBuff(buffState.Will, 0, 0);
        if (gab > 0)
        {
            getBuffNewVersion(buffState.Will, -1, 0);
            zero = 1;
        }
        if (prbUseSkillBlock.cost <= player.mp || zero > 0)
        {
            if (zero < 1) player.manaAdd(-prbUseSkillBlock.cost);
            gab = manageBuff(buffState.CardUseToMaxHpBuff, 0, 0);
            if (gab > 0)
            {
                getBuffNewVersion(buffState.MaxHp, gab, 1);
            }
            SkillBlock sb = prbUseSkillBlock.sb;
            if (!sb.isReinforceSkill) usedDeckList.Add(new DeckData(sb, 0));
            prbUseSkillBlock.cost = -1;
            if (player.golemHp < 1) return;
            for (int i = 0; i < sb.skillEvent.Length; ++i)
            {
                StartCoroutine(skillEventCoroutine(sb, i));
            }
            StartCoroutine("effectCoroutine", sb);
            atkAudio[atkAudioCounter].clip = sb.sound;

            StartCoroutine("atkAudioPlayer", new Vector3(0, 0.3f, atkAudioCounter + 0.3f) + sb.soundData);
            atkAudioCounter = (atkAudioCounter + 1) % atkAudio.Length;

            anims[sb.AnimNumber].SetTrigger("atk");
            //if (sb.animString != "") anims[(int)sb.target[0]].SetTrigger(sb.animString);

        }
    }
    IEnumerator skillEventCoroutine(SkillBlock sb, int i)
    {
        for (int j = 0; j < sb.skillVal[i].z; ++j)
        {
            sb.skillEvent[i].Invoke(new SkillBlock.TriggerData(i, 0));// 스킬 이벤트 발동
            yield return new WaitForSeconds(0.3f);
        }
    }
    IEnumerator effectCoroutine(SkillBlock sb)
    {
        for (int i = 0; i < sb.effect.Length; ++i)
        {
            Vector3 effectTr;
            switch (sb.effectPos[i].z)
            {
                case 0:
                    effectTr = player.transform.position + Vector3.up * 0.85f;
                    break;
                case 1:
                    effectTr = player.golemTr.position + Vector3.up * 3.1f;
                    break;
                default:
                    effectTr = monsterTr.position;
                    break;
            }
            //effectTr += new Vector3Int(sb.effectPos[i].x, sb.effectPos[i].y);
            getNewEffect(effectTr).StartCoroutine("startAnim", new EffectData(sb.effect[i], sb.effectPos[i].x));

            yield return new WaitForSeconds(0.2f);
            //getNewEffect(effectTr).StartCoroutine("startAnim", new EffectData(sb.effect[i], sb.skillVal[i].z)); //Z 스킬별 반복횟수
        }



    }
    public void useAllSkill()
    {
        for (int q = 0; q < myBlocks.Length; ++q)
        {
            if (myBlocks[q].cost != -1)
            {
                SkillBlock sb = myBlocks[q].sb;
                if (!sb.isReinforceSkill) usedDeckList.Add(new DeckData(sb, 0));
                if (player.golemHp < 1) continue;
                if (sb.skillName == "영혼절단") continue;
                for (int i = 0; i < sb.skillEvent.Length; ++i)
                {
                    for (int j = 0; j < sb.skillVal[i].z; ++j)
                    {
                        myBlocks[q].sb.skillEvent[i].Invoke(new SkillBlock.TriggerData(i, 0));// 스킬 이벤트 발동
                    }
                }
                for (int i = 0; i < sb.effect.Length; ++i)
                {
                    Vector3 effectTr;
                    switch (sb.effectPos[i].z)
                    {
                        case 0:
                            effectTr = player.transform.position + Vector3.up * 0.75f;
                            break;
                        case 1:
                            effectTr = player.golemTr.position + Vector3.up * 3.1f;
                            break;
                        default:
                            effectTr = monsterTr.position;
                            break;
                    }
                    effectTr += new Vector3Int(sb.effectPos[i].x, sb.effectPos[i].x);
                    getNewEffect(effectTr).StartCoroutine("startAnim", sb.effect[i]);
                }

                atkAudio[atkAudioCounter].clip = sb.sound;

                StartCoroutine("atkAudioPlayer", new Vector3(0, 0.3f, atkAudioCounter + 0.3f) + sb.soundData);
                atkAudioCounter = (atkAudioCounter + 1) % atkAudio.Length;

                anims[sb.AnimNumber].SetTrigger("atk");

                //if (sb.animString != "") anims[(int)sb.target[0]].SetTrigger(sb.animString);

                myBlocks[q].cost = -1;
            }
        }
    }
    public List<EffectManager> effectList = new List<EffectManager>();
    public EffectManager getNewEffect(Vector3 pos = new Vector3())
    {
        EffectManager eTemp = effectList.Find(x => x.myImg.sprite == null);
        if (eTemp == null)
        {
            eTemp = Instantiate(effectList[0].gameObject).GetComponent<EffectManager>();
            effectList.Add(eTemp);
        }
        eTemp.transform.position = pos;
        return eTemp;
    }
    delegate void getDmg(int a);
    public IEnumerator continuousAtk(Vector3Int _s)
    {
        //x대상, y데미지, z횟수
        WaitForSeconds ws = new WaitForSeconds(1.0f / _s.z);
        getDmg even;
        switch (_s.x)
        {
            case 0:
                even = player.hpAdd;
                break;
            case 1:
                even = player.golemHpAdd;
                break;
            default:
                even = player.hpAdd;
                break;
        }
        for (int i = 0; i < _s.z; ++i)
        {
            even(_s.y);
            yield return ws;
        }
    }
    IEnumerator atkAudioPlayer(Vector3 _s)
    {
        // x 첫 사운드까지 딜레이, y 횟수
        WaitForSeconds ws0 = new WaitForSeconds(1.0f / _s.y);
        yield return new WaitForSeconds(_s.x);
        for (int i = 0; i < (int)_s.y; ++i)
        {
            if (sm.monsterDeath)
            {
                break;
            }
            atkAudio[(int)_s.z].Play();
            yield return ws0;
        }
    }
    public void pointerOut()
    {
        skillblockInfoPenal.SetActive(false);
        pointOut = false;
        Time.timeScale = 1;
    }
    public Transform showPopupUI;
    public Transform cardListUI;
    public GameObject firstCard;
    public void PopupUISetting(int a)
    {
        showPopupUI.gameObject.SetActive(a > 0);
        if (a > 0)
        {
            GM.btnSound[0].Play();
            Time.timeScale = 0;
            for (int i = 1; i < showPopupUI.childCount; ++i)
            {
                showPopupUI.GetChild(i).gameObject.SetActive(i == a);
            }
        }
        else
        {
            Time.timeScale = 1;
            GM.btnSound[1].Play();
        }
    }


    public int step;

    [System.Serializable]
    public struct nodeData
    {
        public Sprite img;
        public string text;
        [TextArea(5, 15)]
        public string info;
    }
    //0.적, 1.강적, 2.대장
    //3.일반광물, 4.화염, 5.바람, 6.대지, 7.바다
    //8.미지, 9.보물, 10.휴식
    public nodeData[] nodeDatas;

    [System.Serializable]
    public class mapData
    {
        public Vector2Int[] myNode;
        public mapData()
        {
            myNode = new Vector2Int[4];
        }
        public static mapData operator %(mapData a, mapData b)
        {
            for (int i = 0; i < 4; ++i)
            {
                a.myNode[i] = b.myNode[i];
            }
            return a;
        }
    }

    public mapData[] next4MapData;
    public mapData[] next2MapData;
    public mapData nowMapData;
    public Vector3[] itemPersents;
    public Vector3[] enemyPersents;
    public void changeMap(int a)
    {
        bool open = a < 0;
        if (open) a = 0;
        //선택은 0왼쪽, 1오른쪽만
        nowMapData %= next2MapData[a];
        for (int i = 0; i < 2; ++i)
        {
            next2MapData[i] %= next4MapData[a * 2 + i];
            setMapNodeShow(i, next2MapData[i]);
        }
        for (int i = 0; i < 4; ++i)
        {
            settingMapData(next2MapData[i / 2], next4MapData[i]);
            setMapNodeShow(2 + i, next4MapData[i]);
        }
        if (!open)
        {
            GM.btnSound[4].Play();
            actionCountText.text = (++nowProcessNum) + "/" + maxProcessNum;
            checkEvent(0);
        }
    }
    public int EventGab;
    public void nextEvent()
    {
        checkEvent(EventGab + 1);
    }
    public Sprite[] orbSprites;
    public Image getOreImage;
    public TextMeshProUGUI getOreText;
    public Animator getOreAnim;
    public void getNewOre(Vector2Int vTemp, int count = 1)
    {
        oreCount += count;
        ++GM.myOres[vTemp.x - 3];
        getOreImage.sprite = orbSprites[vTemp.x - 3];
        //getOreText.text = "+" + count;
        getOreAnim.SetTrigger("start");

    }
    public void showOreUIText()
    {
        oreCountUIText.text = oreCount.ToString();
        GM.saveOreCount();
    }

    public void cardDrawWhenBattleStart()
    {
        for (int i = 0; i < equipList.Count; ++i)
        {
            switch (equipList[i].buffSort)
            {
                case equipState.StartMana:
                    player.manaAdd(equipList[i].count);
                    break;
                //return "시작시 마나를 획득합니다.";
                case equipState.StartCard:
                    StartCoroutine(getNewBlock(equipList[i].count, false, true));
                    break;
                //return "시작시 카드를 뽑습니다.";
                case equipState.StartGolemHpHeal:
                    player.golemHpAdd(equipList[i].count);
                    break;
                //return "시작시 골램이 체력을 얻습니다.";
                case equipState.StartMonsterDelay:
                    sm.monsterDelay += equipList[i].count;
                    break;
                //return "시작시 적이 더 오래 딜레이를 가집니다.";
                case equipState.StartGetOre:
                    oreCount += equipList[i].count;
                    showOreUIText();
                    break;
                //return "시작시 돈을 얻습니다.";
                case equipState.StartGetWill:
                    getBuffNewVersion(buffState.Will, equipList[i].count, 0);
                    //return "시작시 의지를 얻습니다.";
                    break;
                case equipState.StartGetMpRegen:
                    getBuffNewVersion(buffState.MpRegen, equipList[i].count, 0);
                    break;
                case equipState.StartGetHealBuff:
                    getBuffNewVersion(buffState.Heal, equipList[i].count, 0);
                    break;
                case equipState.StartGetDexBuff:
                    getBuffNewVersion(buffState.Dex, equipList[i].count, 0);
                    break;
                case equipState.StartGetThreeBuff:
                    getBuffNewVersion(buffState.Atk, equipList[i].count, 0);
                    getBuffNewVersion(buffState.Heal, equipList[i].count, 0);
                    getBuffNewVersion(buffState.Dex, equipList[i].count, 0);
                    break;
                default:
                    break;
            }
        }
    }
    public TextMeshProUGUI getPlayerAddHpText, getGolemAddHpText;
    public Animator getPlayerAddHpAnim, getGolemAddHpAnim;
    public void checkEvent(int a)
    {
        //0.적, 1.강적, 2.대장
        //3.일반광물, 4.화염, 5.바람, 6.대지, 7.바다
        //8.미지, 9.보물, 10.휴식 11.불러오기
        //a가 3을 넘거나
        EventGab = a;
        try
        {
            switch (nowMapData.myNode[EventGab].x)
            {
                case 0:
                    monster = GM.normalMonsters[Random.Range(0, GM.normalMonsters.Length)];
                    sm.MonsterInit();
                    nextEvent(Vector3Int.zero);
                    break;
                case 1:
                    monster = GM.strongMonsters[Random.Range(0, GM.strongMonsters.Length)];
                    sm.MonsterInit();
                    nextEvent(Vector3Int.zero);
                    break;
                case 2:
                    monster = GM.strongMonsters[Random.Range(0, GM.strongMonsters.Length)];
                    sm.MonsterInit();
                    nextEvent(Vector3Int.zero);
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    getNewOre(nowMapData.myNode[EventGab]);
                    showOreUIText();
                    nextEvent();
                    //광석얻기
                    break;
                case 8:
                    int ran = Random.Range(-2, GM.events.Length);
                    if (ran == -2)
                    {
                        monster = GM.strongMonsters[Random.Range(0, GM.strongMonsters.Length)];
                        sm.MonsterInit();
                        nextEvent(Vector3Int.zero);
                    }
                    else if (ran == -1)
                    {
                        monster = GM.normalMonsters[Random.Range(0, GM.normalMonsters.Length)];
                        sm.MonsterInit();
                        nextEvent(Vector3Int.zero);
                    }
                    else
                    {
                        PopupUISetting(4);
                        eventsSetting(GM.events[ran]);
                    }
                    break;
                case 10:
                    getPlayerAddHpText.text = "+" + 10;
                    getPlayerAddHpAnim.SetTrigger("start");
                    player.hpAdd(10);
                    getGolemAddHpText.text = "+" + 20;
                    getGolemAddHpAnim.SetTrigger("start");
                    player.golemHpAdd(20);
                    break;
                case 11:
                    nextEvent(Vector3Int.zero);
                    break;
                default:
                    nextEvent(new Vector3Int(1, 0, 0));
                    //다음노드 선택
                    break;
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            nextEvent(new Vector3Int(1, 0, 0));
        }


        //스뎁수 올려주기
    }
    public Transform eventUI;
    public void eventsSetting(unknownEventST st)
    {
        eventUI.GetChild(0).GetComponent<TextMeshProUGUI>().text = st.situation;
        eventUI.GetChild(1).GetComponent<Image>().sprite = st.sprite;
        Transform buttonSet = eventUI.GetChild(2);
        buttonSet.GetChild(0).gameObject.SetActive(true);
        buttonSet.GetChild(1).gameObject.SetActive(true);
        eventUI.GetChild(1).GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 350);
        if (st.selectC == "") buttonSet.GetChild(2).gameObject.SetActive(false);
        else
        {
            buttonSet.GetChild(2).gameObject.SetActive(true);
            buttonSet.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = st.selectC;
        }
        Transform a = buttonSet.GetChild(0);
        a.GetComponent<Button>().onClick.RemoveAllListeners();
        a.GetChild(0).GetComponent<TextMeshProUGUI>().text = st.selectA;
        for (int i = 0; i < st.selectAResult.Length; ++i)
        {
            int temp = i;
            a.GetComponent<Button>().onClick.AddListener(() => st.selectAResult[temp].Invoke(0, st.selectValA[temp]));
        }
        Transform b = buttonSet.GetChild(1);
        b.GetComponent<Button>().onClick.RemoveAllListeners();
        b.GetChild(0).GetComponent<TextMeshProUGUI>().text = st.selectB;
        for (int i = 0; i < st.selectBResult.Length; ++i)
        {
            int temp1 = i;
            b.GetComponent<Button>().onClick.AddListener(() => st.selectBResult[temp1].Invoke(1, st.selectValB[temp1]));
        }
        GM.saveGameProgress(6);
        PlayerPrefs.SetString("EventData", st.name);
    }
    public void getEquipState(int a)
    {
        switch (a)
        {
            default:
                setEquip(equipState.StartCard, 1);
                break;
            case 1:
                setEquip(equipState.StartMana, 10);
                break;
            case 2:
                setEquip(equipState.StartGolemHpHeal, 5);
                break;
            case 3:
                setEquip(equipState.StartMonsterDelay, 1);
                break;
            case 4:
                setEquip(equipState.StartGetOre, 1);
                break;
            case 5:
                setEquip(equipState.StartGetWill, 1);
                break;
            case 6:
                setEquip(equipState.StartGetMpRegen, 1);
                break;
            case 7:
                setEquip(equipState.StartGetDexBuff, 1);
                break;
            case 8:
                setEquip(equipState.StartGetHealBuff, 1);
                break;
            case 9:
                setEquip(equipState.StartGetThreeBuff, 1);
                break;
        }
    }

    public void choice()
    {
        getEquipState(Random.Range(0, 5));
        /*PopupUISetting(0);
        Time.timeScale = 1;
        settingEventPenal(0);*/
    }
    public void outTo(int i)
    {
        PopupUISetting(0);
        Time.timeScale = 1;
        settingEventPenal(0);
    }
    public AudioClip[] bgmList;
    public void changeBGM(int a)
    {
        AudioSource asouce = GetComponent<AudioSource>();
        asouce.Stop();
        asouce.clip = bgmList[a];
        asouce.Play();
    }
    public void nextEvent(Vector3Int vTemp)
    {
        if (vTemp.x == 1 && Curtain.gameObject.activeSelf)
        {
            //이미 열려있으면
            if (floorLev == 3)
            {
                Curtain.gameObject.SetActive(false);
                monster = GM.lastBoss;
                sm.monTier = 4;
                sm.isBoss = true;
                changeBGM(2);
                actionCountText.text = "광기의 골렘";
                floorLev = 4;
                sm.MonsterInit();
            }
            else if (nowProcessNum >= maxProcessNum)
            {
                Curtain.gameObject.SetActive(false);
                //현보스
                int a = Random.Range(0, bossRandList.Count);
                monster = GM.leaderMonsters[bossRandList[a]];
                sm.isBoss = true;
                bossRandList.RemoveAt(a);
                nowProcessNum = 0;
                ++floorLev;
                actionCountText.text = "대장";
                floorText.text = (floorLev + 1).ToString();
                floorImg.sprite = floorSprites[floorLev];
                sm.MonsterInit();
            }
            else
            {
                Transform tTemp = Curtain.transform;
                for (int i = 0; i < tTemp.childCount; ++i)
                {
                    tTemp.GetChild(i).gameObject.SetActive(false);
                }
                settingEventPenal(vTemp.y);
            }
            return;
        }
        else
        {
            StartCoroutine("curtainOnOff", vTemp);
        }
    }
    public Image Curtain;
    IEnumerator curtainOnOff(Vector3Int vTemp)
    {
        Curtain.gameObject.SetActive(true);
        Transform tTemp = Curtain.transform;
        for (int i = 0; i < tTemp.childCount; ++i)
        {
            tTemp.GetChild(i).gameObject.SetActive(false);
        }
        //x, 0: 닫기 1: 열기
        //y, 0: 노드선택 1: 카드선택 2:이벤트
        float end;
        if (vTemp.x == 0)
        {
            end = 0.3f;
            Curtain.fillAmount = 1;
            while (Curtain.fillAmount > end)
            {
                Curtain.fillAmount -= Time.deltaTime * 3;
                yield return null;
            }
            Curtain.gameObject.SetActive(false);
        }
        else
        {
            end = 0.95f;
            Curtain.fillAmount = 0.3f;
            while (Curtain.fillAmount <= end)
            {
                Curtain.fillAmount += Time.deltaTime * 3;
                yield return null;
            }
            Curtain.fillAmount = 1;
            settingEventPenal(vTemp.y);
        }
    }
    public IEnumerator getSpecificCard(DeckData a, int val)
    {
        for (int i = 0; i < val; ++i)
        {
            myBlocks[0] = a.deepCopy();
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void settingEventPenal(int a)
    {
        Transform tTemp = Curtain.transform.GetChild(a);
        tTemp.gameObject.SetActive(true);
        switch (a)
        {
            case 0:
                //노드선택
                GM.saveGameProgress(5);
                break;
            case 1:
                //새 카드선택
                openGetCardPenal();
                break;
            default:
                return;
        }
    }

    public Transform getCardPenal;
    public int[] newSkillTierPercent = new int[3] { 9, 5, 1 };
    public int[,] skillTierPercentCluster = new int[3, 3] { { 9, 5, 1 }, { 7, 4, 1 }, { 4, 2, 1 } };
    public List<int>[] newSkillBlocks = new List<int>[3];

    public void settingAbletoHave() // 파츠 별로 골렘 속성 체크 함수
    {
        int[] element = { 0, 0, 0, 0 };
        int max = 0;
        for (int i = 0; i < GM.golemPart.Length; ++i)
        {
            if (GM.golemPart[i].x > 0)
            {
                if (++element[GM.golemPart[i].x - 1] > max)
                {
                    max = element[GM.golemPart[i].x - 1];
                }
            }
        }
        int use = Random.Range(0, 4);
        for (int i = 0; i < 4; ++i)
        {
            use = (use + 1) % 4;
            if (max == element[use])
            {
                break;
            }
        }
        GM.settingAllSkillSet(use);
        checkShildPersentSetting(use);
        GM.saveGameProgress(use);    // 모든 정보 저장
    }
    SkillBlock[] cardPenalCard;
    public void openGetCardPenal()
    {
        canSelectNewCard = true;
        cardPenalCard = new SkillBlock[3];
        int percentSum = newSkillTierPercent[0] + newSkillTierPercent[1] + newSkillTierPercent[2];
        for (int i = 0; i < 3; ++i)
        {
            //newSkillBlock[i] = newSkillBlocks[Random.Range(0, newSkillBlocks.Count)];
            int a = Random.Range(0, percentSum);
            SkillBlock sb = new SkillBlock();
            if (a < newSkillTierPercent[0]) //노말
            {
                cardPenalCard[i] = GM.allSkillArrayNormal[Random.Range(0, GM.allSkillArrayNormal.Count)];
            }
            else if (a >= newSkillTierPercent[0] + newSkillTierPercent[1]) //히든
            {
                cardPenalCard[i] = GM.allSkillArrayHidden[Random.Range(0, GM.allSkillArrayHidden.Count)];
            }
            else  //레어
            {
                cardPenalCard[i] = GM.allSkillArrayRare[Random.Range(0, GM.allSkillArrayRare.Count)];
            }
            if (cardPenalCard[1] != null)
            {
                if (cardPenalCard[0].skillName == cardPenalCard[1].skillName)
                {
                    --i;
                    continue;
                }
            }
            if (cardPenalCard[2] != null)
                if (cardPenalCard[0].skillName == cardPenalCard[2].skillName || cardPenalCard[1].skillName == cardPenalCard[2].skillName)
                {
                    --i;
                    continue;
                }
            //SkillBlock sb = GM.allSkillArray[Random.Range(0, GM.allSkillArray.Length)];
            Transform tTemp = getCardPenal.GetChild(1 + i);
            tTemp.GetComponent<Image>().sprite = cardBackImg[cardPenalCard[i].skillTier];
            tTemp.Find("icon").GetComponent<Image>().sprite = cardPenalCard[i].skillIcon;
            tTemp.Find("cost").GetComponent<TextMeshProUGUI>().text = cardPenalCard[i].skillVal[0].x.ToString();
            tTemp.Find("name").GetComponent<TextMeshProUGUI>().text = cardPenalCard[i].skillName;
            tTemp.Find("info").GetComponent<TextMeshProUGUI>().text = getInfoText(cardPenalCard[i].info, cardPenalCard[i].skillVal[0].y, 0);
        }
        getCardPenal.gameObject.SetActive(true);
    }
    public Sprite[] cardBackImg;
    public Animator newCardGetAnim;
    public bool canSelectNewCard;
    public void selectNewCard(int a)
    {
        if (!canSelectNewCard && a != 3) return;
        canSelectNewCard = false;
        //0,1,2 카드 3, 선택취소
        if (a == 3)
        {
            GM.btnSound[1].Play();
            ++anCounter;
            nextEvent(new Vector3Int(1, 0, 0));
            return;
        }
        DeckData newCard = new DeckData(cardPenalCard[a], 0);
        GM.customedDeck.Add(newCard);
        addCardListUI(newCard);
        newCardGetAnim.SetTrigger("getCard" + a);
        GM.btnSound[3].Play();
    }

    public Transform selectMapPenal;
    public void setMapNodeShow(int a, mapData md)
    {
        Transform t = selectMapPenal.GetChild(a);
        for (int i = 0; i < 4; ++i)
        {
            Transform tTemp = t.GetChild(i);
            int gab = md.myNode[i].x;
            if (gab < 0)
            {
                tTemp.gameObject.SetActive(false);
            }
            else
            {
                tTemp.gameObject.SetActive(true);
                tTemp.GetChild(0).GetComponent<Image>().sprite = nodeDatas[gab].img;
                tTemp.GetChild(2).GetComponent<TextMeshProUGUI>().text = nodeDatas[gab].text;
            }
        }
    }
    public void settingMapData(mapData preb, mapData set)
    {
        int itemPercent = 0;
        int enemyPernect = 0;
        //0.적, 1.강적, 2.대장
        //3.일반광물, 4.화염, 5.바람, 6.대지, 7.바다
        //8.미지, 9.보물, 10.휴식
        for (int i = 0; i < 4; ++i)
        {
            switch (preb.myNode[i].x)
            {
                case 0:
                    itemPercent = 1;
                    break;
                case 1:
                    itemPercent = 2;
                    break;
                case 2:
                    itemPercent = 3;
                    break;
                case 3:
                    enemyPernect = 1;
                    break;
                case 8:
                case 9:
                case 10:
                    enemyPernect = 2;
                    break;
                default:
                    break;
            }
        }
        int count = 0;
        while (count == 0)
        {
            set.myNode[count].x = randomCounter(itemPersents[itemPercent], 2);
            if (set.myNode[count].x < 0)
            {
                set.myNode[count].y = 0;
                //아이템 없슴
            }
            else
            {
                set.myNode[count].x = Random.Range(3, 8);
                //이씀
                count++;
                //얼만큼 줄래?
                set.myNode[count].y = itemPercent;
            }

            set.myNode[count].x = randomCounter(enemyPersents[enemyPernect], 2);
            if (set.myNode[count].x < 0)
            {
                set.myNode[count].y = 0;
                //적 없슴
            }
            else
            {
                //이씀
                count++;
                //얼만큼 줄래?
                set.myNode[count].y = enemyPernect;
            }

            if (count == 0)
            {
                set.myNode[count].x = randomCounter(new Vector3(3, 0.2f, 5), 3);
                if (set.myNode[count].x < 0)
                {
                    set.myNode[count].y = 0;
                    //이벤트 없슴
                }
                else
                {
                    //이씀
                    //8.미지, 9.보물, 10.휴식
                    set.myNode[count].x = (Random.Range(0, 2) == 0) ? 8 : 10;
                    count++;
                    //얼만큼 줄래?
                    set.myNode[count].y = itemPercent;
                }
            }
        }
        for (int i = count; i < 4; ++i)
        {
            set.myNode[i].x = -1;
            if (set.myNode[i].y < 0) set.myNode[i].y = 0;
        }
    }
    public int randomCounter(Vector3 vTemp, float none)
    {
        float gab = vTemp.x + vTemp.y + vTemp.z;
        gab = Random.Range(-none, gab);
        if (gab < 0)
        {
            return -1;
        }
        else if (gab < vTemp.z)
        {
            return 2;
        }
        else if (gab < vTemp.z + vTemp.y)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public void CheckMana(bool up)
    {
        manaText.text = (int)player.mp + "/" + (int)player.checkMaxMana();
        manaResenText.text = "+" + player.checkManaRegen();
        if (up)
        {
            //뒤가 먼저오르고 앞
            manaImgBack.color = Color.green;
            manaImgBack.fillAmount = player.mp / player.checkMaxMana();
        }
        else
        {
            //앞이 먼저오르고 뒤
            manaImgBack.color = Color.red;
            manaImg.fillAmount = player.mp / player.checkMaxMana();
        }
    }

    public void addCardListUI(DeckData deckData)
    {
        Transform newCard = Instantiate(firstCard).transform;
        newCard.SetParent(cardListUI);
        SkillBlock sb = deckData.sb;
        newCard.localScale = new Vector2(1, 1);
        newCard.GetComponent<Image>().sprite = cardBackImg[Mathf.Clamp(sb.skillTier, 0, 2)];
        newCard.GetChild(1).GetComponent<Image>().sprite = sb.skillIcon;
        newCard.GetChild(4).GetComponent<TextMeshProUGUI>().text = deckData.cost.ToString();
        newCard.GetChild(5).GetComponent<TextMeshProUGUI>().text = sb.skillName;
        newCard.GetChild(6).GetComponent<TextMeshProUGUI>().text = getInfoText(sb.info, sb.skillVal[0].y, 0);
        //newCard.GetChild().GetComponent<TextMeshProUGUI>().text = newCard.lev.ToString(); //만약 레벨도 카드에 출력한다면
    }
    public TextMeshProUGUI playerHpUI;
    public Image playerHpBar;
    public TextMeshProUGUI golemHpUI;
    public Image golemHpBar;
    public TextMeshProUGUI monsterHpUI;
    public Image monsterHpUIimage;
    public void showPlayerHpUI()
    {
        playerHpUI.text = player.hp + "/" + player.checkMaxHp();
        playerHpBar.fillAmount = (float)player.hp / player.checkMaxHp();
    }
    float gardPersent;
    public void showGolemHpUI()
    {
        if (player.golemHp > player.checkGolemMaxHp()) player.golemHp = player.checkGolemMaxHp();
        golemHpUI.text = (int)player.golemHp + "/" + (int)player.checkGolemMaxHp();
        gardPersent = Mathf.Clamp(player.golemHp / player.checkGolemMaxHp(), 0, 0.999f);
        golemHpBar.fillAmount = gardPersent;
        shildCheckGab.text = (int)(gardPersent * 100) + "%";
        Color cTemp = Color.white;
        cTemp.a = gardPersent;
        shildImg.color = cTemp;
    }
    public void showMonsterHpUI()
    {
        if (sm.monCurHp > sm.checkMonMaxHp()) sm.monCurHp = sm.checkMonMaxHp();
        monsterHpUI.text = "" + (int)sm.monCurHp + "/" + sm.checkMonMaxHp();
        monsterHpUIimage.fillAmount = sm.monCurHp / sm.checkMonMaxHp();
    }
    public Transform shildCheck;
    public Image shildImg;
    public TextMeshProUGUI shildCheckGab;
    public void checkShildPersentSetting(int a)
    {
        shildImg = shildCheck.GetChild(a).GetComponent<Image>();
        for (int i = 3; i >= 0; --i)
        {
            if (i != a)
            {
                Destroy(shildCheck.GetChild(i).gameObject);
            }
        }
    }
    void regenFuc()
    {
        regenTimer += Time.deltaTime;
        if (regenTimer > regenLimit)
        {
            regenTimer = 0;
            int gab=manageBuff(buffState.HpRegen,2);
            sm.HpAdd(gab);
            if (gab > 0)
            {
                getBuffNewVersion(buffState.HpRegen, -1, 2);
            }
            else if(gab < 0)
            {
                getBuffNewVersion(buffState.HpRegen, 1, 2);
            }
            gab = manageBuff(buffState.HpRegen, 1);
            if (gab > 0)
            {
                player.golemHpAdd(gab);
                getBuffNewVersion(buffState.HpRegen, -1, 1);
            }
            else if(gab < 0)
            {
                player.golemLostHp(gab);
                getBuffNewVersion(buffState.HpRegen, 1, 1);
            }
            gab = manageBuff(buffState.Intel, 0);
            player.manaAdd(gab);
            if (gab > 0)
            {
                getBuffNewVersion(buffState.Intel, -1, 0);
            }
            else if(gab < 0){
                getBuffNewVersion(buffState.Intel, 1, 0);
            }
        }
    }
    public bool isStart = false;
    public float regenTimer = 0;
    public float regenLimit;
    public IEnumerator effectCoroutine(EffectData effect, int target, int repeat)
    {
        Vector3 effectTr;
        switch (target)
        {
            case 0:
                effectTr = player.transform.position + Vector3.up * 0.85f;
                break;
            case 1:
                effectTr = player.golemTr.position + Vector3.up * 3.1f;
                break;
            default:
                effectTr = monsterTr.position;
                break;
        }
        //effectTr += new Vector3Int(sb.effectPos[i].x, sb.effectPos[i].y);
        getNewEffect(effectTr).StartCoroutine("startAnim", new EffectData(effect, repeat));
        yield return null;
    }
    private void Update()
    {
        float maxMana = player.checkMaxMana();
        player.mp = Mathf.Clamp(player.mp + player.checkManaRegen() * Time.deltaTime, 0, maxMana);
        manaText.text = (int)player.mp + "/" + (int)maxMana;
        if (player.mp == maxMana && player.isGolemDeath)
        {
            for (int i = 0; i < myBlocks.Length; ++i)
            {
                Transform tTemp = blockBox.GetChild(i);
                if (myBlocks[i].cost != -1)
                {
                    usedDeckList.Add(myBlocks[i].deepCopy());
                    myBlocks[i].cost = -1;
                    //tTemp.gameObject.SetActive(false);
                }
                tTemp.GetComponent<Animator>().SetBool("GoIdle", false);
                tTemp.GetChild(0).gameObject.SetActive(false);
            }
            player.mp = 0;
            CheckMana(false);
            player.golemHp = 5;
            player.isGolemDeath = false;
            player.StartCoroutine(player.golemBlending(true, anims[1]));
            if (nowProcessNum == 0 && sm.monCurHp < 1)
            {
                player.hp = player.checkMaxHp();
                player.golemHp = player.checkGolemMaxHp();
                showPlayerHpUI();
                showGolemHpUI();
            }
            else
                player.golemHpAdd((int)player.checkGolemMaxHp() / 2);
        }
        if (isStart)
        {
            sm.CheckMonster();
            regenFuc();
            if (!player.isGolemDeath && player.golemHp <= 0)
            {
                player.golemDeathFunc();
            }
        }
        /*if (Input.GetKeyUp(KeyCode.Q))
        {
            //PopupUISetting(4);
            //eventsSetting(GM.events[Random.Range(0, GM.events.Length)]);
            // testMethod();
            //getNewBlock();
            ending(false);
        }
        if (Input.GetKeyUp(KeyCode.W)) {
            switch(Random.Range(0, 5))
            {
                case 0:
                    setEquip(equipState.StartCard, 1);
                    break;
                case 1:
                    setEquip(equipState.StartMana, 5);
                    break;
                case 2:
                    setEquip(equipState.StartGetOre, 1);
                    break;
                case 3:
                    setEquip(equipState.StartGolemHpHeal,3);
                    break;
                case 4:
                    setEquip(equipState.StartGetWill, 3);
                    break;
                default:
                    setEquip(equipState.StartMonsterDelay, 1);
                    break;
            }
        }*/
    }

    public void SelectMap(mapData md)
    {
        for (int i = 0; i < md.myNode.Length; ++i)
        {
            if (md.myNode[i].x >= 0 && md.myNode[i].x <= 2)
            {
                switch (md.myNode[i].x)
                {
                    case 0:
                        monster = GM.normalMonsters[Random.Range(0, GM.normalMonsters.Length)];
                        sm.MonsterInit();
                        newSkillTierPercent[0] = skillTierPercentCluster[0, 0];
                        newSkillTierPercent[1] = skillTierPercentCluster[0, 1];
                        newSkillTierPercent[2] = skillTierPercentCluster[0, 2];
                        break;
                    case 1:
                        monster = GM.strongMonsters[Random.Range(0, GM.strongMonsters.Length)];
                        sm.MonsterInit();
                        newSkillTierPercent[0] = skillTierPercentCluster[1, 0];
                        newSkillTierPercent[1] = skillTierPercentCluster[1, 1];
                        newSkillTierPercent[2] = skillTierPercentCluster[1, 2];
                        break;
                    case 2:
                        monster = GM.strongMonsters[Random.Range(0, GM.strongMonsters.Length)];
                        sm.MonsterInit();
                        newSkillTierPercent[0] = skillTierPercentCluster[2, 0];
                        newSkillTierPercent[1] = skillTierPercentCluster[2, 1];
                        newSkillTierPercent[2] = skillTierPercentCluster[2, 2];
                        break;
                }

            }
        }

    }
    /*public GameObject dmgText;
    public int dmgTextCount;
    public List<Transform> dmgTextList;
    public Color[] dmgTextColor;
    public Transform showDmg(int a, int type) //0 -데미지 줌, 1- 데미지 받음 2- 회복 3- 마나 회복
    {
        dmgTextCount %= 33;
        dmgTextCount++;
        Transform temp;
        if (dmgTextList.Count < dmgTextCount)
        {
            temp = Instantiate(dmgText, Vector3.back * dmgTextCount, Quaternion.identity).transform;
            dmgTextList.Add(temp);
        }
        else
        {
            temp = dmgTextList[dmgTextCount - 1];
        }
        temp.GetChild(0).GetComponent<TextMeshPro>().text = a.ToString();
        temp.GetChild(0).GetComponent<TextMeshPro>().color = dmgTextColor[type];
        temp.gameObject.SetActive(false);
        return temp;
    }*/
    public Transform[] buffSetPenal = new Transform[3];
    public Transform[] buffSetInvPenal = new Transform[3];
    public Transform statusPenal, statusInvPenal;
    public GameObject buffUIset;
    public GameObject cardUIset;

    public List<equipInfo> equipList = new List<equipInfo>();
    public class equipInfo
    {
        public equipState buffSort; //버프 종류
        public int target; // 버프 대상
        public Transform myUI;
        public int count; //중첩 수
        public string getName()
        {
            switch (buffSort)
            {
                case equipState.StartMana:
                    return "마나";
                case equipState.StartCard:
                    return "근원";
                case equipState.StartGolemHpHeal:
                    return "완드";
                case equipState.StartMonsterDelay:
                    return "소드";
                case equipState.StartGetOre:
                    return "펜타클";
                case equipState.StartGetWill:
                    return "컵";
                case equipState.StartGetMpRegen:
                    return "바보";
                case equipState.StartGetHealBuff:
                    return "연인";
                case equipState.StartGetDexBuff:
                    return "마법사";
                case equipState.StartGetThreeBuff:
                    return "세계";
                default:
                    return "";
            }
        }
        public string getInfo()
        {
            switch (buffSort)
            {
                case equipState.StartMana:
                    return "시작시 마나를 획득합니다.";
                case equipState.StartCard:
                    return "시작시 카드를 뽑습니다.";
                case equipState.StartGolemHpHeal:
                    return "시작시 골램이 체력을 얻습니다.";
                case equipState.StartMonsterDelay:
                    return "시작시 적이 더 오래 딜레이를 가집니다.";
                case equipState.StartGetOre:
                    return "시작시 광물을 얻습니다.";
                case equipState.StartGetWill:
                    return "시작시 의지를 얻습니다.";
                case equipState.StartGetMpRegen:
                    return "시작시 순환을 얻습니다.";
                case equipState.StartGetHealBuff:
                    return "시작시 지능을 얻습니다.";
                case equipState.StartGetDexBuff:
                    return "시작시 민첩을 얻습니다.";
                case equipState.StartGetThreeBuff:
                    return "시작시 힘, 민첩, 지능을 얻습니다.";
                default:
                    return "";
            }
        }
    }
    public enum equipState { StartCard = 0, StartMana, StartGolemHpHeal, StartMonsterDelay, StartGetOre, StartGetWill, StartGetMpRegen, StartGetDexBuff, StartGetHealBuff, StartGetThreeBuff }
    public Transform equipUIsetting()
    {
        Transform tTemp;
        if (statusInvPenal.childCount > 0)
        {
            tTemp = statusInvPenal.GetChild(0);
            tTemp.parent = statusPenal;
        }
        else
        {
            tTemp = Instantiate(cardUIset, statusPenal).transform;
        }
        return tTemp;
    }
    public void setEquip(equipState buffSort, int gab)
    {
        for (int i = 0; i < equipList.Count; ++i)
        {
            if (equipList[i].buffSort == buffSort)
            {
                int val = equipList[i].count += gab;
                if (val == 0)
                {
                    equipList[i].myUI.SetParent(statusInvPenal);
                    equipList.RemoveAt(i);
                    return;
                }
                TextMeshProUGUI temp = equipList[i].myUI.GetChild(1).GetComponent<TextMeshProUGUI>();
                if (val < 0) temp.color = Color.red;
                else temp.color = Color.green;
                temp.text = val.ToString();
                return;
            }
        }
        Transform ui = equipUIsetting();
        var bf = new equipInfo
        {
            buffSort = buffSort,
            target = 0,
            myUI = ui,
            count = gab
        };
        Vector3 vTemp = ui.position;
        vTemp.z = equipList.Count;
        ui.position = vTemp;
        ui.GetChild(0).GetComponent<Image>().sprite = GM.StartBuffImg[(int)buffSort];
        TextMeshProUGUI tmp = ui.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (gab < 0) tmp.color = Color.red;
        else tmp.color = Color.green;
        tmp.text = bf.count.ToString();
        equipList.Add(bf);
    }
    public Transform buffUITransform(int target)
    {
        Transform tTemp;
        if (buffSetInvPenal[target].childCount > 0)
        {
            tTemp = buffSetInvPenal[target].GetChild(0);
            tTemp.parent = buffSetPenal[target];
        }
        else
        {
            tTemp = Instantiate(buffUIset, buffSetPenal[target]).transform;
        }
        return tTemp;
    }


    public class buffInfoNew
    {
        public buffState buffSort; //버프 종류
        public int target; // 버프 대상
        public Transform myUI;
        public int count; //중첩 수
        public SkillBlock skillblock;
    }
    public List<buffInfoNew>[] buffListNew = new List<buffInfoNew>[3];
    public enum buffState { Atk = 0, Dex, Heal, MpRegen, Intel, MaxHp, Def, HpRegen, Reflect, Ember, 
        FriendlyFire, Smelting, Combustion, Will, CurseBlade, PainToMp, CardUseToMaxHpBuff, SmallAtkToReflect, PainToGetSpecificCard, Time };

    //유저 : 힘(공격수치), 민첩(방어도수치), 지능(치유), 순환(마나회복), 총명(마나변동), 
    //골렘 : 방어도(최대 체력변동), 갑옷(피해변동), 건강(지속 체력회복,감소), 가시(피해반사)

    //불씨(불속성 카드 뽑으면 추가 드로우), 화염친화(불속성 카드 데미지 증가), 제련(카드 뽑을때마다 방어도 증가), 연소(방어도 쌓을때마다 적에게 피해 5)
    //의지(다음 스킬 사용시 코스트 0)
    //저주받은 칼날 : 공격시 적에게 건강 -1 버프 부여함
    //피해 입으면 MP 얻음
    //5이하 피해에 특정 데미지 반사
    //피해 입으면 특정 카드 획득
    //카드 사용시 방어도 획득
    public int setBuffStateGab(int gab, buffState buffType)
    {
        return gab | buffStateCodeFilter((int)buffType + 1).x;
    }
    public int removeBuffStateGab(int gab, buffState buffType)
    {
        int a = 0;
        a = ~(a | (0b1 << (int)buffType));
        return gab & a;
    }
    public int getBuffStateGab(int gab, buffState buffType)
    {
        // 해당 위치값을 1과 bit and
        // 다시 원위치 해서 돌려줌
        Vector2Int filter = buffStateCodeFilter((int)buffType + 1);
        return (gab & filter.x) >> filter.y;
    }

    public Vector2Int buffStateCodeFilter(int buffType)
    {
        if (buffType > 0)
        {
            return new Vector2Int(0b1 << buffType, buffType);
        }
        return Vector2Int.zero;
    }

    public int[] BuffState = new int[3] { 0, 0, 0 }; // 0 - 플레이어, 1 - 골렘, 2 - 적


    public void getBuffNewVersion(buffState buffSort, int gab, int target, SkillBlock sb = null, bool checkDex = true)
    {
        //버프는 걸리는 당장처리x 스킬블럭쓸때마다 버프창 탐색
        //위에서부터 0, 1, 2 ...
        // 0 -  공격력, 1 - 최대체력, 2 - 회복카드 사용시 적용되는 회복력
        // 3 - 마나리젠, 4 - 마나 최대치, 5 - 방어력, 6 - 초당 회복량, 7 - 피해반사량
        //유저 : 힘(공격수치), 민첩(방어도수치), 지능(치유), 순환(마나회복), 총명(마나변동) 
        //골렘 : 방어도(최대 체력변동), 갑옷(피해변동), 건강(지속 체력회복,감소), 가시(피해반사)
        //특수 :<<파워업카드가 생길때 마다 추가

        //불씨(화염류 카드를 뽑을때 마다 카드를 n장더 뽑습니다)
        //화염 친화(화염류 카드가 n의 피해를 더 줍니다.)
        //제련(카드를 뽑을 때 마다 방어도를 n 얻습니다.)
        if (sm.monsterDeath || gab == 0) return;
        if (target == 1 && player.isGolemDeath) return;


        startGetBuffAnim(target, GM.BuffImg[(int)buffSort], buffSort);

        int add = 0;
        
        if (target == 1 && buffSort == buffState.MaxHp)
        {
            if (getBuffStateGab(BuffState[1], MapManager.buffState.Combustion) == 1)
            {
                for (int i = 0; i < buffListNew[1].Count; ++i)
                {
                    if (buffListNew[1][i].buffSort == MapManager.buffState.Combustion)  //combustion의 스킬블럭을 가져와야 되서 이렇게 놔둠
                    {
                        SkillBlock block = buffListNew[1][i].skillblock;
                        StartCoroutine("effectCoroutine", block);
                        atkAudio[atkAudioCounter].clip = block.sound;
                        StartCoroutine("atkAudioPlayer", new Vector3(0, 0.3f, atkAudioCounter + 0.3f) + block.soundData);
                        atkAudioCounter = (atkAudioCounter + 1) % atkAudio.Length;
                        sm.APuDa(buffListNew[1][i].count);
                    }
                }
            }
            if (checkDex)
            {
                add = manageBuff(buffState.Dex, 0);
            }
        }
        if (gab + add == 0) return;



        //적은 둘다!
        if (getBuffStateGab(BuffState[target], buffSort) == 1)
        {
            for (int i = 0; i < buffListNew[target].Count; ++i)
            {
                if (buffListNew[target][i].buffSort == buffSort)
                {
                    buffListNew[target][i].count += gab + add;
                    if (buffListNew[target][i].count == 0)
                    {
                        BuffState[target] = removeBuffStateGab(BuffState[target], buffSort);
                        buffListNew[target][i].myUI.parent = buffSetInvPenal[target];
                        buffListNew[target].RemoveAt(i);
                        return;
                    }
                    TextMeshProUGUI temp = buffListNew[target][i].myUI.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (buffListNew[target][i].count < 0) temp.color = Color.red;
                    else temp.color = Color.green;
                    temp.text = (buffListNew[target][i].count).ToString();
                    buffImmediate(buffSort, target, buffListNew[target][i].count);
                    return;
                }
            }
        }
        Transform ui = buffUITransform(target);
        // 중복버프 처리
        var bf = new buffInfoNew
        {
            buffSort = buffSort,
            target = target,
            myUI = ui,
            count = gab + add,
            skillblock = sb
        };
        ui.GetChild(0).GetComponent<Image>().sprite = GM.BuffImg[(int)buffSort];
        //ui.GetChild(2).GetComponent<Image>().sprite = buffImg[(int)buffSort];
        TextMeshProUGUI tmp = ui.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (gab < 0) tmp.color = Color.red;
        else tmp.color = Color.green;
        tmp.text = bf.count.ToString();
        buffImmediate(buffSort, target, gab + add);
        buffListNew[target].Add(bf);
        BuffState[target] = setBuffStateGab(BuffState[target], buffSort);
    }


    public void buffImmediate(buffState buffSort, int target, int val)//즉시 적용 파트
    {
        switch (buffSort)
        {
            case buffState.MpRegen:
                player.addMpRegen = val;
                manaResenText.text = "+" + player.checkManaRegen();
                break;
            case buffState.MaxHp:

                if (target < 2) // 인형, 골렘
                {
                    float addHps = val - player.addGolemMaxHp;
                    player.addGolemMaxHp = val;
                    if (addHps > 0) player.golemHp += addHps;
                    showGolemHpUI();
                }
                else  // 적
                {
                    int a = val - sm.monAddHp;
                    sm.monAddHp = val;
                    if (a > 0) sm.HpAdd(a);

                    showMonsterHpUI();
                }
                break;
        }
    }


    public SpriteRenderer[] getBuffImage;
    public Animator[] getBuffAnimator;
    public TextMesh[] getBuffText;
    public void startGetBuffAnim(int target, Sprite img, buffState bs)
    {
        getBuffImage[target].sprite = img;
        getBuffText[target].text = getBuffString(bs);
        getBuffAnimator[target].SetTrigger("get");
    }
    string getBuffString(buffState bs)
    {
        switch (bs)
        {
            case buffState.HpRegen:
                return "건강";
            case buffState.Dex:
                return "민첩";
            case buffState.Atk:
                return "힘";
            case buffState.Heal:
                return "지능";
            case buffState.MpRegen:
                return "순환";
            case buffState.Intel:
                return "총명";
            case buffState.MaxHp:
                return "방어도";
            case buffState.Def:
                return "갑옷";
            case buffState.Reflect:
                return "가시";
            case buffState.Will:
                return "의지";
        }
        return "";
    }
    public void buffTextSortSetting()
    {
        for (int i = 0; i < 3; ++i)
        {
            MeshRenderer mTemp = getBuffAnimator[i].transform.Find("text").GetComponent<MeshRenderer>();
            mTemp.sortingLayerName = "Effect";
            mTemp.sortingOrder = 1;
        }
    }

    public void reset()
    {
        for (int j = 0; j < 3; ++j)
        {
            if (buffListNew[j].Count == 0) continue;
            for (int i = 0; i < buffListNew[j].Count; ++i)
            {
                buffListNew[j][i].myUI.parent = buffSetInvPenal[j];
            }
            buffListNew[j].Clear();
        }
        for (int i = 0; i < myBlocks.Length; ++i)
        {
            myBlocks[i].cost = -1;
        }
        willUseDeckList.Clear();
        usedDeckList.Clear();
        for (int i = 0; i < GM.customedDeck.Count; ++i)
        {
            willUseDeckList.Add(GM.customedDeck[i].deepCopy());
        }
        player.addMaxHp = 0;
        player.addMaxMp = 0;
        player.addMpRegen = 0;
        manaResenText.text = "+" + player.checkManaRegen();
        player.addGolemHpRegen = 0;
        player.addGolemMaxHp = 0;
        player.mp = checkStartMana();
        showGolemHpUI();
        showPlayerHpUI();
    }
    public int manageBuff(buffState buffSort, int target, int kind = 0) // 0 - 버프 스텍 반환 1 - 버프 제거, 2 - 버프 스텍 반환뒤 버프 제거
    {
        try
        {
            if (getBuffStateGab(BuffState[target], buffSort) == 0) return 0;
            for (int i = 0; i < buffListNew[target].Count; ++i)
            {
                if (buffListNew[target][i].buffSort == buffSort)
                {
                    if (kind == 0) { return buffListNew[target][i].count; }
                    else
                    {
                        if (kind == 2) return buffListNew[target][i].count;
                        BuffState[target] = removeBuffStateGab(target, buffSort);
                        buffListNew[target][i].myUI.parent = buffSetInvPenal[target];
                        buffListNew[target].RemoveAt(i);
                        return 0;
                    }
                }
            }
        }
        catch (System.IndexOutOfRangeException)
        {

        }
        catch (System.NullReferenceException)
        {

        }catch(System.Exception e)
        {
        }
        return 0;
    }
    public void deleatAllBuff(int target)
    {
        BuffState[target] = 0;
        for (int i = buffListNew[target].Count - 1; i >= 0; --i)
        {
            buffListNew[target][i].myUI.parent = buffSetInvPenal[target];
            buffListNew[target].RemoveAt(i);
        }
        buffListNew[target].Clear();
    }

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
    public TextMeshProUGUI helpInfoBox;
    public GameObject prebSeletHelpBox;
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
    public void returnHome(int a)
    {
        for (int i = 0; i < 4; ++i)
        {
            GM.golemPart[i] = Vector2Int.zero;
        }
        if (a == 0)
        {
        }
        else
        {
            PlayerPrefs.SetInt("PlayerDeckElement", -1);
        }
        LoadingSceneManager.LoadScene("Main");
    }
    public Transform endingPenal;
    public void ending(bool die)
    {
        if (endingPenal.gameObject.activeSelf) return;
        endingPenal.gameObject.SetActive(true);
        StartCoroutine(endingText(die));
    }
    IEnumerator endingText(bool die)
    {
        endingPenal.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text =
            (die) ? "기절" : "승리";
        TextMeshProUGUI text = endingPenal.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = "";
        string show = string.Empty;
        if (die)
        {
            show = "눈앞이 아득해 집니다.";
        }
        else
        {
            int y = youngScore();
            int a = anScore();
            int j = juScore();

            show = "점수 평가\n\n(기획,개발,그래픽)'02' : ";
            show += y.ToString();
            show += "\n(프로그래밍)'MonamiWater' : ";
            show += j.ToString();
            show += "\n(프로그래밍)'artist3837' : ";
            show += a.ToString();
            show += "\n\n총 합 : ";
            show += (y + a + j).ToString();

            int clearData = PlayerPrefs.GetInt("ClearData");
            int clearScoreData = PlayerPrefs.GetInt("ClearScoreData");
            PlayerPrefs.SetInt("ClearData", clearData + 1);
            PlayerPrefs.SetInt("ClearScoreData", clearScoreData + y + a + j);
        }

        WaitForSeconds ws = new WaitForSeconds(0.07f);
        for (int i = 0; i < show.Length; ++i)
        {
            text.text += show[i];
            if (show[i] != ' ') GM.btnSound[4].Play();
            else yield return null;
            yield return ws;
        }
        yield return ws;
        endingPenal.GetChild(1).Find("btn").gameObject.SetActive(true);
    }
    public int youngCounter;
    public int anCounter;
    public int maxDmg;
    public int youngScore()
    {
        int scr = Mathf.Clamp(youngCounter * 100, 0 ,500);
        scr += Mathf.Min(equipList.Count * 30, 300);
        scr += Mathf.Min(GM.customedDeck.Count * 2, 100);
        scr += Mathf.Min(maxDmg, 100);
        return scr;
    }
    public int anScore()
    {
        return Mathf.Min(anCounter * 5, 50);
    }
    public int juScore()
    {
        int scr = 0;
        for (int i = 0; i < GM.customedDeck.Count; ++i)
        {
            switch (GM.customedDeck[i].sb.skillTier)
            {
                case 1:
                    scr += 1;
                    break;
                case 2:
                    scr += 3;
                    break;
            }
        }
        return Mathf.Min(scr, 50);
    }
    /*void testMethod()
    {
        SkillBlock sb =GM.sd[(skillTestVal++)%GM.sd.Length];
        if (skillTestVal == GM.sd.Length) skillTestVal = 0;
        if (sb.isGolemSkill && player.golemHp < 1) return;
        for (int i = 0; i < sb.skillEvent.Length; ++i)
        {
            StartCoroutine(skillEventCoroutine(sb, i));
        }
        StartCoroutine("effectCoroutine", sb);
        atkAudio[atkAudioCounter].clip = sb.sound;

        StartCoroutine("atkAudioPlayer", new Vector3(0, 0.3f, atkAudioCounter + 0.3f) + sb.soundData);
        atkAudioCounter = (atkAudioCounter + 1) % atkAudio.Length;

        anims[sb.AnimNumber].SetTrigger("atk");
    }*/
}


