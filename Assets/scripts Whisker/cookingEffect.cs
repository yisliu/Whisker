using UnityEngine;

public class cookingEffect : MonoBehaviour
{
    [Header("Prefab Input")]
    public GameObject objectA;
    public GameObject objectB;

    [Header("Prefab Output")]
    public GameObject objectR;

    public Transform spawnPoint;

    private GameObject currentA = null;

    private GameObject currentB = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(objectA.tag) && currentA == null)
        {
            currentA = other.gameObject;
        }
        else if (other.gameObject.CompareTag(objectB.tag) && currentB == null)
        {
            currentB = other.gameObject;
        }

        if (currentA != null && currentB != null)
        {
            cookedItem();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == currentA)
        {
            //currentA = null;
        }

        if (other.gameObject == currentB)
        {
            //currentB = null;
        }
    }

    private void cookedItem()
    {
        Destroy(currentA);
        Destroy(currentB);
        currentA = null;
        currentB = null;
        Vector3 spawnP = spawnPoint != null ? spawnPoint.position : transform.position;
        Instantiate(objectR, spawnP, Quaternion.identity);
    }
}
