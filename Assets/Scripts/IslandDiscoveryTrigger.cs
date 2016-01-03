////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - IslandDiscoveryTrigger.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandDiscoveryTrigger : MonoBehaviour
{
    public float                        onCaptureScore = 20.0f;
    public float                        scorePerTime = 1.0f;
    public float                        timePassedForScore = 2.0f;
    private float                       m_scoreToAdd = 0.0f;
    public UnityEngine.UI.Image         tickImage = null;
    public UnityEngine.UI.Text          ownerText = null;
    public UnityEngine.UI.Text          scoreText = null;
    public float                        m_scaleSpeed = 1.0f;
    public float                        m_fadeSpeed = 4.0f;
    public float                        m_minScoreSize = 1.0f;
    public float                        m_scaleToFade = 1.5f;
    private UnityEngine.UI.Outline      m_scoreOutline = null;
    private RectTransform               m_scoreTransform = null;
    private Vector3                     m_scoreScale;
    private float                       m_timePassed = 0.0f;
    private Canvas                      m_canvas = null;
    private IslandDiscoveryNode[]       m_nodes;
    private GameObject                  m_owner = null;
    private List<SpriteRenderer>        m_islands = new List<SpriteRenderer>();
    private GameObject                  m_patrolAI = null;
    private PatrolAI                    m_ai = null;
    private static Dictionary<int, int> sm_islandsOwned = new Dictionary<int, int>();
    private bool                        m_aiSpawned = false;
    private bool                        m_aiInitialised = false;
    private bool                        m_ownerWithinRange = false;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        sm_islandsOwned.Clear();

        m_scoreOutline = scoreText.GetComponent<UnityEngine.UI.Outline>();
        m_scoreTransform = scoreText.GetComponent<RectTransform>();
        scoreText.gameObject.SetActive(false);

        m_nodes = transform.parent.GetComponentsInChildren<IslandDiscoveryNode>();
        foreach(var node in m_nodes)
        {
            node.SetTrigger(this);
        }

        var islands = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        foreach(var island in islands)
        {
            if(island.CompareTag("Island"))
            {
                m_islands.Add(island);
            }
        }

        if(m_islands.Count == 0)
        {
            Debug.LogError("No associated island sprite");
        }
        if(m_nodes.Length == 0)
        {
            Debug.LogError("No associated nodes");
        }

        m_canvas = GetComponent<Canvas>();
        m_canvas.enabled = false;
    }

    /// <summary>
    /// Checks whether the island has been discovered
    /// </summary>
    void Update()
    {
        //if (Utilities.GetNetworking().IsConnectedToLevel() && Utilities.IsLevelLoaded() && !Utilities.IsGameOver() && !m_aiInitialised)
        //{
        //    //TODO: Disabled atm. Work in progress.
        //    if (PhotonNetwork.isMasterClient)
        //    {
        //        m_patrolAI = PhotonNetwork.InstantiateSceneObject("PatrolAIPhotonView", transform.position, Quaternion.identity, 0, null);
        //        m_aiInitialised = true;
        //        m_ai = m_patrolAI.GetComponentInChildren<PatrolAI>();

        //    }
        //    else
        //    {
        //        //Find Patrol AI
        //        var patrolShips = PhotonNetwork.FindGameObjectsWithComponent(typeof(PatrolAI));

        //    }
        //}

        UpdateIslandOwner();
        UpdateIslandScore();
    }

    /// <summary>
    /// Checks and updates who owns the island
    /// </summary>
    void UpdateIslandOwner()
    {
        GameObject owner = m_nodes[0].Owner;
        for (int i = 1; i < m_nodes.Length; ++i)
        {
            if (owner == null ||
               m_nodes[i].Owner == null ||
               owner.name != m_nodes[i].Owner.name)
            {
                // Island was captured but is no longer
                owner = null;
                break;
            }
        }

        if (owner == null)
        {
            if (IsDiscovered())
            {
                Debug.Log("Island no longer owned");
                SetCaptured(null);
            }
        }
        else if (m_owner == null || m_owner.name != owner.name)
        {
            Debug.Log("Setting new owner of island: " + owner.name);
            SetCaptured(owner);
        }
    }

    /// <summary>
    /// Updates the score per tick and floating score text
    /// </summary>
    void UpdateIslandScore()
    {
        if (m_owner != null && PlayerManager.IsControllablePlayer(m_owner))
        {
            m_timePassed += Time.deltaTime;
            if (m_timePassed >= timePassedForScore)
            {
                m_scoreToAdd += scorePerTime;
                m_timePassed = 0.0f;

                if ((int)m_scoreToAdd > 0)
                {
                    ShowScore(m_scoreToAdd);
                    m_owner.GetComponent<PlayerScore>().AddScore(m_scoreToAdd);
                    m_scoreToAdd = 0.0f;
                }
            }
        }

        if (scoreText.gameObject.activeSelf)
        {
            m_scoreScale.x += Time.deltaTime * m_scaleSpeed;
            m_scoreScale.y = m_scoreScale.x;
            m_scoreScale.z = m_scoreScale.x;
            m_scoreTransform.localScale = m_scoreScale;

            if (m_scoreScale.x > m_scaleToFade)
            {
                float alpha = scoreText.color.a - (Time.deltaTime * m_fadeSpeed);
                alpha = Mathf.Min(Mathf.Max(0.0f, alpha), 1.0f);
                m_scoreOutline.effectColor = new Color(0.0f, 0.0f, 0.0f, alpha);

                scoreText.color = new Color(
                    scoreText.color.r,
                    scoreText.color.g,
                    scoreText.color.b,
                    alpha);

                if (alpha == 0.0f)
                {
                    scoreText.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Shows the score text score
    /// </summary>
    void ShowScore(float score)
    {
        scoreText.gameObject.SetActive(true);
        m_scoreScale.x = m_minScoreSize;
        m_scoreScale.y = m_minScoreSize;
        m_scoreScale.z = m_minScoreSize;
        m_scoreTransform.localScale = m_scoreScale;
        scoreText.text = "+" + ((int)score).ToString();
        m_scoreOutline.effectColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        scoreText.color = new Color(
            scoreText.color.r, 
            scoreText.color.g, 
            scoreText.color.b, 
            1.0f);
    }

    /// <summary>
    /// Sets whether the island is captured
    /// </summary>
    void SetCaptured(GameObject owner)
    {
        m_canvas.enabled = owner != null;

        if(owner != null)
        {
            tickImage.color = Utilities.GetPlayerColor(owner);
            ownerText.text = Utilities.GetPlayerName(owner);
            ShowScore(onCaptureScore);
            scoreText.color = new Color(
                tickImage.color.r,
                tickImage.color.g,
                tickImage.color.b,
                1.0f);

            if (PlayerManager.IsCloseToPlayer(owner.transform.position, 30.0f))
            {
                SoundManager.Get().PlaySound(SoundManager.SoundID.ISLAND_FIND);
            }

            var player = PlayerManager.GetControllablePlayer();
            if(player != null && player.name == owner.name)
            {
                owner.GetComponent<PlayerScore>().AddScore(onCaptureScore);
            }
        }
        else
        {
            tickImage.color = new Color(1.0f, 1.0f, 1.0f);
            ownerText.text = "";
        }

        foreach(var island in m_islands)
        {
            island.color = tickImage.color;
        }

        if (m_owner != null)
        {
            int ownerID = Utilities.GetPlayerID(m_owner);
            if(sm_islandsOwned.ContainsKey(ownerID))
            {
                sm_islandsOwned[ownerID]--;
            }
        }
        if (owner != null)
        {
            int ownerID = Utilities.GetPlayerID(owner);
            if (sm_islandsOwned.ContainsKey(ownerID))
            {
                sm_islandsOwned[ownerID]++;
            }
            else
            {
                sm_islandsOwned[ownerID] = 1;
            }
        }

        m_scoreToAdd = 0.0f;
        m_timePassed = 0.0f;
        m_owner = owner;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (m_owner != null)
            {
                int ownerPlayerID = Utilities.GetPlayerID(m_owner);
                NetworkedPlayer nearbyPlayer = other.gameObject.GetComponent<NetworkedPlayer>();
                if (ownerPlayerID == nearbyPlayer.PlayerID)
                {
                    m_ownerWithinRange = true;
                    nearbyPlayer.IslandWithinRange = this.gameObject;
                    Debug.Log("Owner entered island range.");
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (m_owner != null)
            {
                //Makes sure that after island has been captured m_ownerWithinRange is set to true.
                if (!m_ownerWithinRange)
                {
                    int ownerPlayerID = Utilities.GetPlayerID(m_owner);
                    NetworkedPlayer nearbyPlayer = other.gameObject.GetComponent<NetworkedPlayer>();
                    if (ownerPlayerID == nearbyPlayer.PlayerID)
                    {
                        m_ownerWithinRange = true;
                        nearbyPlayer.IslandWithinRange = this.gameObject;
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (m_owner != null)
            {
                int ownerPlayerID = Utilities.GetPlayerID(m_owner);
                NetworkedPlayer nearbyPlayer = other.gameObject.GetComponent<NetworkedPlayer>();
                if (ownerPlayerID == nearbyPlayer.PlayerID)
                {
                    m_ownerWithinRange = false;
                    nearbyPlayer.IslandWithinRange = null;
                    Debug.Log("Owner exited island range.");
                }
            }
        }
    }

    /// <summary>
    /// Returns whether this island has been discovered
    /// </summary>
    public bool IsDiscovered()
    {
        return m_canvas.enabled;
    }

    /// <summary>
    /// Returns the owner of this island
    /// </summary>
    public GameObject GetOwner()
    {
        return m_owner;
    }

    /// <summary>
    /// Whether the current owner of the island is within range.
    /// </summary>
    public bool OwnerWithinRange
    {
        get { return m_ownerWithinRange; }
    }

    public bool AISpawned
    {
        get { return m_aiSpawned; }
    }

    /// <summary>
    /// Gets a list of what player owns what islands
    /// </summary>
    static public Dictionary<int, int> GetIslandsOwned()
    {
        return sm_islandsOwned;
    }

    public void SpawnPatrolAI(int playerID)
    {
        
        if (m_patrolAI == null)
        {
            m_patrolAI = PhotonNetwork.Instantiate("PatrolAIPhotonView", Vector3.zero, Quaternion.identity, 0);
        }
        if (m_patrolAI != null)
        {
            m_ai = m_patrolAI.GetComponentInChildren<PatrolAI>();
            m_ai.Initialise(m_nodes, playerID);
            m_aiSpawned = true;
        }
    }
}