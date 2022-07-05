using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetMonster : MonoBehaviour
{
    public int monDex;  // �� ��ø (��ø)
    public float monMaxHp, monCurHp;  // ���� �ִ�ü��, ����ü�� (��)
    public int monAddHp;

    public string curSkillName; // ���� �ߵ����� ��ų �̸�
    public bool isBoss;
    public float monSkillTriggerTime;    // ���� ��ų �ߵ��ð�
    public float monSkillTriggerRemainTime; // ���� �ߵ��ð�
    public int monSelectSkill;  // ���Ͱ� ������ ��ų��ȣ
    public string monName; // ���� �̸�, �ڵ�
    public int monTier; // 0.��, 1.����, 2.����
    public Animator anim;
    public bool isEnemyAction;  // ���Ͱ� �׼��� ���ߴ��� ���� üũ
                                // �ִϸ��̼ǿ��� ���� �׼��� ���� �Ǿ����� Ȯ�ο�
    public MonsterSkill nowSelectSkill;
    public float checkMonMaxHp()
    {
        return monAddHp + monMaxHp;
    }
    public MapManager mm;
    public void MonsterInit()
    {
        monAddHp = 0;
        //int[] num = { normalMonsters.Length, strongMonsters.Length, leaderMonsters.Length };
        mm.isStart = true;
        mm.reset();
        mm.cardDrawWhenBattleStart();
        mm.regenTimer = 0f;
        monsterDelay = 0.5f;
        Monster m = mm.monster;
        monMaxHp = m.baseHp + mm.floorLev * 14;
        this.GetComponent<Animator>().runtimeAnimatorController = m.anim;
        this.gameObject.SetActive(true);
        monCurHp = checkMonMaxHp();
        monTier = m.mTier;
        isEnemyAction = true;
        monsterState = 0;
        //monSkillTriggerRemainTime = 0;
        monSelectSkill = -1;    // ù��° ��ų���� ���
        mm.showMonsterHpUI();

        GameManager.Instans.saveGameProgress(4);
    }
    public int CheckAtk(int a)
    {
        return a + mm.manageBuff(MapManager.buffState.Atk, 2);
    }
    public int CheckDef()
    {
        return mm.manageBuff(MapManager.buffState.Def, 2);
    }

    public void CheckSkillTriggerTime()
    {
        monSkillTriggerRemainTime -= Time.deltaTime;
    }

    // ���� ���� : 0 - ���, 1 - ��ų�ߵ���, 2 - �ִϸ��̼ǽ�����
    public int monsterState = 0;
    public float monsterDelay;
    // ���� ���� üũ
    public void CheckMonster()
    {
        switch (monsterState)
        {
            case 0: // ������
                if (monsterDelay > 0)
                {
                    monsterDelay -= Time.deltaTime;
                    if (monsterDeath && monsterDelay <= 0)
                    {
                        if (mm.player.isGolemDeath)
                        {
                            mm.player.mp = 100;
                        }
                        monsterDeath = false;
                        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
                        /*if(!isBoss)Mapm.nextEvent(new Vector3Int(1, 1, 0));
                        else {
                            //������ ȹ�� // ȹ�� ������ nextEvent(new Vector3Int(1, 1, 0)); ����
                            isBoss = false;
                        }*/
                        mm.nextEvent(new Vector3Int(1, 1, 0)); //������ ȹ�� ������ �ش� �� ����
                    }
                    if (isEnemyAction == false)
                    {
                        MonsterAction();
                    }
                    break;
                }
                else
                {
                    isEnemyAction = false;
                    MonsterSelectAction();
                    mm.monSkillImage.color = Color.white;
                    monsterState = 1;
                }
                break;
            case 1: // ��ų�ߵ���
                CheckSkillTriggerTime();
                if (monCurHp < 0)    // ����� ��ų�� �ߵ����� �ʵ���
                {
                    monsterState = 0;
                }
                else if (monSkillTriggerRemainTime <= 0)
                {
                    if(nowSelectSkill == null)
                    {
                        monsterDelay = 0.5f;
                        monsterState = 0;
                    }
                    monsterState = 2;
                    string kind = "MonsterAtk";
                    switch (nowSelectSkill.isSkillKind)
                    {
                        case skillKind.BUFF :
                            kind = "MonsterBuff";
                            break;
                        case skillKind.SKILL:
                            kind = "MonsterSkill";
                            break;
                    }
                    anim.SetTrigger(kind);
                }
                break;
            case 2: // �ִϸ��̼ǽ�����
                if (monsterDelay > 0)
                {
                    monsterState = 0;
                    mm.monSkillImage.color = new Vector4(255, 255, 255, 0);
                    mm.monDmgKindText.text = "";
                }
                break;
        }
    }
    // ������ ���Ͱ� ���ϴ� �ൿ ����
    public void MonsterSelectAction()
    {
        ++monSelectSkill;
        if (monSelectSkill >= mm.monster.skills.Length)
        {
            if(monTier<2) mm.getBuffNewVersion(MapManager.buffState.Atk, 1, 2);
            monSelectSkill = 0;
        }
        nowSelectSkill = mm.monster.skills[monSelectSkill];
        nowSelectSkill.SetSkillKind();
        monSkillTriggerTime = nowSelectSkill.triggerTime;
        if (monTier < 3)
        {
            monSkillTriggerRemainTime = monSkillTriggerTime * (1 - (Mathf.Min(3, mm.floorLev)) * 0.2f);
        }
        else
        {
            monSkillTriggerRemainTime = monSkillTriggerTime;
        }
        mm.monSkillTriggerTimeText.text = monSkillTriggerRemainTime.ToString();
        mm.monSkillImage.sprite = nowSelectSkill.skillIcon;
    }
    /*
    public void RagePatternSelectAction()
    {
        MapManager mm = MapManager.instance;
        for(int i = 0; i < monSkillSetting.Length; ++i)
        {
            if(monSkillSetting[i].isSkillKind == skillKind.SKILL)
            {
                monSelectSkill = i;
                break;
            }
        }
        monSkillSetting[monSelectSkill].SetSkillKind();
        monSkillTriggerTime = monSkillSetting[monSelectSkill].triggerTime;
        monSkillTriggerRemainTime = monSkillTriggerTime;
        mm.monSkillTriggerTimeText.text = monSkillTriggerRemainTime.ToString();
        mm.monSkillImage.sprite = monSkillSetting[monSelectSkill].skillIcon;
    }
    */

    public void MonsterAction()
    {
        if (nowSelectSkill == null)
        {
            monsterDelay = 0.5f;
            monsterState = 0;
            return;
        }
        isEnemyAction = true;

        // ���Ͱ� ����ִٸ� ��ų ��� �Ҹ� ���
        if (monCurHp > 0)
        {
            for (int i = 0; i < nowSelectSkill.skillEvent.Length; ++i)
            {
                // ��ų �̺�Ʈ �ߵ�
                nowSelectSkill.skillEvent[i].Invoke(new MonsterSkill.TriggerData(i));
            }
            mm.atkAudio[mm.atkAudioCounter].clip = nowSelectSkill.sound;
            mm.StartCoroutine("atkAudioPlayer", new Vector3(0, 0.3f, mm.atkAudioCounter + 0.3f) + nowSelectSkill.soundData);
            mm.atkAudioCounter = (mm.atkAudioCounter + 1) % mm.atkAudio.Length;
            // ��ų������ ������ 0.3�� ���
            monsterDelay = 0.3f;
        }
    }

    public void HpAdd(int val)
    {
        if (monCurHp < 1) return;

        if (val < 0)
        {
            val = Mathf.Max(-CheckDef() - val, 1);
            APuDa(val);
        }
        else
        {
            monCurHp = Mathf.Min(monCurHp + val, checkMonMaxHp());
            mm.showDmgText(2, val);
        }
        mm.showMonsterHpUI();
    }

    public bool monsterDeath;
    public void APuDa(int dmg, bool isReflected = false)  // ���Ͱ� �����Ծ����� ����Ǵ� �Լ�
    {
        int gab = mm.manageBuff(MapManager.buffState.Time, 2);
        if (gab > 0)
        {
            dmg = 0;
            mm.getBuffNewVersion(MapManager.buffState.Time, -1, 2);
        }
        if (dmg <= 0)
        {
            monCurHp = Mathf.Min(monCurHp - dmg, checkMonMaxHp());
            mm.showDmgText(2, -dmg);
            mm.showMonsterHpUI();
        }
        else
        {
            dmg = Mathf.Max(1, dmg - CheckDef());
            if (dmg > mm.maxDmg) mm.maxDmg = dmg;
            monCurHp = Mathf.Max(0, monCurHp - dmg);
            mm.showDmgText(2, -dmg);
            if (!isReflected)
            {
                gab = mm.manageBuff(MapManager.buffState.Reflect, 2);
                if (gab != 0) mm.player.golemHpAdd(-gab, true);
            }
        }
        /*Transform temp = mm.showDmg(dmg, 0); //0 -������ ��, 1- ������ ���� 2- ȸ��
        temp.position = new Vector3(transform.position.x + Random.Range(-0.6f, -0.4f),
            transform.position.y + Random.Range(0.6f, 0.7f),
            temp.position.z);
            
        temp.gameObject.SetActive(true);
        temp.GetComponent<Animator>().SetTrigger("Enemy");*/
        mm.showMonsterHpUI(); 
        if (monCurHp < 1)
        {
            monCurHp = 0;
            monsterDelay = 0.5f;
            mm.reset();
            if (isBoss)
            {
                Player player = mm.player;
                player.hp = player.checkMaxHp();
                player.golemHp = player.checkGolemMaxHp();
                mm.showPlayerHpUI();
                mm.showGolemHpUI();
                isBoss = false;
                switch (mm.floorLev)
                {
                    case 1:
                    case 2:
                        mm.mapImg.GetChild(mm.floorLev).gameObject.SetActive(true);
                        mm.changeBGM(mm.floorLev - 1);
                        mm.actionCountText.text = mm.nowProcessNum + "/" + mm.maxProcessNum;
                        mm.actionCountText.fontSize = 55;
                        if (mm.player.hp >= 20) mm.youngCounter++;
                        break;
                    case 4:
                        if (mm.player.hp >= 20) mm.youngCounter += 2;
                        mm.ending(false);
                        //���� Ŭ���� ó��
                        break;
                }
            }
            monsterDeath = true;
            monsterState = 0;
            mm.getBuffText[2].gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}
