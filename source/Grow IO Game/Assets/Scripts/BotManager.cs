using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public GameObject botPrefab;
    public GameHelperScript _gameHelper;
    public List<BotScript> bots = new List<BotScript>();
    public List<PlayerHelperScript> players = new List<PlayerHelperScript>();

    public static BotManager instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        for(int i = 0; i < 20; i++)
        {
            CreateBot(i);
        }
    }

    public void CreateBot(int i)
    {
        GameObject bot = Instantiate(botPrefab);
        bot.transform.position = new Vector2(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f));
        bots.Add(bot.GetComponent<BotScript>());
        bots[i].botManager = this;
        bots[i].index = i;
    }
}
