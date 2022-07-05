using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDmg : MonoBehaviour
{
    public GameManager GM;
    public Animator anim;
    private void Start()
    {
        GM = GameManager.Instans;
    }
    public Image[] myNum;
    public void showDmg(int a)
    {
        List<Sprite> font = (a >= 0) ? GM.dmgFontGreen : GM.dmgFontRed;
        if(a == 0)
        {
            for(int i = 0; i < 3; ++i)
            {
                myNum[i].gameObject.SetActive(false);
            }
            myNum[3].sprite = font[0];
            myNum[3].gameObject.SetActive(true);
        }
        else
        {
            a = Mathf.Min(Mathf.Abs(a), 9999);
            int len = (int)(Mathf.Log10(a)) + 1;
            for (int i = 0; i < 4; ++i)
            {
                if (i < 4 - len)
                {
                    myNum[i].gameObject.SetActive(false);
                }
                else
                {
                    int gab = (int)(a / Mathf.Pow(10, 3 - i)) % 10;
                    myNum[i].sprite = font[gab];
                    myNum[i].gameObject.SetActive(true);
                }
            }
            anim.SetTrigger("start");
        }
    }
}
