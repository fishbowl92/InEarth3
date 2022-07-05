using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum skillKind { BUFF = 0, ATK, SKILL };
[CreateAssetMenu(fileName = "Monster Skill Data", menuName = "Scriptable Object/Monster Skill Data")]
public class MonsterSkill : ScriptableObject
{   // 0 : ���, 1 : ����, 2~ : ��ų
    public string msName;
    public Target[] target; // ��ų ��� �÷��̾�, ��, �� ������� �з�
    public Vector2Int[] msVal;     // x : ��ų�� ����Ǵ� ��ġ , y : ����Ƚ��
    public int triggerTime; // �ߵ��ð�

    public skillKind isSkillKind;   // � ��ų���� ���� ǥ��
    public UnityEvent<TriggerData>[] skillEvent;
    [TextArea(10, 20)]
    public string info;
    public Sprite skillIcon;
    public AudioClip sound;
    [Header("y : �ݺ�Ƚ���� ����, x,z�� 0���� ����")]
    public Vector3 soundData;//x������, y�ݺ�Ƚ��
    public AnimationClip anim;  // ��ų �ִϸ��̼�
    public int[] plusInfoData;  // �������� �ɼ� ���� ��ȣ
    public class TriggerData
    {
        public int num;     // ��° ��ų
        public TriggerData(int number)
        {
            num = number;
        }
    }
    public void SetSkillKind()
    {
        MapManager Mapm = MapManager.instance;
        switch (isSkillKind)
        {
            case skillKind.ATK:
                Mapm.monDmgKindText.text = "����";
                break;
            case skillKind.BUFF:
                Mapm.monDmgKindText.text = "��ȭ";
                break;
            case skillKind.SKILL:
                Mapm.monDmgKindText.text = "��ų";
                break;
        }
    }
    public void Atk(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        Vector3Int val = new Vector3Int(1,-Mathf.Max(1, sm.CheckAtk(msVal[m.num].x)), msVal[m.num].y);
        Mapm.StartCoroutine(Mapm.continuousAtk(val));
    }
    public void Heal(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        sm.HpAdd(msVal[m.num].x + Mapm.manageBuff(MapManager.buffState.Heal, 2));
    }
    public void intBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Heal, msVal[m.num].x, (int)target[m.num]);
    }
    public void AtkBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Atk, msVal[m.num].x, (int)target[m.num]);
    }
    public void DefBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Def, msVal[m.num].x, (int)target[m.num]);
    }
    public void MaxHpBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.MaxHp, msVal[m.num].x, (int)target[m.num]);
    }
    public void HpRegenBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.HpRegen, msVal[m.num].x, (int)target[m.num]);
    }
    public void TimeBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Time, msVal[m.num].x, (int)target[m.num]);
    }
    public void AtkByHalfHp(TriggerData m)
    {//AncientBossEnergyRelease, ��� ����, ������
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        int dmg = (int)sm.monCurHp / 2;
        sm.monCurHp -= dmg;
        Vector3Int val = new Vector3Int(1, -dmg, msVal[m.num].y);
        Mapm.StartCoroutine(Mapm.continuousAtk(val));
    }
    public void AtkAndReduceAktBuffIfDmgToDoll(TriggerData m)
    {//ArcherParalyzingArrow ,����ȭ��, ���� ȭ��
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        int hp = Mapm.player.hp;
        Vector3Int val = new Vector3Int(1, -Mathf.Max(1, sm.CheckAtk(msVal[m.num].x)), msVal[m.num].y);
        Mapm.StartCoroutine(Mapm.continuousAtk(val));
        if (hp > Mapm.player.hp)
        {
            Mapm.getBuffNewVersion(MapManager.buffState.Atk, -2, 0);
        }
        /*
        if (p.hp < checkPlayerHp)
        {
            Mapm.getBuffNewVersion(MapManager.buffState.Atk, -2, 0);
        }

        */
    }
    public void AtkByDoublePercentDmgToDoll(TriggerData m)
    {//AssassinBladeDance, Į��
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        Player p = Mapm.player;
        p.isPierced = true;
        Vector3Int val = new Vector3Int(1, -Mathf.Max(1, sm.CheckAtk(msVal[m.num].x)), msVal[m.num].y);
        Mapm.StartCoroutine(Mapm.continuousAtk(val));
    }
    public void StealHalfMaxHpBuff(TriggerData m)
    {//BloodKingControl, ����
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        if (Mapm.manageBuff(MapManager.buffState.MaxHp, 1) != 0)
        {
            int half = Mapm.manageBuff(MapManager.buffState.MaxHp, 1) / 2;
            Mapm.getBuffNewVersion(MapManager.buffState.MaxHp, -half, 1);
            Mapm.getBuffNewVersion(MapManager.buffState.MaxHp, half, 2);
        }
    }
    public void AtkAndHealbyDmg(TriggerData m)
    {//Vampiric, BloodKingSlash, BloodKingSplit, ����, ������
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        Player p = Mapm.player;
        Vector3Int val = new Vector3Int(1, -Mathf.Max(1, sm.CheckAtk(msVal[m.num].x)), msVal[m.num].y);
        Mapm.StartCoroutine(Mapm.continuousAtk(val));
        sm.HpAdd(-p.checkDef() - val.y + Mapm.manageBuff(MapManager.buffState.Heal, 2));
        Mapm.showMonsterHpUI();
    }
    public void reflectBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Reflect, msVal[m.num].x, (int)target[m.num]);
    }
    public void maxHpBuffHalf(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        int half = Mapm.manageBuff(MapManager.buffState.MaxHp, (int)target[m.num]) / 2;
        Mapm.getBuffNewVersion(MapManager.buffState.MaxHp, -half, (int)target[m.num]);
    }
    public void asMaxMpAtkMutlple(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        for (int i = 0; i < Mapm.manageBuff(MapManager.buffState.Intel, 2); ++i)
        {
            Atk(m);
        }
    }
    public void asIntelAtk(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        Vector3Int val = new Vector3Int(1, -Mathf.Max(1, sm.CheckAtk(Mapm.manageBuff(MapManager.buffState.Intel, 2))), msVal[m.num].y);
        Mapm.StartCoroutine(Mapm.continuousAtk(val));
    }
    public void asMaxHpAtk(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        Vector3Int val = new Vector3Int(1, -(Mathf.Max(1, sm.CheckAtk(Mapm.manageBuff(MapManager.buffState.MaxHp, 2)))), msVal[m.num].y);
        Mapm.StartCoroutine(Mapm.continuousAtk(val));
    }
    public void atkPiercingMaxHpBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        int dmg = Mathf.Max(1, sm.CheckAtk(msVal[m.num].x));
        if (dmg > Mapm.manageBuff(MapManager.buffState.MaxHp, 1))
        {
            dmg = (int)(dmg * 1.5f);
        }
        Vector3Int val = new Vector3Int(1, -dmg, msVal[m.num].y);
        Mapm.StartCoroutine(Mapm.continuousAtk(val));
    }
    public void lostHp(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        //sm.monCurHp -= msVal[m.num].x;
        sm.APuDa(msVal[m.num].x);
        // ���� ���
        /*if (sm.monCurHp <= 0)
        {
            sm.monCurHp = 0;
            sm.gameObject.SetActive(false);
            Mapm.reset();
            Mapm.nextEvent(new Vector3Int(1, 1, 0));
            sm.monsterDelay = 1f;
            // (����) �ൿ�� ������Ʈ. ���ϳ� Ŭ����(�����϶�) ���� �ൿ�� 1����
            Mapm.CheckActionCountUI();
            Mapm.showMonsterHpUI();
        }*/
    }
    public void lostMp(TriggerData m)
    {
        MapManager.instance.player.manaAdd(-msVal[m.num].x);
    }
    public void manaRegenBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        Mapm.getBuffNewVersion(MapManager.buffState.MpRegen, msVal[m.num].x, (int)target[m.num]);
    }
    public void asIntelStealArmor(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        int val = Mapm.manageBuff(MapManager.buffState.Intel, 2);
        Mapm.getBuffNewVersion(MapManager.buffState.MaxHp, -val, 1);
        Mapm.getBuffNewVersion(MapManager.buffState.MaxHp, val, 2);
    }
    public void intelBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Intel, msVal[m.num].x, (int)target[m.num]);
    }
    public void dexBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Dex, msVal[m.num].x, (int)target[m.num]);
    }
    /*
    public void ragePattern(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        SetMonster sm = Mapm.sm;
        if (sm.monCurHp <= sm.monMaxHp / 2)
        {
            sm.RagePatternSelectAction();
        }
    }
    */
}