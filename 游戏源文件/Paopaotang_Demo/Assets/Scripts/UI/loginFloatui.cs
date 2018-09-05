using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class loginFloatui : MonoBehaviour {
    public Transform signupPanel,logininPanel;

    private void Start()
    {
        signupPanel.DOLocalMoveX(687f, 0.75f);
        logininPanel.DOLocalMoveX(298f, 0.75f);
    }

    public void SignpanelMove()
    {
        signupPanel.DOLocalMoveX(687f, 0.75f);
        logininPanel.DOLocalMoveX(298f, 0.75f);
    }
    public void LogininPanelMove()
    {
        signupPanel.DOLocalMoveX(298f, 0.75f);
        logininPanel.DOLocalMoveX(687f, 0.75f);
    }

}
