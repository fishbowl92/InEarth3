using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster Data", menuName = "Scriptable Object/Monster Data")]
public class Monster : ScriptableObject
{
    public string mName;
    public int mTier;   // 몬스터 등급 - 0 : 적, 1 : 강적, 2 : 대장
    public int baseHp;

    public AnimatorOverrideController anim;
    public enum Type { fire = 0, water, wind, earth };  // 몬스터 속성
    [Header("넣어진 스킬 순서대로 스킬을 사용, 중복으로 넣기 가능")]
    public MonsterSkill[] skills;    // 사용하는 스킬
}
