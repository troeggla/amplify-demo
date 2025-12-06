using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using UnityEngine;

namespace Redis
{
    public class RedisManager : MonoBehaviour
    {
        public static RedisManager Instance { get; private set; }

        private ConnectionMultiplexer _connection;
        private ISubscriber _subscriber;
        public bool IsConnected => _connection is { IsConnected: true };

        public event Action<ConnectionMultiplexer> OnConnect;
        public event Action OnDisconnect;

        public string redisURL;
        public List<string> potatoes;

        private async void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            await ConnectToRedis();
        }

        private async Task<ConnectionMultiplexer> ConnectToRedis()
        {
            _connection = await ConnectionMultiplexer.ConnectAsync(redisURL);
            _subscriber = _connection.GetSubscriber();
            Debug.Log("Connection established");

            OnConnect?.Invoke(_connection);
            return _connection;
        }

        public void SubscribeToChannel(RedisChannel channel, Action<RedisChannel, RedisValue> callback)
        {
            _subscriber?.Subscribe(channel, callback);
        }

        public void SubscribeToChannel<T>(RedisChannel channel, Action<RedisChannel, T> callback)
        {
            _subscriber?.Subscribe(channel, (ch, message) =>
            {
                callback(ch, JsonConvert.DeserializeObject<T>(message));
            });
        }

        public void UnsubscribeFromAllChannels()
        {
            _subscriber?.UnsubscribeAll();
        }

        public void Disconnect()
        {
            if (_connection is null or { IsConnected: false })
            {
                return;
            }

            UnsubscribeFromAllChannels();
            _connection.Close();
            _connection.Dispose();

            OnDisconnect?.Invoke();
        }

        private void OnDestroy()
        {
            Debug.Log("Closing Redis connection");
            Disconnect();
        }
    }
}
