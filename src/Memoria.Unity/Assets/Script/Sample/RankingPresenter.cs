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
        [SerializeField, Tooltip("タイトルID")]
        private string _titleId;

        [SerializeField, Tooltip("登録するプレイヤー名の入力フィールド")]
        private TMP_InputField _fieldName;

        [SerializeField, Tooltip("登録するスコアの入力フィールド")]
        private TMP_InputField _fieldScore;

        [SerializeField, Tooltip("ランキング表示用テキスト")]
        private TextMeshProUGUI _leaderboardText;

        [SerializeField, Tooltip("スコア登録ボタン")]
        private Button _registerButton;

        [SerializeField, Tooltip("ランキング取得ボタン")]
        private Button _getLeaderboardButton;

        private RankingClient _rankingClient;
        private const string StatisticName = "HighScore";
        private string _playerName;
        private int _playerScore;

        private void Start()
        {
            _rankingClient = new RankingClient(_titleId);
            _rankingClient.LoginAsync(destroyCancellationToken);

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
            try
            {
                await _rankingClient.SendAsync(_playerName, _playerScore, StatisticName, cancellationToken);
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
                var response = await _rankingClient.LoadAsync(StatisticName, 5, cancellationToken);
                var sb = new StringBuilder();
                for (int i = 0; i < response.Length; i++)
                {
                    sb.AppendLine($"{i + 1}. {response[i].playerName} : {response[i].score}");
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