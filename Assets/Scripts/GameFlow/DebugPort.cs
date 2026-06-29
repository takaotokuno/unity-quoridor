using VContainer;
using UnityEngine;
using System.Linq;

namespace Quoridor{
    public class DebugPort : MonoBehaviour
    {
        private MatchSetting _setting;
        private BoardGamePort _boardGamePort;
        [SerializeField] private EnemyMatchPreset matchPreset;
        [SerializeField] private ObjectLayoutView objectLayoutView;

        [Inject] 
        public void Construct(
            BoardGamePort boardGamePort
        )
        {
            _boardGamePort = boardGamePort;            
        }

        private void Start()
        {
            _setting = new MatchSetting
            {
                BoardSize = matchPreset.BoardSize,
                InitPawns = matchPreset.InitPawns.Select(Pawn => Pawn.ToPosition()).ToArray(),
                StartingSide = matchPreset.StartingSide,
                PlayerFirst = new PlayerSetting
                {
                    IsCpu = matchPreset.PlayerControlMode == PlayerControlMode.Cpu,
                    CpuOptions = ToCpuOptions(matchPreset.PlayerCpuAgentPreset),
                    SkillIds = matchPreset.PlayerSkillIds
                        .Select(SkillId.Of)
                        .ToList()
                },
                PlayerSecond = new PlayerSetting
                {
                    IsCpu = matchPreset.EnemyControlMode == PlayerControlMode.Cpu,
                    CpuOptions = ToCpuOptions(matchPreset.EnemyCpuAgentPreset),
                    SkillIds = matchPreset.EnemySkillIds
                        .Select(SkillId.Of)
                        .ToList()
                },
                ViewPrefabCatalog = matchPreset.ViewPrefabCatalog,
                ObjectLayoutView = objectLayoutView
            };
        }

        private static CpuAgentOptions ToCpuOptions(CpuAgentPreset preset)
        {
            return preset == null
                ? CpuAgentOptions.Default
                : preset.ToOptions();
        }

        public void NewSession()
        {
            _boardGamePort.SendGameRequest(new NewSessionRequest(_setting));
            _boardGamePort.SendMatchCommand(new MatchStartCommand("DebugPort"));
        }

        public void Reset()
        {
            _boardGamePort.SendGameRequest(new ResetRequest());
        }
    }
}
