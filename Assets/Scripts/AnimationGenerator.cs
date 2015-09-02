////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - AnimationGenerator.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationGenerator : MonoBehaviour 
{
    /// <summary>
    /// Game objects passed in
    /// </summary>
    public GameObject explosionAnimation;
    public GameObject hitAnimation;
    public GameObject splashAnimation;

    /// <summary>
    /// Available animations to play 
    /// </summary>
    public enum ID
    {
        EXPLOSION,
        HIT,
        SPLASH,
        MAX_ANIMATIONS
    };
    
    /// <summary>
    /// Information for an animation
    /// </summary>
    class AnimationData
    {
        public SoundManager.SoundID effect;   // Sound effect to play when it starts
        public List<GameObject> instances;    // Container of all animation instances
    };

    private List<AnimationData> m_animations; // Container of all animations
    private SoundManager m_soundManager;      // Allows playing of sound effects

    /// <summary>
    /// Initialises the animation manager
    /// </summary>
    void Start () 
    {
        m_animations = new List<AnimationData>();

        for(int i = 0; i < (int)ID.MAX_ANIMATIONS; ++i)
        {
            m_animations.Add(new AnimationData());
        }

        CreateAnimations(5, ID.EXPLOSION, SoundManager.SoundID.EXPLODE, explosionAnimation, "Explosion");
        CreateAnimations(20, ID.HIT, SoundManager.SoundID.HIT, hitAnimation, "Hit");
        CreateAnimations(20, ID.SPLASH, SoundManager.SoundID.SPLASH, splashAnimation, "Splash");
    }

    /// <summary>
    /// Creates the instances for an animation
    /// </summary>
    void CreateAnimations(int count, 
                          ID id,
                          SoundManager.SoundID effectID,
                          GameObject animation, 
                          string name)
    {
        int index = (int)id;
        m_animations[index].instances = new List<GameObject>();

        for(int i = 0; i < count; ++i)
        {
            m_animations[index].effect = effectID;
            m_animations[index].instances.Add (
                i == 0 ? animation : (GameObject)(Instantiate(animation)));
            
            m_animations[index].instances[i].name = name + i.ToString();
            m_animations[index].instances[i].transform.parent = this.transform;
            m_animations[index].instances[i].SetActive(false);
            m_animations[index].instances[i].GetComponent<Animator>().enabled = false;
        }
    }

    /// <summary>
    /// Updates the animations that are playing
    /// </summary>
    void UpdateAnimations(List<GameObject> animations)
    {
        for(int i = 0; i < animations.Count; ++i)
        {
            if(animations[i].activeSelf)
            {
                var animator = animations[i].GetComponent<Animator>();
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Exit"))
                {
                    animations[i].SetActive(false);
                    animator.CrossFade("Base Layer.Entry", 0.0f);
                    animator.enabled = false;
                }
            }
        }    
    }

    /// <summary>
    /// Updates the animations that are playing
    /// </summary>
    void Update () 
    {
        foreach(var animation in m_animations)
        {
            UpdateAnimations(animation.instances);
        }
    }

    /// <summary>
    /// Plays an animation at the given position
    /// @note y position not used
    /// </summary>    
    public void PlayAnimation(Vector3 position, ID id)
    {
        if(PlayerPlacer.IsCloseToPlayer(position) && StartAnimation(position, id))
        {
            SoundManager.Get().PlaySound(m_animations[(int)id].effect);
        }
    }

    /// <summary>
    /// Starts an animation playing if possible
    /// * @note y position not used
    /// </summary>   
    bool StartAnimation(Vector3 position, ID id)
    {
        int index = (int)id;
        var instances = m_animations[index].instances;

        for(int i = 0; i < instances.Count; ++i)
        {
            if(!instances[i].activeSelf)
            {
                instances[i].SetActive(true);
                instances[i].transform.position = position;
                instances[i].GetComponent<Animator>().enabled = true;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the AnimationGenerator from the scene
    /// </summary>
    public static AnimationGenerator Get()
    {
        var obj = FindObjectOfType<AnimationGenerator>();
        if (!obj)
        {
            Debug.LogError("AnimationGenerator could not be found in scene");
        }
        return obj;
    }
}
