using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float Delay = 3f;

    void Start ()
    {
        Destroy (gameObject, Delay);
    }
}
