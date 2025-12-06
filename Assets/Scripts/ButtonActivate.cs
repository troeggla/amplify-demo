using Redis;
using StackExchange.Redis;
using UnityEngine;

public class ButtonActivate : MonoBehaviour
{
    public float maxHeight = 0.75f;
    public int numButtons = 3;

    private Vector3 _currentPosition;
    private float _initialY;

    private void Start()
    {
        _currentPosition = gameObject.transform.position;
        _initialY = _currentPosition.y;

        Debug.Log("Installing handlers for keyboard");

        RedisManager.Instance.SubscribeToChannel<ButtonMessage>(RedisChannel.Literal("amplify.keyboard"), (_, value) =>
        {
            var activeButton = (value.Button == -1) ? 0 : value.Button;
            var height = (activeButton / (float)numButtons) * maxHeight;

            Debug.Log($"Button pressed: {value.Button} {activeButton} {height}");

            _currentPosition.y = height + _initialY;
        });
    }

    private void Update()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, _currentPosition, Time.deltaTime * 10);
    }
}
