using UnityEngine;
using System.Collections;

/*
 * Handles enemy deaths.
 * TODO: Add score when enemies are destroyed.
 */ 
public class EnemyDeath : MonoBehaviour, IKillable
{
    private Health parentHealth;
    private PlayerScore scoreController;
	private AnimationGenerator animationGenerator;

    //TODO: move score values elsewhere.
    private float enemyScoreValue = 100.0f;
    private bool deathCalled = false;   //Used to ensure Death function is only called once.

    // Use this for initialization
    void Start()
    {
        parentHealth = GetComponentInParent<Health>();
        
		animationGenerator = FindObjectOfType<AnimationGenerator> ();
		if(!animationGenerator)
		{
			Debug.Log("ExplosionGenerator could not be found in scene.");
		}
    }

    // Update is called once per frame
    void Update()
    {
		if(scoreController == null)
		{
			scoreController = FindObjectOfType<PlayerScore>();
			return;
		}

        CheckIfAlive();
    }

    public void Death()
    {
		animationGenerator.PlaceExplosion (transform.position);
        scoreController.AddScore(enemyScoreValue);
        Destroy(transform.parent.gameObject);
    }

    private void CheckIfAlive()
    {
        if (!parentHealth.IsAlive && !deathCalled)
        {
            deathCalled = true;
            Death();
        }
    }

}