using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolData{

    public Vector3 location;
    public bool follow = false;
    public GameObject patrol;
    public int no;
    public int state;
    public float speed = 5f;

    public PatrolData(int no)
    {
        this.no = no;
        state = 0;
    }

    public void show()
    {
        patrol = GameObject.Instantiate(Resources.Load("Perfabs/BOT", typeof(GameObject)), location, Quaternion.identity, null) as GameObject;
        patrol.name = no.ToString();
    }
}
