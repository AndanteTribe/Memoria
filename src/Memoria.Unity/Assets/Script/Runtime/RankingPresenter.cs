using System;
using System.Text;
using System.Threading;
using Memoria;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Runtime
{
    /// <summary>
    /// ランキング登録と取得のプレゼンター.
    /// </summary>
    public class RankingPresenter : MonoBehaviour
    {
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
        /// タイトルIDの入力フィールド.
        /// </summary>
        [SerializeField]
        private TMP_InputField _fieldTitleId;

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

        private const string StaticsName = "HighScore";
        private RankingRegister _register;
        private string _playerName;
        private string _titleId;
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

            _fieldTitleId.onEndEdit.AddListener(text =>
            {
                _titleId = text;
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
            _register = new RankingRegister(_titleId);
            var data = new RankingData<int>
            {
                PlayerName = _playerName,
                Score = _playerScore
            };
            try
            {
                await _register.SendAsync(data, cancellationToken);
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
            _register = new RankingRegister(_titleId);
            try
            {
                var response = await _register.LoadAsync<int>(StaticsName, maxResultsCount: 5, cancellationToken);
                var sb = new StringBuilder();
                for (int i = 0; i < response.AsSpan().Length; i++)
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