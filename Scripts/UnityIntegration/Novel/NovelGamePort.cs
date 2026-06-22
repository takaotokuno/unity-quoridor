using System;
using System.Collections;
using UnityEngine;
using Utage;

namespace Quoridor
{
    public sealed class NovelGamePort : MonoBehaviour, INovelGamePort
    {
        [SerializeField] private AdvEngine advEngine;
        [SerializeField] private ScenarioCatalog catalog;

        public bool IsPlaying { get; private set; }

        // 指定IDのシナリオを再生する
        public void JumpScenario(ScenarioId scenarioId)
        {
            string label = catalog.FindScenario(scenarioId);
            JumpScenario(label, null);
        }

        // 指定IDのシナリオを再生する
        // 終了した時に onComplete が呼ばれる
        public void JumpScenario(ScenarioId scenarioId, Action onComplete)
        {
            string label = catalog.FindScenario(scenarioId);
            JumpScenario(label, onComplete);
        }

        private IEnumerator JumpScenarioAsync(string label, Action onComplete)
        {
            IsPlaying = true;
            advEngine.JumpScenario(label);

            while (!advEngine.IsEndOrPauseScenario)
            {
                yield return null;
            }

            IsPlaying = false;
            onComplete?.Invoke();
        }

        // 指定ラベルのシナリオを再生する共通処理
        // ラベルが無効、または未ロードなら Debug.LogErrorFormat を出して終了
        private void JumpScenario(string label, Action onComplete)
        {
            if (string.IsNullOrEmpty(label))
            {
                Debug.LogErrorFormat("シナリオラベルが空です");
                return;
            }

            if (label[0] == '*')
            {
                label = label.Substring(1);
            }

            if (advEngine.DataManager.FindScenarioData(label) == null)
            {
                Debug.LogErrorFormat("{0} はまだロードされていないか、存在しないシナリオです", label);
                return;
            }

            StartCoroutine(JumpScenarioAsync(label, onComplete));
        }
    }
}
