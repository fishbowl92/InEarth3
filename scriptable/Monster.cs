using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster Data", menuName = "Scriptable Object/Monster Data")]
public class Monster : ScriptableObject
{
    public string mName;
    public int mTier;   // ���� ��� - 0 : ��, 1 : ����, 2 : ����
    public int baseHp;

    public AnimatorOverrideController anim;
    public enum Type { fire = 0, water, wind, earth };  // ���� �Ӽ�
    [Header("�־��� ��ų ������� ��ų�� ���, �ߺ����� �ֱ� ����")]
    public MonsterSkill[] skills;    // ����ϴ� ��ų
}
