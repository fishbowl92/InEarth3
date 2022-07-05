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
                mm.actionCountText.text = "����"; 
                mm.changeBGM(Mathf.Clamp(mm.floorLev - 2, 0, 2));
                mm.sm.isBoss = true;
                break;
            default:
                mm.monster = lastBoss;
                mm.actionCountText.text = "������ ��";
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
                PlayerPrefs.SetInt("savePoint", 1); //���Ϳ� ����
                break;
            case 5:
                PlayerPrefs.SetInt("savePoint", 2); //��� ������
                break;
            case 6:
                PlayerPrefs.SetInt("savePoint", 3); //���� �̺�Ʈ ����
                break;

        }
        MapManager mm = MapManager.instance;
        //�����ؾߵɰ� 2.���Ӽ�
        //0.���� 1.ī�� 3.�񷥻���
        //3.����ü�� 4.��ü��/�ִ�ü�� 5.����
        //6.��ô�� ��ġ 7. ������ �� 8.������� 9.��ġ�������
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
    [ContextMenu("��ų ����")]
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
        //������ �ɸ��� ����ó��x ��ų���������� ����â Ž��
        //���������� 0, 1, 2 ...
        //���� : ��(���ݼ�ġ), ��ø(����ġ), ����(ġ��), ��ȯ(����ȸ��), ����(�����ִ�ġ) 
        //�� : ��(�ִ� ü�º���), ����(���غ���), �ǰ�(���� ü��ȸ��,����), ����(���عݻ�)
        //Ư�� :<<�Ŀ���ī�尡 ���涧 ���� �߰�

        //�Ҿ�(ȭ���� ī�带 ������ ���� ī�带 n��� �̽��ϴ�)
        //ȭ�� ģȭ(ȭ���� ī�尡 n�� ���ظ� �� �ݴϴ�.)
        //����(ī�带 ���� �� ���� ���� n ����ϴ�.)
        switch (a)
        {
            case 0:
                return "�� : ���� ��ġ�� ���մϴ�";
            case 1:
                return "��ø : �� ��ġ�� ���մϴ�";
            case 2:
                return "���� : ġ�� ��ġ�� ���մϴ�";
            case 3:
                return "��ȯ : ���� ȸ�� ��ġ�� ���մϴ�";
            case 4:
                return "�Ѹ� : �Ź� �������� ���մϴ�";
            case 5:
                return "�� : �ִ�ü���� ���մϴ�";
            case 6:
                return "���� : �Դ� ���ذ� ���մϴ�";
            case 7:
                return "�ǰ� : �Ź� ü�·��� ���մϴ�";
            case 8:
                return "���� : ���ظ� ���� ��� ������ ���ظ� �ݴϴ�";
            case 9:
                return "�Ҿ� : ȭ�� ī�带 ���� �� ī�带 �� �̽��ϴ�";
            case 10:
                return "�ұ� : ȭ�� ī�尡 ���ظ� �� �ݴϴ�";
            case 11:
                return "��� : ī�带 ���� �� ���� ���� ����ϴ�";
            case 12:
                return "���� : ���� ���� �� ���� ���ظ� �ݴϴ�";
            case 13:
                return "���� : ���� ��ų�� �ڽ�Ʈ�� 0�� �˴ϴ�";
            case 14:
                return "���� : ������ ���� �ǰ���ġ�� ���ҽ�ŵ�ϴ�";
            case 15:
                return "�˴޻� : ���ظ� ���� ������ ������ ȸ���մϴ�";
            case 16:
                return "�ູ : ī�带 ����� ������ ���� ����ϴ�";
            case 17:
                return "���� : 5�̸��� ���ظ� ���� ��� ���ظ� �ݴϴ�";
            case 18:
                return "��ǳ�� �� : ���ظ� ������ '�ٶ�' ī�带 ����ϴ�";
            case 19:
                return "��� : �� �ѹ� ü�º�ȭ�� 0���� �ٲߴϴ�";
            case 100:
                return "Ư�� : �� ī��� �������� ���ŵ˴ϴ�";
            case 101:
                return "���� : ���� ���簡 �˴ϴ�";
            case 102:
                return "���� : �ش� ī�带 ����ϴ�";
            default:
                return "";
        }
    }
}
