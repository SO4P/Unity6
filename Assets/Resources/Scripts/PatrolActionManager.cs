using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolActionManager : SSActionManager, ISSActionCallback
{
    public myGameObject sceneController;
    public PatrolGo move;
    private bool start = true;

    // Use this for initialization
    void Start()
    {
        sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
        sceneController.actionManager = this;
    }

    protected new void Update()
    {
        if (sceneController.state && start) {
            for (int i = 0; i < 6; i++) {
                move = PatrolGo.GetSSAction();
                this.RunAction(sceneController.patrol.getPatrol(i.ToString()).patrol, move, this);
            }
            start = false;
        }
        if (!sceneController.state)
        {
            start = true;
        }
        base.Update();
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null)
    {
        //  
    }
}
