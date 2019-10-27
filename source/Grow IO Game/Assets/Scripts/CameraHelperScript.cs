using UnityEngine;
using System.Collections;

public class CameraHelperScript : MonoBehaviour
{

    // Use this for initialization
    GameHelperScript _gameHelper;
    Transform _player;

    IEnumerator Start()
    {
        while (_gameHelper == null)
        {
            _gameHelper = GameObject.FindObjectOfType<GameHelperScript>();
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameHelper == null ||
            (_gameHelper != null && _gameHelper.CurrentPlayer == null))
            return;

        _player = _gameHelper.CurrentPlayer.transform;
        Vector3 newPosition = new Vector3(_player.position.x, _player.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.deltaTime * 12);
    }
}