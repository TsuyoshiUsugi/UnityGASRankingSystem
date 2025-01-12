using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace GASRankingSystem.Unity.Sample
{
    public class Sample : MonoBehaviour
    {
        [SerializeField] private Button _dataButton;
        [SerializeField] private InputField _nameInputField;
        [SerializeField] private InputField _scoreField;
        [SerializeField] private Button _getButton;
        [SerializeField] private GameObject _scoreListParent;
        [SerializeField] private GameObject _scoreListPrefab;

        private void Start()
        {
            _dataButton.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(_nameInputField.text) || string.IsNullOrEmpty(_scoreField.text))
                {
                    Debug.LogError("Enter name and score");
                    return;
                }

                if (!float.TryParse(_scoreField.text, out var score))
                {
                    Debug.LogError("Enter a number in score");
                    return;
                }

                GASRankingManager.Instance.SendScore(_nameInputField.text, score);
            });

            _getButton.onClick.AddListener(() =>
            {
                GASRankingManager.Instance.GetScoreList(GASRankingManager.GetScoreOrder.Descending, list =>
                {
                    foreach (Transform child in _scoreListParent.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    foreach (var playerData in list)
                    {
                        var obj = Instantiate(_scoreListPrefab, _scoreListParent.transform);
                        obj.GetComponentsInChildren<Text>()[0].text = playerData.Name;
                        obj.GetComponentsInChildren<Text>()[1].text =
                            playerData.Score.ToString(CultureInfo.CurrentCulture);
                    }
                }, () => { Debug.LogError("Fail"); });
            });
        }
    }
}