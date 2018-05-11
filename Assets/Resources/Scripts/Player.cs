using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player{

    public GameObject player;
    public int location = 5;
    private Vector3 startLocation;
    private Vector3 loc;
    private float speed = 7f;

    public Player(Vector3 a)
    {
        startLocation = a;
        loc = a;
        player = GameObject.Instantiate(Resources.Load("Perfabs/Player", typeof(GameObject)), startLocation, Quaternion.identity, null) as GameObject;
    }

    public void move()
    {
        if(player.transform.position != loc)
        {
            player.GetComponent<Animation>().Play();
            player.transform.LookAt(loc);
            player.transform.position = Vector3.MoveTowards(player.transform.position, loc, speed * Time.deltaTime);
        }
        else
        {
            player.GetComponent<Animation>().Stop();
        }
    }

    public void setDis(float x,float z)
    {
        loc = player.transform.position;
        loc.x += x;
        loc.z += z;
        move();
    }

    public void reSet()
    {
        location = 5;
        GameObject.Destroy(player);
        player = GameObject.Instantiate(Resources.Load("Perfabs/Player", typeof(GameObject)), startLocation, Quaternion.identity, null) as GameObject;
    }
}
