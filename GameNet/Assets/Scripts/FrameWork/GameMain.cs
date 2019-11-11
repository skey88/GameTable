using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Stars
{
    public class GameMain : Main
    {
        protected override void Awake()
        {
            base.Awake();
             
            StartCoroutine(ReqGameSvr("id")); 
        }


        IEnumerator<int> getRes()
        {
            yield return 1;

        }


        IEnumerator<UnityWebRequest> ReqGameSvr(string id)
        {
            string url = "http://test.sporger.com:9040/login/platform?id=" + id;
            TyLogger.Log("url=" + url);
            UnityWebRequest www = new UnityWebRequest(url);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                TyLogger.Log(www.error);
            }
            else
            {
                TyLogger.Log(www.downloadHandler.text);
                //HttpLoginCallbackData httpLoginCallbackData = LitJson.JsonMapper.ToObject<HttpLoginCallbackData>(getData.text);
            }
            yield return null;
        }
    }

}