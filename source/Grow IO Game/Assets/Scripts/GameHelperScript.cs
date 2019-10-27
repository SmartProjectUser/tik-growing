using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameHelperScript : NetworkBehaviour
{
    public Vector2 MapSize = new Vector2(300, 300);
    private PlayerHelperScript _playerHelper;
    Vector2 vecScale;
    private float delta;
    private float mass;
    public GameObject pointPrefab;
    private float quotient;
    public Sprite bigWeightS, lowWeightS, freezeS, poisonS;
    //public GameObject[] boosts; //альтернатива для цветов
    public Color[] colorPallete;
    public PlayerHelperScript CurrentPlayer
    {
        get { return _playerHelper; }
        set { _playerHelper = value; }
    }

    
    [Server]
    // Use this for initialization
    IEnumerator Start()
    {
        Debug.Log("GameHelper Start ()");
        yield return new WaitForSeconds(1);
        delta = 8 * Mathf.Pow(20, -Mathf.Log(2, 0.1f)) * Mathf.Pow(mass, Mathf.Log(2, 0.1f));
        for (int i = 0; i < 1500; i++)
        {            
            CreatePoint(Color.green);
        }
    }

    [Server]
    public void CreatePoint(Color color)
    {                        
        GameObject point = Instantiate(pointPrefab);       
        GivePointPos(point);
        NetworkServer.Spawn(point);
    }

    [Server]
    public void GivePointPos(GameObject point)
    {
        int rand = Random.Range(0, 50);
        point.GetComponent<PointHelperScript>().color = Color.white;
        if (rand == 0)
        {
            point.tag = "BigWeight";
            point.GetComponent<SpriteRenderer>().sprite = bigWeightS;
            //point.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else if(rand == 1)
        {
            point.tag = "LowWeight";
            point.GetComponent<SpriteRenderer>().sprite = lowWeightS;
            //point.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if(rand == 2)
        {
            point.tag = "Poison";
            point.GetComponent<SpriteRenderer>().sprite = poisonS;
            //point.GetComponent<SpriteRenderer>().color = Color.black;
        }
        else if(rand == 4)
        {
            point.tag = "Freeze";
            point.GetComponent<SpriteRenderer>().sprite = freezeS;
            //point.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            point.GetComponent<SpriteRenderer>().color = colorPallete[Random.Range(0, colorPallete.Length)];
            point.GetComponent<PointHelperScript>().color = point.GetComponent<SpriteRenderer>().color;
        }
        Vector2 randV2 = Random.insideUnitCircle;
        point.transform.position = randV2 * Random.Range(75, MapSize.x);
    }

    // Update is called once per frame
    void Update()
    {
        mass -= 0.000000000002f * mass * mass;
    }
}