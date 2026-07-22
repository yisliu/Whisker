using UnityEngine;

public class finishOrder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Customer")]
    public string itemTag = "bread";

    private void OnMouseDown()
    {
        foodDelivered();
    }

    // Update is called once per frame
    public void foodDelivered()
    {
        GameObject heldF = GameObject.FindWithTag(itemTag);
        if (heldF == null)
        {
            Debug.Log("No food");
        }
        else
        {
            Debug.Log("Food Delivered");
            Destroy(heldF);
            Destroy(gameObject);
        }
        //Destroy(gameObject);
    }
}
