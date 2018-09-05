using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HomeRoomChangePanel : MonoBehaviour {
    public Transform ALLPanel;
    public float changeTime;
    public void ALLPanelChanged(int index)
    {
        switch (index) {
            case (0):
                ALLPanel.DOLocalMoveX(0, changeTime);
                break;
            case (1):
                ALLPanel.DOLocalMoveX(-977.83f, changeTime);
                break;
            case (2):
                ALLPanel.DOLocalMoveX(-1964.61f, changeTime);
                break;
            case (3):
                ALLPanel.DOLocalMoveX(-2957f, changeTime);
                break;
            case (4):
                ALLPanel.DOLocalMoveX(-3957.38f, changeTime);
                break;
        }

    }
}
