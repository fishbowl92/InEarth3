using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public GameManager GM;
    public MapManager mm;
    public int count;
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Q))
        {
            for (int i = 0; i < 3; ++i)
            {
                //newSkillBlock[i] = newSkillBlocks[Random.Range(0, newSkillBlocks.Count)];
                SkillBlock sb = GM.startSkillArray[count];
                Transform tTemp = mm.getCardPenal.GetChild(1 + i);
                tTemp.GetComponent<Image>().sprite = mm.cardBackImg[sb.skillTier];
                tTemp.Find("icon").GetComponent<Image>().sprite = sb.skillIcon;
                tTemp.Find("cost").GetComponent<TextMeshProUGUI>().text = sb.skillVal[0].x.ToString();
                tTemp.Find("name").GetComponent<TextMeshProUGUI>().text = sb.skillName;
                tTemp.Find("info").GetComponent<TextMeshProUGUI>().text = mm.getInfoText(sb.info, sb.skillVal[0].y, 0);
                count = (count + 1) % GM.startSkillArray.Length;
            }
        }
    }
}
