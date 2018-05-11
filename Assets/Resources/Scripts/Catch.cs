using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catch : MonoBehaviour {

    public delegate void GameoverEvent();
    public static event GameoverEvent GameoverChange;

    void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.tag);
        //当玩家与巡逻兵相撞
        if (other.gameObject.tag == "Player")
        {
            //游戏结束，发布消息
            if (GameoverChange != null)
            {
                GameoverChange();
            }
        }
    }
}
