using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PointHelperScript : NetworkBehaviour
{
    [SyncVar]
    public Color color;
    SpriteRenderer _sr;
    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
