using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GlobalStateManager globalManager;
    public string type;

    [Range(1, 2)]
    public int playerNumber = 1;
    public float moveSpeed = 5f;
    public bool canDropBombs = true;
    public bool canMove = true;
    public bool dead = false;

    public GameObject bombPrefab;
    private Rigidbody rigidBody;
    private Transform myTransform;
    private Animator animator;

    public Text heartText, powerText, timeText;
    private int heart, power;
    private float timer, currentTimer;
    private Vector3 startPos;
    public int killNumber;
    public GameObject p;
    public Text kill;

    //炸弹预设
    public GameObject bomb;

    //网络同步
    private float lastSendInfoTime = float.MinValue;
    //操控类型
    public enum CtrlType
    {
        none,
        player,
        net,
    }
    public CtrlType ctrlType = CtrlType.player;
    void Start()
    {
        heart = 2;
        power = 2;
        timer = 0;
        currentTimer = 5;
        rigidBody = GetComponent<Rigidbody>();
        myTransform = transform;
        animator = GetComponent<Animator>();
        startPos = transform.position;
        globalManager = GameObject.Find("Manager").GetComponent<GlobalStateManager>();
    }

    void Update()
    {
        //网络同步
        if (ctrlType == CtrlType.net)
        {
            NetUpdate();
            return;
        }
        //操控
        UpdateMovement();

       // powerText.text = ": " + power + " m";
       // timeText.text = ": " + timer.ToString("F1") + " s";

        if (type == 1.ToString())
        {
            kill.text = killNumber.ToString();
            heartText.text = "#unknow";
        }
        else if (type == 2.ToString())
            //heartText.text = ": " + heart + " left";
        if (timer != 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;
        }
    }
    //last 上次的位置信息
    Vector3 lPos;
    Vector3 lRot;
    //forecast 预测的位置信息
    Vector3 fPos;
    Vector3 fRot;
    //时间间隔
    float delta = 1;
    //上次接收的时间
    float lastRecvInfoTime = float.MinValue;

    //位置预测
    public void NetForecastInfo(Vector3 nPos, Vector3 nRot)
    {
        //预测的位置
        fPos = lPos + (nPos - lPos) * 2;
        fRot = lRot + (nRot - lRot) * 2;
        if (Time.time - lastRecvInfoTime > 0.08f)
        {
            fPos = nPos;
            fRot = nRot;
        }
        //时间
        delta = Time.time - lastRecvInfoTime;
        //更新
        lPos = nPos;
        lRot = nRot;
        lastRecvInfoTime = Time.time;
    }

    //初始化位置预测数据
    public void InitNetCtrl()
    {
        lPos = transform.position;
        lRot = transform.eulerAngles;
        fPos = transform.position;
        fRot = transform.eulerAngles;
        Rigidbody r = GetComponent<Rigidbody>();
        r.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void NetUpdate()
    {
        //当前位置
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        //更新位置
        if (delta > 0)
        {
            transform.position = Vector3.Lerp(pos, fPos, delta);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(rot),
                                              Quaternion.Euler(fRot), delta);
        }
    }

    private void UpdateMovement()
    {
        animator.SetBool("Walking", false);
        if (!canMove)
            return;
        if (playerNumber == 1)
            UpdatePlayer1Movement();
        else
            UpdatePlayer2Movement();
    }
    private void UpdatePlayer1Movement()
    {
        //只有玩家操控才会生效
        if (ctrlType != CtrlType.player)
            return;

        if (Input.GetKey(KeyCode.W))
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(0, 0, 0), 1f);
            animator.SetBool("Walking", true);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.velocity = new Vector3(-moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(0, 270, 0), 1f);
            animator.SetBool("Walking", true);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(0, 180, 0), 1f);
            animator.SetBool("Walking", true);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigidBody.velocity = new Vector3(moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(0, 90, 0), 1f); 
            animator.SetBool("Walking", true);
        }
        if (canDropBombs && Input.GetKeyDown(KeyCode.Space) && timer == 0)
            DropBomb();

        //网络同步
        if (Time.time - lastSendInfoTime > 0.08f)
        {
            SendUnitInfo();
            lastSendInfoTime = Time.time;
        }
    }

    //本地单机游戏时启用操作2
    private void UpdatePlayer2Movement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(0, 0, 0), 1f);
            animator.SetBool("Walking", true);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigidBody.velocity = new Vector3(-moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(0, 270, 0), 1f);
            animator.SetBool("Walking", true);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(0, 180, 0), 1f);
            animator.SetBool("Walking", true);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            rigidBody.velocity = new Vector3(moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(0, 90, 0), 1f);
            animator.SetBool("Walking", true);
        }

        if (canDropBombs && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) && timer == 0)
            DropBomb();
    }
    private void DropBomb()
    {
        if (bombPrefab)
        {
            timer = currentTimer;
            GameObject bom = Instantiate(bombPrefab,
                new Vector3(Mathf.RoundToInt(myTransform.position.x), bombPrefab.transform.position.y, Mathf.RoundToInt(myTransform.position.z)),
                bombPrefab.transform.rotation);
            bom.GetComponent<Bomb>().range = power;
            if(type == 1.ToString())
                bom.GetComponent<Bomb>().ID = 1;
            else
                bom.GetComponent<Bomb>().ID = 2;

        }

    }

    //发送同步信息
    public void SendUnitInfo()
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("UpdateUnitInfo");
        //位置与旋转
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        proto.AddFloat(pos.x);
        proto.AddFloat(pos.y);
        proto.AddFloat(pos.z);
        proto.AddFloat(rot.x);
        proto.AddFloat(rot.y);
        proto.AddFloat(rot.z);
        NetMgr.srvConn.Send(proto);
    }
    public void SendShootInfo(Transform bulletTrans)
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("Shooting");
        //位置旋转
        Vector3 pos = bulletTrans.position;
        Vector3 rot = bulletTrans.eulerAngles;
        proto.AddFloat(pos.x);
        proto.AddFloat(pos.y);
        proto.AddFloat(pos.z);
        proto.AddFloat(rot.x);
        proto.AddFloat(rot.y);
        proto.AddFloat(rot.z);
        NetMgr.srvConn.Send(proto);
    }
    public void NetShoot(Vector3 pos, Vector3 rot)
    {
        //产生炮弹
        GameObject bulletObj = (GameObject)Instantiate(bomb, pos, Quaternion.Euler(rot));
        Bomb bombCmp = bulletObj.GetComponent<Bomb>();
    }
    public void SendHit(string username, float damage)
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("Hit");
        proto.AddString(username);
        NetMgr.srvConn.Send(proto);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!dead && other.CompareTag("Explosion") && type == 2.ToString())
        {
            if (heart > 0)
                heart -= 1;
            else
            {
                dead = true;
                globalManager.PlayerDied(playerNumber);
                Destroy(gameObject);
            }

        }
        else if (!dead && other.CompareTag("Explosion") && type == 1.ToString())
        {
            transform.position = startPos;
            p.GetComponent<Player>().killNumber += 1;
        }
        if (other.CompareTag("Timer"))
        {
            currentTimer -= 0.5f;
            if (currentTimer <= 1f)
                currentTimer = 1f;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Heart"))
        {
            heart++;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Power"))
        {
            power++;
            Destroy(other.gameObject);
            if (power >= 10)
                power = 10;
        }
    }
}
