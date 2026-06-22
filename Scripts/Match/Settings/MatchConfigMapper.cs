using System.Linq;
using UnityEngine;

namespace Quoridor
{
    public static class MatchConfigMapper
    {
        public static MatchStateConfig ToStateConfig(MatchSetting setting)
        {
            return new MatchStateConfig
            {
                BoardSize = setting.BoardSize,
                InitPawns = setting.InitPawns.ToArray(),
                PlayerFirst = ToPlayerConfig(setting.PlayerFirst),
                PlayerSecond = ToPlayerConfig(setting.PlayerSecond),
                StartingSide = setting.StartingSide
            };
        }

        public static MatchPresentationConfig ToPresentationConfig(MatchSetting setting)
        {
            var clone = Object.Instantiate(setting.ObjectLayoutView);
            clone.CanvasView.Show();

            return new MatchPresentationConfig
            {
                BoardSize = setting.BoardSize,
                InitPawns = setting.InitPawns.ToArray(),
                SkillIdsFirst = setting.PlayerFirst.SkillIds.ToArray(),
                SkillIdsSecond = setting.PlayerSecond.SkillIds.ToArray(),
                ViewPrefabCatalog = setting.ViewPrefabCatalog,
                ObjectLayoutView = clone,
            };
        }

        private static PlayerConfig ToPlayerConfig(PlayerSetting setting)
        {
            return new PlayerConfig(
                setting.IsCpu,
                setting.SkillIds.ToArray(),
                setting.CpuStrategyKind
            );
        }
    }
}