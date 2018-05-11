# 智能巡逻兵

+ 游戏设计要求：
  + 创建一个地图和若干巡逻兵(使用动画)；
  + 每个巡逻兵走一个3~5个边的凸多边型，位置数据是相对地址。即每次确定下一个目标位置，用自己当前位置为原点计算；
  + 巡逻兵在设定范围内感知到玩家，会自动追击玩家；
  + 失去玩家目标后，继续巡逻；
  + 计分：玩家每次甩掉一个巡逻兵计一分，与巡逻兵碰撞游戏结束；

+ UML类图
![UML](https://github.com/SO4P/Unity6/blob/master/UML.png)

+ 动画部分
  + 没有使用动画状态机，因为将动画加到状态机后动画就不动了，该状态的Motion为空，所以简单使用了Animation中的Start和Stop方法开始/结束动画
  
+ 使用订阅与发布模式传消息
  + 在两个触发器中使用该模式
    + 地图格子的触发器，每个方格是一个触发器，标记当前玩家所在的区域，当玩家离开该区域到达下一区域时，发布计分消息。
    + 巡逻兵触发器，当玩家与巡逻兵碰撞时，发布游戏结束消息。
  + 订阅者为场控myGameObject
  
+ 代码

    + 摄像头跟随脚本
    ```
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
    ```

    + myGameObject
    ```
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
        private float timer;  //计时器，记录玩家当前游玩的时间
        public int score = 0;  //记分员

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
    ```
    + 巡逻兵
        + PatrolData
        ```
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
        ```

        + PatrolFactory
        ```
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        public class PatrolFactory
        {

            private Vector3[] position =
            {
                new Vector3(-20,0f,-20),new Vector3(-20,0f,0),new Vector3(-20,0f,20),
                new Vector3(0,0f,-20),new Vector3(0,0f,0),new Vector3(0,0f,20)
            };
            private int no;
            private List<PatrolData> patrols;
            private int loc = 0;

            private static PatrolFactory instance;
            public static PatrolFactory getInstance()
            {
                if (instance == null)
                {
                    instance = new PatrolFactory();
                }
                return instance;
            }

            private PatrolFactory()
            {
                no = 0;
                patrols = new List<PatrolData>();
            }

            public void makePatrol(int num)
            {
                for(no = 0;no < num;no++)
                {
                    PatrolData temp = new PatrolData(no);
                    temp.location = position[no];
                    temp.show();
                    patrols.Add(temp);
                }
            }

            public PatrolData getPatrol(string no)
            {
                foreach(PatrolData patrol in patrols)
                {
                    if (no.Equals(patrol.no.ToString()))
                        return patrol;
                }
                return null;
            }

            public void follow(int no)
            {
                if (loc != 0)
                    patrols[loc - 1].follow = false;
                patrols[no - 1].follow = true;
                loc = no;
            }

            public void reSet()
            {
                patrols[loc - 1].follow = false;
                loc = 0;
                no = 0;
                foreach (PatrolData patrol in patrols)
                {
                    patrol.patrol.transform.position = position[no];
                    no++;
                }
            }
        }
        ```

        + 巡逻兵动作PatrolGo
        ```
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

        ```
        + 巡逻兵动作管理PatrolActionManager
        ```
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
        ```

    + Player
    ```
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
    ```

  + 触发器
    + 地板
    ```
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
    ```

    + 巡逻兵

    ```
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Catch : MonoBehaviour {

        public delegate void GameoverEvent();
        public static event GameoverEvent GameoverChange;

        void OnCollisionEnter(Collision other)
        {
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
    ```
+ [演示视频](https://github.com/SO4P/Unity6/blob/master/%E6%BC%94%E7%A4%BA%E8%A7%86%E9%A2%91.mp4)
