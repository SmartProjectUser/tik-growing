using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerHelperScript : NetworkBehaviour
{
    [SyncVar]
    public float Speed = 5;

    [SyncVar]
    public float Size = 1f;

    //CAMERAS SETTINGS
    private Camera cam;
    [SyncVar]
    private float delta;
    private float canSize;
    public bool IsPart { get; set; }
    [SyncVar]
    private int massCoin;
    [SyncVar]
    public float mass;
    private float k = 1;
    GameHelperScript _gameHelper;
    // Use this for initialization
    void Start()
    {
        BotManager.instance.players.Add(this);
        _gameHelper = GameObject.FindObjectOfType<GameHelperScript>();
        cam = GameObject.FindObjectOfType<Camera>();
        massCoin = 10;
        mass = 10;
        canSize = 8;
        delta = 8 * Mathf.Pow(20, -Mathf.Log(2, 0.1f)) * Mathf.Pow(mass, Mathf.Log(2, 0.1f));
        if (!isLocalPlayer)
            return;        
        _gameHelper.CurrentPlayer = this;        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Size, Size, Size);
        if (!isLocalPlayer)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(k == -1)
        transform.position = Vector3.MoveTowards(transform.position, -(Vector3)mousePos + (2 * transform.position),
            Time.deltaTime * Speed);
        else if(k == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, mousePos,
            Time.deltaTime * Speed);
        }

        CheckBounds();

        if(cam.orthographicSize > canSize)
        {
            if(cam.orthographicSize - 1 > canSize)
            {
                cam.orthographicSize = canSize;
            }
            else
            {
                cam.orthographicSize -= 0.001f;
            }
        }
        else
        {
            if(cam.orthographicSize + 1 < canSize)
            {
                cam.orthographicSize = canSize;
            }
            else
            {
                cam.orthographicSize += 0.001f;
            }
        }
    }

    private void CheckBounds()
    {
        if (transform.position.x >= _gameHelper.MapSize.x)
            transform.position = new Vector3(_gameHelper.MapSize.x - 0.01f,
                transform.position.y, 0);

        if (transform.position.y >= _gameHelper.MapSize.y)
            transform.position = new Vector3(transform.position.x,
               _gameHelper.MapSize.y - 0.01f, 0);

        /// Низ
        if (transform.position.x <= -_gameHelper.MapSize.x)
            transform.position = new Vector3(-_gameHelper.MapSize.x + 0.01f,
                transform.position.y, 0);

        if (transform.position.y <= -_gameHelper.MapSize.y)
            transform.position = new Vector3(transform.position.x,
               -_gameHelper.MapSize.y + 0.01f, 0);
    }

    [Server]
    public void ChangeSize(float size)
    {
        Size = size;
        Speed = 0.995f * Speed;

        transform.localScale = new Vector2(mass / 200+ 0.95f, mass / 200+0.95f);
    }

    [ServerCallback]
    void OnTriggerStay2D(Collider2D other)
    {
        Bounds enemy = other.bounds;
        Bounds current = GetComponent<Collider2D>().bounds;

        Vector2 centerEnemy = enemy.center;
        Vector2 centerCurrent = current.center;
        canSize += 0.002f * massCoin;
        if (current.size.x > enemy.size.x &&
           Vector3.Distance(centerCurrent, centerEnemy) < current.size.x)
        {
            if (other.GetComponent<PointHelperScript>())
            {
                ChangeSize(Size + 0.05f);
                //_gameHelper.CreatePoint(Color.red);
            }
            else
                ChangeSize(Size + other.transform.localScale.x);

            other.GetComponent<SpriteRenderer>().color = Color.red;
            _gameHelper.GivePointPos(other.gameObject);            
            //NetworkServer.Destroy(other.gameObject);
        }
        mass += massCoin;
    }
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Freeze")
        {            
            StartCoroutine(AddSpeedWithDelay(Speed - 0.4f, 4));
            Speed = 0.4f;
        }
        else if (collision.tag == "BigWeight")
        {
            Speed /= 2;
            StartCoroutine(SetSpeedWithDelay(2, 7));
        }
        else if (collision.tag == "LowWeight")
        {
            Speed *= 1.45f;
            StartCoroutine(SetSpeedWithDelay(1 / 1.45f, 6));
        }
        else if(collision.tag == "Poison")
        {
            k = -1;
            StartCoroutine(SetKWithDelay(2, 15));
        }                
    }

    [ServerCallback]

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (mass > collision.gameObject.GetComponent<PlayerHelperScript>().mass)
            {
                //mass += collision.gameObject.GetComponent<PlayerHelperScript>().mass;
                ChangeSize(Size + 0.1f);
                NetworkServer.Destroy(collision.gameObject);
            }                
            else
            {
                collision.gameObject.GetComponent<PlayerHelperScript>().ChangeSize(Size + 0.1f);
                //collision.gameObject.GetComponent<PlayerHelperScript>().mass += mass;
                NetworkServer.Destroy(gameObject);
            }                
        }
        else if(collision.gameObject.tag == "Bot")
        {
            if (mass > collision.gameObject.GetComponent<BotScript>().mass)
            {
                ChangeSize(Size + 0.1f);
                //mass += collision.gameObject.GetComponent<BotScript>().mass * 10;
                BotManager.instance.bots.Remove(collision.gameObject.GetComponent<BotScript>());
                NetworkServer.Destroy(collision.gameObject);
            }                
            else
            {
                collision.gameObject.GetComponent<BotScript>().ChangeSize(Size + 0.1f);
                //collision.gameObject.GetComponent<BotScript>().mass += mass;
                NetworkServer.Destroy(gameObject);
            }
        }
    }

    [Server]
    IEnumerator SetMassWithDelay(float value, float delay)
    {
        yield return new WaitForSeconds(delay);
        mass *= value;
    }

    [Server]
    IEnumerator SetKWithDelay(float value, float delay)
    {
        yield return new WaitForSeconds(delay);
        k += value;
    }

    [Server]
    IEnumerator SetSpeedWithDelay(float value, float delay)
    {
        yield return new WaitForSeconds(delay);
        Speed *= value;
    }

    [Server]
    IEnumerator AddSpeedWithDelay(float value, float delay)
    {
        yield return new WaitForSeconds(delay);
        Speed += value;
    }
}