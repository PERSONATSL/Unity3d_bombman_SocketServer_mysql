using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreateRoomFloatUI : MonoBehaviour {
    public static CreateRoomFloatUI instance;

    public Transform createRoomPanel, AllPanel, roomListPanel,matchRoomPanel;
    private void Start()
    {
        instance = this;
    }

    public void OnClicked_Create()
    {
        createRoomPanel.DOLocalMoveY(-14.2f, 0.5f);
        AllPanel.DOLocalMoveY(-586f, 0.5f);
    }
    public void OnCancelClicked_Create()
    {
        createRoomPanel.DOLocalMoveY(572f, 0.5f);
        AllPanel.DOLocalMoveY(-14.2f, 0.5f);
    }
    public void OnClicked_Roomlist()
    {
        roomListPanel.DOLocalMoveY(-14.2f, 0.5f);
        AllPanel.DOLocalMoveY(580f, 0.5f);
    }
    public void OnCancelClicked_Roomlist()
    {
        roomListPanel.DOLocalMoveY(-580f, 0.5f);
        AllPanel.DOLocalMoveY(-14.2f, 0.5f);
    }

    public void OnClicked_Match()
    {
        createRoomPanel.DOLocalMoveY(572f, 0.5f);
        matchRoomPanel.DOLocalMoveY(-14.2f, 0.5f);
        AllPanel.DOLocalMoveY(-586f, 0.5f);
    }
    public void OnCancelClicked_Match()
    {
        Invoke("OnDisableMatchPanel", 0.5f);
        matchRoomPanel.DOLocalMoveY(572f, 0.5f);
        AllPanel.DOLocalMoveY(-14.2f, 0.5f);
    }

    public void OnJoinClicked()
    {
 
        roomListPanel.DOLocalMoveY(-580f, 0.5f);
        matchRoomPanel.DOLocalMoveY(-14.2f, 0.5f);
    }

}
