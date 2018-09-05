using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestroyFloatTips : MonoBehaviour
{

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.DOLocalMoveY(60, 0.75f);

    }
}
