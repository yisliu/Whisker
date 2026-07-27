using UnityEngine;

public class finishOrder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Customer")]
    public string itemTag = "bread";
	public float deliveryRadius = 2f;

    private void OnMouseDown()
    {
        foodDelivered();
    }

    // Update is called once per frame
    /*public void foodDelivered()
    {*/
/*
        GameObject heldF = GameObject.FindWithTag(itemTag);
        if (heldF == null)
        {
            Debug.Log("No food");
        }
        else
        {
            Debug.Log("Food Delivered");
			ScoreManager.Instance?.AddPoints(1000);
            Destroy(heldF);
            Destroy(gameObject);
        }
        //Destroy(gameObject);
*/
        /*GameObject heldF = GameObject.FindWithTag(itemTag);
        if (heldF == null)
        {
            Debug.Log("No food");
			ScoreManager.Instance?.AddPoints(-1000);

        }
        else
        {
            Debug.Log("Food Delivered");
			ScoreManager.Instance?.AddPoints(1000);
            Destroy(heldF);
			GetComponentInParent<customerAI>()?.CompleteOrder();
			Destroy(gameObject);
        }
	*/
/*
		GameObject heldF = null;
		Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, deliveryRadius); 
		foreach (Collider collider in nearbyColliders){
			if(collider.tag == itemTag){
				heldF = collider.gameObject;
				break;
			}
		}
		if(heldF != null){
			Debug.Log("Food Delivered");
			ScoreManager.Instance?.AddPoints(1000);
            Destroy(heldF);
			GetComponentInParent<customerAI>()?.CompleteOrder();
			Destroy(gameObject);
		}
		else{
			Debug.Log("No food");
			ScoreManager.Instance?.AddPoints(-1000);
		}
    }

*/
    public void foodDelivered()
    {
		PlayerMovementFixed player = FindObjectOfType<PlayerMovementFixed>();
		if (player == null || player.HeldObject == null)
		{
			GetComponentInParent<customerAI>()?.TriggerGreeting();
		}
		else if (!player.HeldObject.CompareTag(itemTag))
		{
			Debug.Log("Bad kitty!");
			GetComponentInParent<customerAI>()?.WrongOrder();
		}
		else
		{
			Debug.Log("Food Delivered");
			ScoreManager.Instance?.AddPoints(1000);
			Destroy(player.HeldObject.gameObject);
			GetComponentInParent<customerAI>()?.CompleteOrder();
			Destroy(gameObject);
		}
    }
}
