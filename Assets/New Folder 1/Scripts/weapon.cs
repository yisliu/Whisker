using UnityEngine;
using StarterAssets;
using Unity.Mathematics;

public class weapon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    StarterAssetsInputs inputs;
	[SerializeField] weaponso wps;
	[SerializeField] GameObject hitVFXPrefab;
	[SerializeField] Animator animator;
    [SerializeField] ParticleSystem muzzleFlash;
	[SerializeField] LayerMask enemyLayer;

	const string SHOOT_STRING = "idle";
    void Awake()
    {
        inputs = GetComponentInParent<StarterAssetsInputs>();
    }
    void Update()
    {
        HandleShoot();
    }

    void HandleShoot()
    {


        if (!inputs.shoot) return;

		if(wps==null){
			Debug.Log("1");
		}
		if(hitVFXPrefab==null){
			Debug.Log("2");
		}
		if(animator==null){
			Debug.Log("3");
		}
		if(muzzleFlash==null){
			Debug.Log("4");
		}

        muzzleFlash.Play();
		animator.Play(SHOOT_STRING, 0, 0f);
		inputs.ShootInput(false);
 
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
			GameObject gb = Instantiate(hitVFXPrefab, hit.point, quaternion.identity);
            ParticleSystem pS = gb.GetComponent<ParticleSystem>();
			EnemyHealth enemtHealth = hit.collider.GetComponent<EnemyHealth>();
            enemtHealth?.TakeDamage(wps.Damage);
			if(pS!=null){
				pS.Play();
				Destroy(gb, pS.main.duration + pS.main.startLifetime.constantMax);
			}
        } 
    }
}

