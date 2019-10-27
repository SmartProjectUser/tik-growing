using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BotScript : NetworkBehaviour
{

    [SyncVar]
    public float Speed = 5;

    [SyncVar]
    public float Size = 1f;

    [SyncVar]
    private float delta;
    private float canSize;
    //public bool IsPart { get; set; }
    [SyncVar]
    private int massCoin;
    [SyncVar]
    [HideInInspector]
    public float mass;
    private float k = 1;
    GameHelperScript _gameHelper;
    [SyncVar]
    private Vector2 movePos;

    void Start()
    {
        _gameHelper = GameObject.FindObjectOfType<GameHelperScript>();
        Size = Random.Range(0.1f, 1.5f);
        massCoin = 10;
        mass = 10;
        canSize = 8;
        delta = 8 * Mathf.Pow(20, -Mathf.Log(2, 0.1f)) * Mathf.Pow(mass, Mathf.Log(2, 0.1f));
        ChangeSize(Size);
        InvokeRepeating("CheckForMove", 1.0f, 1.0f);
    }

    void Update()
    {
        transform.localScale = new Vector3(Size, Size, Size);        

        //movePos = Camera.main.ScreenToWorldPoint(movePos);

        if (k == -1)
            transform.position = Vector3.MoveTowards(transform.position, -(Vector3)movePos + (2 * transform.position),
                Time.deltaTime * Speed);
        else if (k == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePos,
            Time.deltaTime * Speed);
        }

        CheckBounds();
    }

    private void CheckBounds()
    {
        if (transform.position.x >= 3000)
            transform.position = new Vector3(_gameHelper.MapSize.x - 0.01f,
                transform.position.y, 0);

        if (transform.position.y >= 3000)
            transform.position = new Vector3(transform.position.x,
               _gameHelper.MapSize.y - 0.01f, 0);

        /// Низ
        if (transform.position.x <= -3000)
            transform.position = new Vector3(-_gameHelper.MapSize.x + 0.01f,
                transform.position.y, 0);

        if (transform.position.y <= -3000)
            transform.position = new Vector3(transform.position.x,
               -_gameHelper.MapSize.y + 0.01f, 0);
    }

    [Server]
    public void ChangeSize(float size)
    {
        Size = size;
        Speed = 0.995f * Speed;

        transform.localScale = new Vector2(mass / 200 + 0.95f, mass / 200 + 0.95f);
    }

















    [SyncVar]
    public int index;

    public BotManager botManager;


    private void CheckForMove()
    {
        /*for (int i = 0; i < botManager.bots.Count; i++)
        {
            if (i == index)
                continue;
            if(mass > botManager.bots[i].mass)
            {
                movePos = botManager.bots[i].transform.position - transform.position;
                return;
            }
        }*/
        movePos = new Vector2(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
    }
}
