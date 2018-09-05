using UnityEngine;
using UnityEngine.UI;

public class DeathModel : MonoBehaviour
{

    public Text timeLeft;
    public float matchTime;
    public GlobalStateManager globalManager;

    void Update()
    {
        matchTime -= Time.deltaTime;
        timeLeft.text = (int)matchTime + "s";
        if (matchTime <= 0)
        {
            matchTime = 0;
            globalManager.PlayerDied1();
        }
    }
}
