using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GASRankingSystem.Unity
{
    /// <summary>
    /// GASを用いたランキングシステムを管理するシングルトンクラス
    /// 取得はGetScoreListを呼び出し、送信はSendScoreを呼び出す
    /// </summary>
    public class GASRankingManager : MonoBehaviour
    {
        [SerializeField] private string _deployId;
        [SerializeField] private int _maxUserInputNameCount = 10;
        private static GASRankingManager _instance;
        public static GASRankingManager Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject); // 必要に応じて
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// スコアを送信する
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="score"></param>
        /// <param name="success"></param>
        /// <param name="fail"></param>
        public void SendScore(string userName, float score, Action<string> success, Action fail)
        {
            if (userName.Length > _maxUserInputNameCount)
            {
                Debug.LogError("ユーザー名は" + _maxUserInputNameCount + "文字以下で入力してください");
                fail.Invoke();
                return;
            }
            StartCoroutine(PostRequest(CreateData(userName, score), success, fail));
        }
        
        /// <summary>
        /// スコアを取得する
        /// </summary>
        /// <param name="order">降順か昇順か</param>
        /// <param name="onSuccess"></param>
        /// <param name="onFailure"></param>
        public void GetScoreList(GetScoreOrder order, Action<List<PlayerData>> onSuccess, Action onFailure)
        {
            StartCoroutine(GetScoreListCoroutine(order, onSuccess, onFailure));
        }

        #region internal
        private string CreateData(string userName, float score)
        {
            var playerData = new PlayerData
            {
                Name = userName,
                Score = score
            };
            return JsonUtility.ToJson(playerData);
        }
        
        private IEnumerator PostRequest(string json, Action<string> success, Action fail)
        {
            var url = $"https://script.google.com/macros/s/{_deployId}/exec";
            var request = new UnityWebRequest(url, "POST");
            var postData = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                success.Invoke(request.downloadHandler.text);
            }
            else
            {
                fail.Invoke();
            }
        }


        private IEnumerator GetScoreListCoroutine(GetScoreOrder order, Action<List<PlayerData>> onSuccess, Action onFailure)
        {
            string jsonResponse = null;
            var isSuccess = false;

            yield return GetScoreListFromGAS(
                json => 
                {
                    jsonResponse = json;
                    isSuccess = true;
                },
                () =>
                {
                    isSuccess = false;
                }
            );

            if (isSuccess && !string.IsNullOrEmpty(jsonResponse))
            {
                var playerDataList = ParseJsonToPlayerDataList(jsonResponse);

                if (order == GetScoreOrder.Ascending)
                {
                    playerDataList.Sort((x, y) => x.Score.CompareTo(y.Score));
                }

                onSuccess?.Invoke(playerDataList);
            }
            else
            {
                onFailure?.Invoke();
            }
        }

        private IEnumerator GetScoreListFromGAS(Action<string> success, Action fail)
        {
            var url = $"https://script.google.com/macros/s/{_deployId}/exec";
            var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                success.Invoke(request.downloadHandler.text);
            }
            else
            {
                fail.Invoke();
            }
        }

        private List<PlayerData> ParseJsonToPlayerDataList(string jsonResponse)
        {
            var playerArray = JsonUtility.FromJson<PlayerDataArrayWrapper>(jsonResponse).PlayerDataArray;
            return new List<PlayerData>(playerArray);
        }

        public enum GetScoreOrder
        {
            Ascending,
            Descending
        }
        
        #endregion
    }

    [Serializable]
    public class PlayerData
    {
        public string Name;
        public float Score;
    }

    [Serializable]
    public class PlayerDataArrayWrapper
    {
        public PlayerData[] PlayerDataArray;
    }
}
