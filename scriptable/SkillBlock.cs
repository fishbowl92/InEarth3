using TMPro;
using UnityEngine;
using UnityEngine.Events;
public enum Target { player = 0, golem, enemy };
[CreateAssetMenu(fileName = "skillBlock", menuName = "Scriptable Object/skill")]

public class SkillBlock : ScriptableObject
{
    public string skillName;
    public Monster.Type skillType;
    public Vector3Int[] skillVal; // x - ���� �ڽ�Ʈ, y ������, ���� ��, z �ݺ�Ƚ��
    public int skillTier;
    public Target[] target;// ��ų ��� �÷��̾�, ��, �� ������� �з�
    public UnityEvent<TriggerData>[] skillEvent;
    [TextArea(10, 20)]
    public string info;
    public EffectData[] effect;
    public Vector3Int[] effectPos; //x,y ������κ����� ����Ʈ ��ġ  z - 0 �÷��̾�, 1 ��, 2 ����
    //public string animString;
    public int AnimNumber;  // ���� �ִϸ��̼��� �������� ����
    public Sprite skillIcon;
    public AudioClip sound;
    public Vector3 soundData;//x������, y�ݺ�Ƚ��
    public bool isReinforceSkill;
    public bool isFire;
    public bool boksaCard;
    public SkillBlock sb;
    public int[] plusInfoData;  // �������� �ɼ� ���� ��ȣ
    public class TriggerData
    {
        public int num;
        public int lv;
        public TriggerData(int number, int lev)
        {
            num = number;
            lv = lev;
        }
    }
    public void healPlayer(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        p.hpAdd(skillVal[m.num].y);
        Mapm.showPlayerHpUI();
    }
    public void defBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Def, skillVal[m.num].y, (int)target[m.num]);
    }
    public void AtkBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Atk, skillVal[m.num].y, (int)target[m.num]);
    }
    public void manaRegenBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.MpRegen, skillVal[m.num].y, (int)target[m.num]);
    }
    public void dexBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Dex, skillVal[m.num].y, (int)target[m.num]);
    }
    public void MaxHpBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.MaxHp, skillVal[m.num].y, (int)target[m.num]);
    }
    public void IntelBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Intel, skillVal[m.num].y, (int)target[m.num]);
    }
    public void healPowerBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Heal, skillVal[m.num].y, (int)target[m.num]);
    }
    public void combustionBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Combustion, skillVal[m.num].y, (int)target[m.num], sb);

    }
    public void golemMaxHpBuffByHand(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        int count = 0;
        for (int i = 0; i < Mapm.myBlocks.Length; ++i)
        {
            if (Mapm.myBlocks[i].cost != -1) ++count;
        }
        count *= skillVal[m.num].y;
        Mapm.getBuffNewVersion(MapManager.buffState.MaxHp, count, (int)target[m.num]);
    }
    public void golemHpRegenBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.HpRegen, skillVal[m.num].y, (int)target[m.num]);
    }
    public void golemReflectBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Reflect, skillVal[m.num].y, (int)target[m.num]);
    }
    public void golemReflectBuffByDmg(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        int gab = skillVal[m.num].y + p.checkAtk();
        Mapm.getBuffNewVersion(MapManager.buffState.Reflect, Mathf.Max(1, gab - Mapm.sm.CheckDef()), (int)target[m.num]);
    }
    public void ifFireMoreDrawBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Ember, skillVal[m.num].y, (int)target[m.num]);
    }

    public void fireAddtionalDmgBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.FriendlyFire, skillVal[m.num].y, (int)target[m.num]);
    }
    public void smeltingBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Smelting, skillVal[m.num].y, (int)target[m.num]);
    }
    public void healGolem(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        p.golemHpAdd(skillVal[m.num].y);

        Mapm.showGolemHpUI();
    }
    public void lostHpPlayer(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        p.hpAdd(-skillVal[m.num].y);
        Mapm.showPlayerHpUI();
    }
    public void lostHpGolem(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        p.golemLostHp(skillVal[m.num].y);
        Mapm.showGolemHpUI();
    }
    public void buffTest(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.Def, skillVal[m.num].y, (int)target[m.num]);
    }
    public void atk(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;

        if (target[m.num] == Target.enemy)
        {
            Mapm.sm.APuDa(skillVal[m.num].y + p.checkAtk() + elementSpecial());
        }
        else if (target[m.num] == Target.player)
        {
            p.hpAdd(-skillVal[m.num].y);
        }
        else if (target[m.num] == Target.golem)
        {
            p.golemHpAdd(-skillVal[m.num].y);
        }
        //�ִϸ��̼�
    }
    public void buffProportionAtk(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        Mapm.sm.APuDa(Mapm.manageBuff(MapManager.buffState.MaxHp, 1, 2) + p.checkAtk() + elementSpecial());
        //�ִϸ��̼�
    }
    public void buffProportionAtkNoConsume(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        Mapm.sm.APuDa(Mapm.manageBuff(MapManager.buffState.MaxHp, 1) + p.checkAtk() + elementSpecial());
        //�ִϸ��̼�
    }
    public void buffProportionAtkNoConsumeDef(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        Mapm.sm.APuDa(Mapm.manageBuff(MapManager.buffState.Def, 1) * skillVal[m.num].y + p.checkAtk() + elementSpecial());
        //�ִϸ��̼�
    }
    public void manaFill(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player player = Mapm.player;
        player.manaAddition(skillVal[m.num].y);
        //�ִϸ��̼�
    }
    public void copiedBlockToUsedBox(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        if (Mapm.willUseDeckList.Count > 0)
            Mapm.usedDeckList.Add(new MapManager.DeckData(this, 0));
        else
        {
            Mapm.usedDeckList.Add(new MapManager.DeckData(this, 0));
        }
    }
    public void nextSkill_is_cost_0(TriggerData m)
    {
        MapManager.instance.getBuffNewVersion(MapManager.buffState.Will, skillVal[m.num].y, 0);
    }
    public void getNewCard(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.StartCoroutine(Mapm.getNewBlock(skillVal[m.num].y, true, true));
    }
    public void getSpecificCard(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.StartCoroutine(Mapm.getSpecificCard(new MapManager.DeckData(sb, 0, sb.skillVal[0].x), skillVal[m.num].y));
    }
    public void atkAndGetCard(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;

        if (target[m.num] == Target.enemy)
        {
            int dmg = Mathf.Max(skillVal[m.num].y + p.checkAtk() - Mapm.sm.CheckDef(), 1);
            Mapm.StartCoroutine(Mapm.getNewBlock(dmg, true, true));
            Mapm.sm.APuDa(dmg);
        }
        else if (target[m.num] == Target.player)
        {
            p.hpAdd(-skillVal[m.num].y);
        }
        else if (target[m.num] == Target.golem)
        {
            p.golemHpAdd(-skillVal[m.num].y);
        }
        //�ִϸ��̼�
    }
    public void useAllSkill(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.useAllSkill();
    }
    public void specificCardInDeck(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.willUseDeckList.Add(new MapManager.DeckData(sb, 0));
    }
    public void maxHpBuffStackMultiple(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.MaxHp, Mapm.manageBuff(MapManager.buffState.MaxHp, 1)* skillVal[m.num].y, 1, checkDex: false);
    }
    public void hpRegenBuffStackMultiple(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        int stack = Mapm.manageBuff(MapManager.buffState.HpRegen, (int)target[m.num], 2);
        Mapm.getBuffNewVersion(MapManager.buffState.HpRegen, stack * skillVal[m.num].y, (int)target[m.num]);
    }
    public void relationship(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player player = Mapm.player;
        if (player.checkMaxHp() < player.hp + skillVal[m.num].y)
        {
            Mapm.getBuffNewVersion(MapManager.buffState.Dex, 1, 0);
        }
        player.hpAdd(skillVal[m.num].y);
    }
    public void curseBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.CurseBlade, skillVal[m.num].y, (int)target[m.num]);
    }
    public void reDuceHpRegenToReflect(TriggerData m)
    {//petrification, ��ȭ
        MapManager Mapm = MapManager.instance;
        int value = Mapm.manageBuff(MapManager.buffState.Reflect,1);
        value = Mathf.Min(0, -value);
        Mapm.getBuffNewVersion(MapManager.buffState.HpRegen, value, (int)target[m.num]);
    }
    public void painToMpBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.PainToMp, skillVal[m.num].y, (int)target[m.num]);
    }
    public void cardUseToMaxHpBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.CardUseToMaxHpBuff, skillVal[m.num].y, (int)target[m.num]);
    }
    public void smallAtkToReflectBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.SmallAtkToReflect, skillVal[m.num].y, (int)target[m.num]);
    }
    public void painToGetSpecificCardBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.getBuffNewVersion(MapManager.buffState.PainToGetSpecificCard, skillVal[m.num].y, (int)target[m.num], sb);
    }

    public void removeMaxHpBuff(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Mapm.manageBuff(MapManager.buffState.MaxHp, (int)target[m.num], 1);
        Mapm.showMonsterHpUI();
    }
    public void atkAndHeal(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        int dmg = skillVal[m.num].y + p.checkAtk();
        p.golemHpAdd(Mathf.Max(dmg - Mapm.sm.CheckDef(), 1));
        Mapm.sm.APuDa(dmg);
        //�ִϸ��̼�
    }
    /*
    public class TriggerData
    {
        public int num;
    }
    public void heal(TriggerData m)
    {
        if (target[m.num] == Target.golem) {
            // �� �Ŵ������� �� ������ ���ͼ� effect[m.num]�� ����Ʈ�� ������+(effectPos.x, effecrPos.y)�� ��ġ�� ���
            // skillVal[skill_lev] 
            // �� ���� ü�� ���ͼ� �����ֱ�
            // UI ���
        }
        else if (target[m.num] == Target.player)
        {
            // �� �Ŵ������� �÷��̾� ������ ���ͼ� effect[m.num]�� ����Ʈ�� ������+(effectPos.x, effecrPos.y)�� ��ġ�� ���
            // �÷��̾� ���� ü�� ���ͼ� �����ֱ�
            // UI ���
        }
    }
    public void getMana(TriggerData m)
    {
        // skillVal[skill_lev].x ��ŭ ���� ����
        // UI ���
    }
    public void atk(TriggerData m)
    {
        Vector3 startPos;
        Vector3 endPos;//���� ��ġ
        switch (effectPos[m.num].z)
        {
            case 0: //�÷��̾�
                //startPos = �÷��̾� ������ + new Vetor3(effectPos[m.num].x, effectPos[m.num].y);
                break;
            case 1: //��
                break;
            case 2: //����
                break;
        }
        //���⼭ �ڷ�ƾ���� ����Ʈ �����Ҽ� �ֵ��� �ڷ�ƾ�� ȣ��� player ��ũ��Ʈ�� startPos, endPos ����

    }
    */

    int elementSpecial(int target = 2)
    {
        MapManager Mapm = MapManager.instance;
        if (isFire)
        {
            return Mapm.manageBuff(MapManager.buffState.FriendlyFire, 0);
        }
        int gab = Mapm.manageBuff(MapManager.buffState.CurseBlade, 0);
        if (gab > 0) Mapm.getBuffNewVersion(MapManager.buffState.HpRegen, -gab, 2);
        return 0;
    }
}
