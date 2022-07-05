using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int hp, maxHp, addMaxHp;
    public float golemHp, golemMaxHp, addGolemMaxHp;
    public int playerDmg;
    public int golemDef;
    public float mp = 50, maxMp = 200, mpRegen = 10, addMpRegen = 0, addMaxMp, addGolemHpRegen, golemHpRegen;
    public Transform golemTr;
    public EffectData golemReviveEffect;

    public bool isPierced;
    public bool isGolemDeath = false;
    // Start is called before the first frame update


    private void Awake()
    {
        hp = checkMaxHp();
        golemHp = checkGolemMaxHp();
    }
    public int checkMaxHp()
    {
        return maxHp + addMaxHp;
    }
    public int checkAtk() //
    {
        return playerDmg + Mapm.manageBuff(MapManager.buffState.Atk, 0);
    }
    public int checkDef()
    {
        return golemDef + Mapm.manageBuff(MapManager.buffState.Def, 1);
    }
    public float checkGolemMaxHp()
    {
        return golemMaxHp + addGolemMaxHp;
    }
    public float checkMaxMana()
    {
        return maxMp + addMaxMp;
    }
    public void hpAdd(int val)
    {
        if (val < 0)
        {
            hp += val;
            Mapm.showDmgText(0, val);
        }
        else
        {
            /*
            int healing = val;
            if (Mapm.getBuffStateGab(Mapm.BuffState[0], MapManager.buffState.Heal) == 1)
            {
                for (int i = 0; i < Mapm.buffListNew[0].Count; ++i)
                {
                    if (Mapm.buffListNew[0][i].buffSort == MapManager.buffState.Heal)
                    {
                        healing += Mapm.buffListNew[0][i].count;
                        break;
                    }
                }
            }
            */
            hp += val;
            Mapm.showDmgText(0, val);
        }
        if (hp < 1) //플레이어 죽음
        {
            Mapm.ending(true);
        }
        else if (hp > checkMaxHp())
        {
            hp = checkMaxHp();
        }
        Mapm.showPlayerHpUI();
    }
    public void golemLostHp(int val)
    {
        int dmg = val > 0 ? val : -val;
        golemHp -= dmg;
        Mapm.showDmgText(1, -dmg);
        if (golemHp < 0)
        {
            golemDeathFunc();
        }
        Mapm.showGolemHpUI();
    }
    public MapManager Mapm;
    public void golemHpAdd(int val)//
    {
        Animator anim = Mapm.anims[1];
        int gab = Mapm.manageBuff(MapManager.buffState.Time, 1);
        if (gab > 0)
        {
            val = 0;
            Mapm.getBuffNewVersion(MapManager.buffState.Time, -1, 1);
        }
        if (val < 0)
        {
            if (golemHp < 1)
            {
                hpAdd(-1);
                return;
            }
            val = Mathf.Max(-checkDef() - val, 1);
            golemHp -= val;
            Mapm.showDmgText(1, -val);

            gab = Mapm.manageBuff(MapManager.buffState.PainToMp, 0);
            if (gab != 0) mp += gab;
            gab = Mapm.manageBuff(MapManager.buffState.SmallAtkToReflect, 1);
            if (gab != 0 && val < 5)
            {
                Mapm.sm.APuDa(gab);
            }

            if (Mapm.getBuffStateGab(Mapm.BuffState[1], MapManager.buffState.PainToGetSpecificCard) == 1)
            {
                for (int i = 0; i < Mapm.buffListNew[1].Count; ++i)
                {
                    if (Mapm.buffListNew[1][i].buffSort == MapManager.buffState.PainToGetSpecificCard)
                    {
                        StartCoroutine(Mapm.getSpecificCard(new MapManager.DeckData(Mapm.buffListNew[1][i].skillblock, 0, Mapm.buffListNew[1][i].skillblock.skillVal[0].x), Mapm.buffListNew[1][i].count));
                        break;
                    }
                }
            }
            gab = Mapm.manageBuff(MapManager.buffState.Reflect, 1);
            if (gab != 0) Mapm.sm.APuDa(gab, true);
            float ran = Random.Range(0, checkGolemMaxHp());
            ran = isPierced ? ran * 2 : ran;
            if (ran > golemHp)
            {
                hpAdd(-1);
                isPierced = false;
            }
        }
        else
        {
            int healing = val;
            gab = Mapm.manageBuff(MapManager.buffState.Heal, 0);
            if (gab != 0)
            {
                healing += gab;
            }
            golemHp += healing;
            Mapm.showDmgText(1, healing);
        }
        if (golemHp < 1 && !isGolemDeath)
        {
            golemDeathFunc();
        }
        else if (golemHp > checkGolemMaxHp())
        {
            golemHp = checkGolemMaxHp();
        }
        Mapm.showGolemHpUI();
    }
    public void golemHpAdd(int val, bool isReflected = false)//
    {
        int gab = Mapm.manageBuff(MapManager.buffState.Time, 1);
        if(gab > 0)
        {
            val = 0;
            Mapm.getBuffNewVersion(MapManager.buffState.Time, -1, 1);
        }
        if (val < 0)
        {
            if (golemHp < 1)
            {
                hpAdd(-1);
                return;
            }
            val = Mathf.Max(-checkDef() - val, 1);
            golemHp -= val;
            Mapm.showDmgText(1, -val);

            if (!isReflected)
            {
                gab = Mapm.manageBuff(MapManager.buffState.PainToMp, 0);
                if (gab != 0) mp += gab;
                gab = Mapm.manageBuff(MapManager.buffState.SmallAtkToReflect, 1);
                if (gab != 0 && val < 5)
                {
                    Mapm.sm.APuDa(gab);
                }

                if (Mapm.getBuffStateGab(Mapm.BuffState[1], MapManager.buffState.PainToGetSpecificCard) == 1)
                {
                    for (int i = 0; i < Mapm.buffListNew[1].Count; ++i)
                    {
                        if (Mapm.buffListNew[1][i].buffSort == MapManager.buffState.PainToGetSpecificCard)
                        {
                            StartCoroutine(Mapm.getSpecificCard(new MapManager.DeckData(Mapm.buffListNew[1][i].skillblock, 0, Mapm.buffListNew[1][i].skillblock.skillVal[0].x), Mapm.buffListNew[1][i].count));
                            break;
                        }
                    }
                }
                gab = Mapm.manageBuff(MapManager.buffState.Reflect, 1);
                if (gab != 0) Mapm.sm.APuDa(gab, true);
                float ran = Random.Range(0, checkGolemMaxHp());
                ran = isPierced ? ran * 2 : ran;
                if (ran > golemHp)
                {
                    hpAdd(-1);
                    isPierced = false;
                }
            }
        }
        else
        {
            int healing = val;
            gab = Mapm.manageBuff(MapManager.buffState.Heal, 0);
            if (gab != 0)
            {
                healing += gab;
            }
            golemHp += healing;
            Mapm.showDmgText(1, healing);
        }
        if (golemHp < 1 && !isGolemDeath)
        {
            golemDeathFunc();
        }
        else if (golemHp > checkGolemMaxHp())
        {
            golemHp = checkGolemMaxHp();
        }
        Mapm.showGolemHpUI();
    }
    public void golemDeathFunc()
    {
        isGolemDeath = true;
        StartCoroutine(golemBlending(false, Mapm.anims[1]));
        Mapm.pointerOut();
        golemHp = 0;
        Mapm.showGolemHpUI();
        Mapm.deleatAllBuff(1);
        mp = 0;
        Mapm.CheckMana(false);
        // 카드 드롭 애니메이션
        for (int i = 0; i < Mapm.myBlocks.Length; ++i)
        {
            if (Mapm.myBlocks[i].cost != -1)
            {
                Animator aTemp = Mapm.blockBox.GetChild(i).GetComponent<Animator>();
                aTemp.SetBool("GoIdle", true);
                aTemp.SetTrigger("CardDrop");
            }
        }
        StartCoroutine(Mapm.effectCoroutine(golemReviveEffect, 0, 20));
        Mapm.pointerOut();
    }
    public IEnumerator golemBlending(bool alive, Animator anim)
    {
        if (alive)
        {
            float a = 0;
            while (a < 2)
            {
                a += Time.deltaTime;
                if (a > 2) a = 2;
                anim.SetFloat("Blend", a);
                yield return null;
            }
        }
        else
        {
            float a = 2;
            while (a > 0)
            {
                a -= Time.deltaTime;
                if (a < 0) a = 0;
                anim.SetFloat("Blend", a);
                yield return null;
            }
        }
    }
    public void manaAddition(float val)
    {
        manaAdd(val);
    }
    public void manaAdd(float val)
    {
        mp += val;
        if (mp < 0)
        {
            mp = 0;
        }
        else if (mp > checkMaxMana())
        {
            mp = checkMaxMana();
        }

        Mapm.CheckMana(val > 0);
    }
    public void manaAdd(int val)
    {
        mp += val;
        if (mp < 0)
        {
            mp = 0;
        }
        else if (mp > checkMaxMana())
        {
            mp = checkMaxMana();
        }

        Mapm.CheckMana(val > 0);
    }
    public float checkGolemHpRegen()
    {
        return golemHpRegen + addGolemHpRegen;
    }
    public float checkManaRegen()
    {
        return mpRegen + addMpRegen;
    }
    // Update is called once per frame

}
