using UnityEngine;

public class windController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private WindZone windZone;

    [Header("Wind Settings")] 
    public float baseStrength = 0.5f;
    public float gustStrength = 2.0f;
    public float gustDuration = 2.0f;
    public float timeBetweenWinds = 5.0f;

    private float gustTimer;
    private bool isGusting;
    
    void Start()
    {
        windZone = GetComponent<WindZone>();
        windZone.windMain = baseStrength;
        gustTimer = timeBetweenWinds;
    }

    // Update is called once per frame
    void Update()
    {
        gustTimer -= Time.deltaTime;
        if (!isGusting && gustTimer <= 0f)
        {
            isGusting = true;
            windZone.windMain = gustStrength;
            gustTimer = gustDuration;
        }
        else if (isGusting && gustTimer <= 0f)
        {
            isGusting = false;
            windZone.windMain = baseStrength;
            gustTimer = timeBetweenWinds;
        }
    }
}
