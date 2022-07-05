using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public SpriteRenderer myImg;
    public IEnumerator startAnim(EffectData sd)
    {
        MapManager mm = MapManager.instance;
        myImg.sprite = null;
        yield return new WaitForSeconds(sd.startDelay);

        WaitForSeconds ws = new WaitForSeconds(Mathf.Max(0.0666f, sd.delay));
        for (int c = 0; c < sd.banbok + 1; ++c)
        {
            for (int i = 0; i < sd.spriteData.Length; ++i)
            {
                if (!mm.Curtain.gameObject.activeSelf)
                {
                    myImg.sprite = sd.spriteData[i];
                    yield return ws;
                }
                else break;
            }
        }
        myImg.sprite = null;
    }
}
