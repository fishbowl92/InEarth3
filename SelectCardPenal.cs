using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCardPenal : MonoBehaviour
{
    public MapManager mm;
    // Start is called before the first frame update
    void Start()
    {
        mm = MapManager.instance;
    }
    
    public void endToSelect()
    {
        mm.nextEvent(new Vector3Int(1,0,0));
    }
}
