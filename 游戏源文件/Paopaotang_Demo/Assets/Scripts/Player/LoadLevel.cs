using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class LoadLevel : MonoBehaviour {
    public Transform tt;
    public void LoadLev(int i)
    {
        switch (i)
        {
            case (0):
                SceneManager.LoadScene(0);
                break;
            case (1):
                SceneManager.LoadScene(1);
                break;
            case (2):
                SceneManager.LoadScene(2);
                break;
            case (3):
                SceneManager.LoadScene(3);
                break;
            case (4):
                SceneManager.LoadScene(4);
                break;
        }
    }
    public void Quitt()
    {
        Application.Quit();
    }

    public void moveT()
    {
        tt.DOLocalMoveX(-173f, 0.5f);
    }
}
