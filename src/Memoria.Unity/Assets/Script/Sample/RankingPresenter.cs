using System;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Memoria.Sample
{
    /// <summary>
    /// ランキング登録と取得のプレゼンター.
    /// </summary>
    public class RankingPresenter : MonoBehaviour
    {
        /// <summary>
        /// タイトルID.
        /// </summary>
        [SerializeField]
        private string _titleId;

        /// <summary>
        /// 登録するプレイヤー名の入力フィールド.
        /// </summary>
        [SerializeField]
        private TMP_InputField _fieldName;

        /// <summary>
        /// 登録するスコアの入力フィールド.
        /// </summary>
        [SerializeField]
        private TMP_InputField _fieldScore;

        /// <summary>
        /// ランキング表示用テキスト.
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _leaderboardText;

        /// <summary>
        /// スコア登録ボタン.
        /// </summary>
        [SerializeField]
        private Button _registerButton;

        /// <summary>
        /// ランキング取得ボタン.
        /// </summary>
        [SerializeField]
        private Button _getLeaderboardButton;

        private const string StatisticName = "HighScore";
        private string _playerName;
        private int _playerScore;

        private void Start()
        {
            _fieldName.onEndEdit.AddListener(text =>
            {
                _playerName = text;
            });

            _fieldScore.onEndEdit.AddListener(text =>
            {
                if (int.TryParse(text, out var score))
                {
                    _playerScore = score;
                }
            });

            _registerButton.onClick.AddListener(() =>
            {
                _ = RegisterScoreAsync(destroyCancellationToken);
            });

            _getLeaderboardButton.onClick.AddListener(() =>
            {
                _ = GetLeaderboardAsync(destroyCancellationToken);
            });
        }

        /// <summary>
        /// スコアをランキングに登録する.
        /// </summary>
        /// <param name="cancellationToken"></param>
        private async Awaitable RegisterScoreAsync(CancellationToken cancellationToken)
        {
            var data = new RankingData
            {
                PlayerName = _playerName,
                Score = _playerScore
            };
            try
            {
                await RankingClient.SendAsync(_titleId, data, cancellationToken);
                Debug.Log("Score registered successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to register score: " + ex.Message);
            }
        }

        /// <summary>
        /// ランキングデータを取得して表示する.
        /// </summary>
        /// <param name="cancellationToken"></param>
        private async Awaitable GetLeaderboardAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await RankingClient.LoadAsync(_titleId, StatisticName, maxResultsCount: 5, cancellationToken);
                var sb = new StringBuilder();
                for (int i = 0; i < response.Length; i++)
                {
                    sb.AppendLine($"{i + 1}. {response[i].PlayerName} : {response[i].Score}");
                }

                _leaderboardText.text = sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to get leaderboard: " + ex.Message);
            }
        }
    }
}