using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private myGameObject sceneController;
    private Player player;

    // Use this for initialization
    void Start () {
        sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
        player = sceneController.player;
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.position = new Vector3(player.player.transform.position.x, this.transform.position.y, player.player.transform.position.z);
	}
}
