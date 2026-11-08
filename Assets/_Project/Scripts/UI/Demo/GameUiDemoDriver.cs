using Game.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UI.Demo
{
    /// <summary>
    /// 4.E.3 UI 示範場景的假 GameManager：提供計時與勝敗，並用按鍵模擬救援流程。
    ///   E = 對最近的 NPC 推進狀態（Waiting/Wandering → Following → Arrived）
    ///   K = 把最近的 NPC 切成 Wandering（模擬小孩跑掉）
    ///   Enter = 重來
    /// </summary>
    public class GameUiDemoDriver : MonoBehaviour, IGameStatus
    {
        [SerializeField] private float _timeLimit = 90f;
        [SerializeField] private float _interactRange = 6f;

        public int CurrentLevel => 1;
        public int CurrentFloor => -1;
        public float TimeLeft { get; private set; }
        public float TimeLimit => _timeLimit;
        public bool IsPlaying { get; private set; }

        private void Start() => ResetGame();

        private void ResetGame()
        {
            TimeLeft = _timeLimit;
            IsPlaying = true;
            foreach (var npc in FindObjectsByType<DemoRescueNpc>(FindObjectsSortMode.None))
                npc.State = RescueState.Waiting;
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (!IsPlaying)
            {
                if (kb.enterKey.wasPressedThisFrame) ResetGame();
                return;
            }

            TimeLeft -= Time.deltaTime;
            if (TimeLeft <= 0f)
            {
                TimeLeft = 0f;
                IsPlaying = false;
                GameEvents.RaiseGameLost();
                return;
            }

            if (kb.eKey.wasPressedThisFrame) Advance(Nearest());
            if (kb.kKey.wasPressedThisFrame)
            {
                var npc = Nearest();
                if (npc != null && npc.State != RescueState.Arrived) npc.State = RescueState.Wandering;
            }
        }

        private void Advance(DemoRescueNpc npc)
        {
            if (npc == null) return;
            if (npc.State == RescueState.Waiting || npc.State == RescueState.Wandering)
            {
                npc.State = RescueState.Following;
                GameEvents.RaiseRescueStarted(npc);
            }
            else if (npc.State == RescueState.Following)
            {
                npc.State = RescueState.Arrived;
                GameEvents.RaiseRescueCompleted(npc);
                CheckAllDone();
            }
        }

        private void CheckAllDone()
        {
            foreach (var npc in FindObjectsByType<DemoRescueNpc>(FindObjectsSortMode.None))
                if (npc.State != RescueState.Arrived) return;
            IsPlaying = false;
            GameEvents.RaiseGameWon();
        }

        private DemoRescueNpc Nearest()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return null;
            DemoRescueNpc best = null;
            float bestSqr = _interactRange * _interactRange;
            foreach (var npc in FindObjectsByType<DemoRescueNpc>(FindObjectsSortMode.None))
            {
                float d = (npc.transform.position - player.transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = npc; }
            }
            return best;
        }
    }
}
