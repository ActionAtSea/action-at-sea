using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletManager : MonoBehaviour
{
    // Use this for initialization

    #region variables

    public static BulletManager current;
    public int poolSize = 20;
    public Rigidbody2D bullet;
    List<Rigidbody2D> pool;
    public bool willGrow = false;

    #endregion

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        pool = new List<Rigidbody2D>();

        for (int i = 0; i < poolSize; ++i)
        {
            Rigidbody2D obj = (Rigidbody2D)Instantiate(bullet);
            //obj.
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}