using TMPro;
using UnityEngine;
using UnityEngine.Events;
public enum Target { player = 0, golem, enemy };
[CreateAssetMenu(fileName = "skillBlock", menuName = "Scriptable Object/skill")]

public class SkillBlock : ScriptableObject
{
    public string skillName;
    public Monster.Type skillType;
    public Vector3Int[] skillVal; // x - 마나 코스트, y 데미지, 버프 값, z 반복횟수
    public int skillTier;
    public Target[] target;// 스킬 대상 플레이어, 골렘, 적 대상인지 분류
    public UnityEvent<TriggerData>[] skillEvent;
    [TextArea(10, 20)]
    public string info;
    public EffectData[] effect;
    public Vector3Int[] effectPos; //x,y 대상으로부터의 이펙트 위치  z - 0 플레이어, 1 골렘, 2 몬스터
    //public string animString;
    public int AnimNumber;  // 누가 애니메이션을 동작할지 선택
    public Sprite skillIcon;
    public AudioClip sound;
    public Vector3 soundData;//x딜레이, y반복횟수
    public bool isReinforceSkill;
    public bool isFire;
    public bool boksaCard;
    public SkillBlock sb;
    public int[] plusInfoData;  // 유저에게 옵션 설명 번호
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
        //애니메이션
    }
    public void buffProportionAtk(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        Mapm.sm.APuDa(Mapm.manageBuff(MapManager.buffState.MaxHp, 1, 2) + p.checkAtk() + elementSpecial());
        //애니메이션
    }
    public void buffProportionAtkNoConsume(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        Mapm.sm.APuDa(Mapm.manageBuff(MapManager.buffState.MaxHp, 1) + p.checkAtk() + elementSpecial());
        //애니메이션
    }
    public void buffProportionAtkNoConsumeDef(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player p = Mapm.player;
        Mapm.sm.APuDa(Mapm.manageBuff(MapManager.buffState.Def, 1) * skillVal[m.num].y + p.checkAtk() + elementSpecial());
        //애니메이션
    }
    public void manaFill(TriggerData m)
    {
        MapManager Mapm = MapManager.instance;
        Player player = Mapm.player;
        player.manaAddition(skillVal[m.num].y);
        //애니메이션
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
        //애니메이션
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
    {//petrification, 석화
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
        //애니메이션
    }
    /*
    public class TriggerData
    {
        public int num;
    }
    public void heal(TriggerData m)
    {
        if (target[m.num] == Target.golem) {
            // 맵 매니저에서 골렘 포지션 얻어와서 effect[m.num]의 이펙트를 포지션+(effectPos.x, effecrPos.y)의 위치에 출력
            // skillVal[skill_lev] 
            // 골렘 현재 체력 얻어와서 더해주기
            // UI 출력
        }
        else if (target[m.num] == Target.player)
        {
            // 맵 매니저에서 플레이어 포지션 얻어와서 effect[m.num]의 이펙트를 포지션+(effectPos.x, effecrPos.y)의 위치에 출력
            // 플레이어 현재 체력 얻어와서 더해주기
            // UI 출력
        }
    }
    public void getMana(TriggerData m)
    {
        // skillVal[skill_lev].x 만큼 마나 증가
        // UI 출력
    }
    public void atk(TriggerData m)
    {
        Vector3 startPos;
        Vector3 endPos;//몬스터 위치
        switch (effectPos[m.num].z)
        {
            case 0: //플레이어
                //startPos = 플레이어 포지션 + new Vetor3(effectPos[m.num].x, effectPos[m.num].y);
                break;
            case 1: //골렘
                break;
            case 2: //몬스터
                break;
        }
        //여기서 코루틴으로 이펙트 실행할수 있도록 코루틴이 호출될 player 스크립트로 startPos, endPos 전달

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
