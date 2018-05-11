using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class check : MonoBehaviour
{
    public delegate void ScoreEvent();
    public static event ScoreEvent ScoreChange;

    public myGameObject sceneController;
    void Start()
    {
        sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            int temp = sceneController.player.location;
            sceneController.player.location = (int)(this.gameObject.transform.position.z + 40) / 20 + (int)(this.gameObject.transform.position.x + 20) / 20 * 3;
            sceneController.patrol.follow(sceneController.player.location);
            if(temp != sceneController.player.location)
            {
                if(ScoreChange != null)
                {
                    ScoreChange();
                }
            }
        }
    }
}
