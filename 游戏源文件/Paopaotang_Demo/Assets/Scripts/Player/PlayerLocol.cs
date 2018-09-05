using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLocol : MonoBehaviour {

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
        //操控
        UpdateMovement();

        powerText.text = ": " + power + " m";
        timeText.text = ": " + timer.ToString("F1") + " s";

        if (type == 1.ToString())
        {
            kill.text = killNumber.ToString();
            heartText.text = "#unknow";
        }
        else if (type == 2.ToString())
            heartText.text = ": " + heart + " left";
            if (timer != 0)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                    timer = 0;
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
    }

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
            if (type == 1.ToString())
                bom.GetComponent<Bomb>().ID = 1;
            else
                bom.GetComponent<Bomb>().ID = 2;

        }

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
            p.GetComponent<PlayerLocol>().killNumber += 1;
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
