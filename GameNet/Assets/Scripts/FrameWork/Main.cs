using UnityEngine;
using System.Collections;
using System;

namespace Stars
{
    public class Main : MonoBehaviour
    {
        static Main _main = null;
        Game game_ = null;
        public string gameType = "Game";
        protected virtual void Awake()
        {
            if (_main != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            _main = this;
            if (gameType == "" || gameType == "Game")
            {
                game_ = new Game();
            }
            else
            {
                game_ = Activator.CreateInstance(Type.GetType(gameType)) as Game;
            }
            game_.awake();
        }

        protected virtual void Start()
        {
            game_.init();
        }

        void Update()
        {
            game_.update();
        }

        void FixedUpdate()
        {
            game_.fixedUpdate();
        }

        void LateUpdate()
        {
            game_.LateUpdate();
        }

        static public Main Get()
        {
            return _main;
        }

        void processMsg(GameObject obj)
        {
            game_.processMsg(obj);
        }

        private void OnDestroy()
        {
            if (game_ != null)
                game_.Dispose();
        }

        public void OnApplicationPause(bool paused)
        {
            if(game_ != null)
            {
                game_.IsPause = paused;
            }
        }
        /// <summary>
        /// 原生代码调用C#接收函数,目前是异步处理
        /// </summary>
        /// <param name="para">格式：消息类型:参数</param>
        public void ReceiveNativeUnitySendMessage(string para)
        {
            if (game_ != null)
            {
                game_.ReceiveNativeUnitySendMessage(para);
            }
        }
    }

}