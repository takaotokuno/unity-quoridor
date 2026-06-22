using System.Collections.Generic;

namespace Quoridor
{
    public sealed class ProbabilisticCannotActStatusEffectProcessor
        : IStatusEffectProcessor
    {
        private readonly IRandomProvider _randomProvider;

        public StatusEffectId EffectId => StatusEffectId.ProbabilisticCannotAct;

        public ProbabilisticCannotActStatusEffectProcessor(
            IRandomProvider randomProvider
        )
        {
            _randomProvider = randomProvider;
        }

        public IReadOnlyList<IMatchEvent> Apply(StatusEffectContext context)
        {
            var rate = context.EffectDefinition.GetInt(
                StatusParameterKeys.Rate,
                100
            );

            if (!_randomProvider.RollPercent(rate))
            {
                return new IMatchEvent[]{};
            }

            context.Player.Runtime.ProhibitAction();

            return new IMatchEvent[]
            {
                new StatusAppliedEvent(
                    context.PlayerId,
                    context.StatusId
                )
            };
        }
    }
}