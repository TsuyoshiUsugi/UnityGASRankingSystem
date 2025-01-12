using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GASRankingSystem.Unity
{
    /// <summary>
    /// Singleton class to manage ranking system using GAS
    /// Call GetScoreList to retrieve rankings and SendScore to send scores
    /// </summary>
    public class GASRankingManager : MonoBehaviour
    {
        [SerializeField] private string _deployURL;
        [SerializeField] private int _maxUserInputNameCount = 10;
        private static GASRankingManager _instance;
        public static GASRankingManager Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Submit Score
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="score"></param>
        /// <param name="success"></param>
        /// <param name="fail"></param>
        public void SendScore(string userName, float score)
        {
            var rankingData = new PlayerData() { Name = userName, Score = score };
            var serializedData = JsonUtility.ToJson(rankingData);
            var requestPayload = new GASRequest
            {
                Action = "addRanking",
                Data = serializedData
            };
            var jsonRequest = JsonUtility.ToJson(requestPayload);
            StartCoroutine(SendRequest(jsonRequest));
        }
        
        /// <summary>
        /// Retrieve ranking information
        /// </summary>
        /// <param name="order"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFailure"></param>
        public void GetScoreList(GetScoreOrder order, Action<List<PlayerData>> onSuccess, Action onFailure)
        {
            StartCoroutine(GetScoreListCoroutine(order, onSuccess, onFailure));
        }

#if UNITY_EDITOR
        /// <summary>
        /// Delete all ranking data
        /// </summary>
        [ContextMenu("Clear Ranking Data")]
        public void ClearRankingData()
        {
            StartCoroutine(SendDeleteRequest());
        }
#endif

        #region internal
        private IEnumerator SendDeleteRequest()
        {
            var jsonData = JsonUtility.ToJson(new GASRequest { Action = "deleteRanking" });
            var request = new UnityWebRequest(_deployURL, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
        
        private IEnumerator SendRequest(string jsonRequest)
        {
            var request = new UnityWebRequest(_deployURL, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Error: {request.error}, Response: {request.downloadHandler.text}");
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
            var request = UnityWebRequest.Get(_deployURL);
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
    
    [Serializable]
    public class GASRequest
    {
        public string Action; 
        public string Data;   
    }
}
