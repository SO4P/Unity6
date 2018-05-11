using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myGameObject : MonoBehaviour, ISceneController, IUserAction
{
    public SSActionManager actionManager { get; set; }
    SSDirector director;
    public PatrolFactory patrol;
    public Player player;
    public Vector3 StartLocation = new Vector3(-5f, 1.5f, -5f);
    public bool state = true;
    private float timer;    //计时器，记录玩家当前游玩的时间
    public int score = 0;   //记分员

	// Use this for initialization
	void Awake () {
        director = SSDirector.getInstance();
        director.currentSceneController = this;
        loadResources();
	}
	
    public void loadResources()
    {
        patrol = PatrolFactory.getInstance();
        patrol.makePatrol(6);
        player = new Player(StartLocation);
    }

    public void reStart()
    {
        state = true;
        patrol.reSet();
        player.reSet();
        timer = 0;
        score = 0;
    }

    void OnGUI()
    {
        if (state)
        {
            timer += Time.fixedDeltaTime;
            GUI.TextField(new Rect(Screen.width - 100, 60, 100, 60), "Time:" + timer.ToString("G4"));
        }
        if (!state)
        {
            GUI.TextField(new Rect(Screen.width / 2, Screen.height / 2 - 60, 150, 60), "Your time:" + timer.ToString("G4") + "\n" + "Your score:" + score.ToString());
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 60, 60), "Restart"))
            {
                reStart();
            }
        }
    }

    void OnEnable()
    {
        //注册事件
        Catch.GameoverChange += Gameover;
        check.ScoreChange += ScoreChange;
    }

    void OnDisable()
    {
        //取消注册事件
        Catch.GameoverChange -= Gameover;
        check.ScoreChange -= ScoreChange;
    }

    void Gameover()
    {
        state = false;
    }

    void ScoreChange()
    {
        score++;
    }

    void Update()
    {
        if (state)
        {
            //获取方向键的偏移量
            float translationX = Input.GetAxis("Horizontal");
            float translationZ = Input.GetAxis("Vertical");
            //移动玩家
            player.setDis(translationX, translationZ);
        }
    }
}
