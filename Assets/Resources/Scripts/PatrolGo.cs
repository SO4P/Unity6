using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolGo : SSAction
{
    private Vector3 dis;
    private PatrolData patrol;
    private myGameObject sceneController;
    private GameObject player;
    private bool go = true;

    public static PatrolGo GetSSAction()
    {
        PatrolGo action = ScriptableObject.CreateInstance<PatrolGo>();
        return action;
    }

    // Use this for initialization
    public override void Start()
    {
        sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
        if (this.gameobject != null)
            patrol = sceneController.patrol.getPatrol(this.transform.name);
        dis = patrol.location;
        player = sceneController.player.player;
        this.gameobject.GetComponent<Animation>().Play();
    }

    // Update is called once per frame
    public override void Update () {
        if (!sceneController.state)
        {
            this.gameobject.GetComponent<Animation>().Stop();
            this.destroy = true;
            this.callback.SSActionEvent(this);
        }
        if (patrol.follow)
        {
            patrol.patrol.transform.LookAt(player.transform.position);
            patrol.patrol.transform.position = Vector3.MoveTowards(patrol.patrol.transform.position, player.transform.position, patrol.speed * Time.deltaTime);
        }
        else
        {
            if (!go)
            {
                setDis();
                patrol.state = (patrol.state + 1) % 4;
                go = true;
            }
            else
            {
                if (patrol.patrol.transform.position != dis)
                {
                    patrol.patrol.transform.LookAt(dis);
                    patrol.patrol.transform.position = Vector3.MoveTowards(patrol.patrol.transform.position, dis, patrol.speed * Time.deltaTime);
                }
                else
                {
                    go = false;
                }
            }
        }
    }

    private void setDis()
    {
        dis = patrol.location;
        switch (patrol.state)
        {
            case 0:
                dis.x += 5f;
                dis.z += 5f;
                break;
            case 1:
                dis.x += 5f;
                dis.z -= 5f;
                break;
            case 2:
                dis.x -= 5f;
                dis.z -= 5f;
                break;
            case 3:
                dis.x -= 5f;
                dis.z += 5f;
                break;
        }
    }
}
