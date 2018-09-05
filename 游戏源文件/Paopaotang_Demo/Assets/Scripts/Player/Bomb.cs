using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    public AudioClip explosionSound;
    public GameObject explosionPrefab;
    public LayerMask levelMask;
    private bool exploded = false;
    public GameObject[] randomItem;
    private int itemNumber;
    public int range = 2;
    public int ID = 1;

    void Start()
    {
        Invoke("Explode", 3f);
    }

    void Explode()
    {
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        StartCoroutine(CreateExplosions(Vector3.forward));
        StartCoroutine(CreateExplosions(Vector3.right));
        StartCoroutine(CreateExplosions(Vector3.back));
        StartCoroutine(CreateExplosions(Vector3.left));

        GetComponent<MeshRenderer>().enabled = false;
        exploded = true;
        transform.Find("Collider").gameObject.SetActive(false);
        Destroy(gameObject, .3f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!exploded && other.CompareTag("Explosion"))
        {
            CancelInvoke("Explode");
            Explode();
        }
    }
    private int n;
    private IEnumerator CreateExplosions(Vector3 direction)
    {
        for (int i = 1; i < range; i++)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, direction - transform.up / 500, out hit, i, levelMask);

            if (!hit.collider)
                Instantiate(explosionPrefab, transform.position + (i * direction) - transform.up / 500, explosionPrefab.transform.rotation);
            else
            {
                if (hit.transform.gameObject.tag == "CanDestroyItem")
                {
                    Instantiate(explosionPrefab, transform.position + (i * direction) - transform.up / 500, explosionPrefab.transform.rotation);
                    n = Random.Range(0, 100);
                    if(ID == 1)
                    {
                        if (n < 15)
                            itemNumber = 1;
                        else if (n < 45)
                            itemNumber = 2;
                        else
                            itemNumber = -1;
                        if (itemNumber >= 0)
                            Instantiate(randomItem[itemNumber], hit.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        if (n < 10)
                            itemNumber = 0;
                        else if (n < 25)
                            itemNumber = 1;
                        else if (n < 55)
                            itemNumber = 2;
                        else
                            itemNumber = -1;
                        if (itemNumber >= 0)
                            Instantiate(randomItem[itemNumber], hit.transform.position, Quaternion.identity);
                    }
                    
                    Destroy(hit.transform.gameObject);
                }
                else
                    break;
            }
            yield return new WaitForSeconds(.02f);
        }
    }
}
