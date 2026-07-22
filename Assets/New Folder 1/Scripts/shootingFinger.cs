using UnityEngine;
using StarterAssets;
using Unity.Mathematics;

public class shootingFinger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //StarterAssetsInputs inputs;
    [SerializeField] weaponso wps;
    [SerializeField] GameObject hitVFXPrefab;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] LayerMask enemyLayer;

    private const string Shoot_Trigger = "Shooting";
    
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleShoot();
            //inputs.ShootInput(false);
        }
    }
    
    void HandleShoot()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();
        if (animator != null)
            animator.SetTrigger(Shoot_Trigger);
        //inputs.ShootInput(false);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, enemyLayer)){
        GameObject gb = Instantiate(hitVFXPrefab, hit.point, quaternion.identity);
        ParticleSystem pS = gb.GetComponent<ParticleSystem>();
        EnemyHealth enemtHealth = hit.collider.GetComponent<EnemyHealth>();
        enemtHealth?.TakeDamage(wps.Damage);
        if (pS != null)
        {
            pS.Play();
            Destroy(gb, pS.main.duration + pS.main.startLifetime.constantMax);
        }
    }
    }

}
