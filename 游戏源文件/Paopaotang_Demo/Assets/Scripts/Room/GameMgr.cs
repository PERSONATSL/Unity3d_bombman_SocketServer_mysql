using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public string username = "";

    void Awake()
    {
        instance = this;
    }
}
