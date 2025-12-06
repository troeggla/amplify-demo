using Redis;
using StackExchange.Redis;
using UnityEngine;

public class VibrationMove : MonoBehaviour
{
    public float maxHeight = .75f;

    private Vector3 _currentPosition;
    private float _initialY;

    private void Start()
    {
        _currentPosition = transform.position;
        _initialY = _currentPosition.y;

        if (RedisManager.Instance.IsConnected) SubscribeToChannel();
        else RedisManager.Instance.OnConnect += SubscribeToChannel;
    }

    private void Update()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, _currentPosition, Time.deltaTime * 10);
    }

    private void OnDestroy() => RedisManager.Instance.OnConnect -= SubscribeToChannel;

    private void SubscribeToChannel()
    {
        Debug.Log("Installing handlers for vibration sensor");

        RedisManager.Instance.SubscribeToChannel<VibrationMessage>(RedisChannel.Literal("amplify.vibration"), (_, value) =>
        {
            var height = (value.Value / 4096f) * maxHeight;
            _currentPosition.y = height + _initialY;
        });
    }
}
