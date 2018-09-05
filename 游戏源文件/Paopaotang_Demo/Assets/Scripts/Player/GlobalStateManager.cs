using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class GlobalStateManager : MonoBehaviour
{
    public List<GameObject> Players = new List<GameObject> ();

    private int deadPlayers = 0;
    private int deadPlayerNumber = -1;
    public GameObject p1, p2;
    public GameObject endPanel;
    public Text res;

    

    public void PlayerDied (int playerNumber)
    {
        deadPlayers++;

        if (deadPlayers == 1)
        {
            deadPlayerNumber = playerNumber;
            Invoke ("CheckPlayersDeath", .3f);
        }
    }

    void CheckPlayersDeath ()
    {
        if (deadPlayers == 1)
        {

            if (deadPlayerNumber == 1)
            {
                res.text = "Player_B WIN !";
                endPanel.transform.DOLocalMoveY(0, .5f);
                p2.GetComponent<PlayerLocol>().enabled = false;             
            } else
            {
                res.text = "Player_A WIN !";
                endPanel.transform.DOLocalMoveY(0, .5f);
                p1.GetComponent<PlayerLocol>().enabled = false;
            }
        } else
        {
            res.text = "DRAW !";
            endPanel.transform.DOLocalMoveY(0, .5f);
        }
    }
    public void PlayerDied1()
    {       
        res.text = "Game Over !";
        endPanel.transform.DOLocalMoveY(0, .5f);
        p1.GetComponent<PlayerLocol>().enabled = false;
        p2.GetComponent<PlayerLocol>().enabled = false;
    }

}
