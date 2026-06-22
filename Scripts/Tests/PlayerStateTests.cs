using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Quoridor;

namespace Quoridor.Tests
{
    /// <summary>
    /// Unit tests for the PlayerState class.
    /// Verifies the behavior of OnTurnStarted(), AddStatus(), HasStatus(), DeepCopy(), and related methods.
    /// </summary>
    [TestFixture]
    public class PlayerStateTests
    {
        // ---- Helper methods ----

        /// <summary>
        /// Creates a PlayerState for tests.
        /// </summary>
        private static PlayerState CreatePlayerState(
            PlayerId playerId = null,
            Dictionary<SkillSlotId, SkillState> skills = null,
            List<StatusState> statuses = null,
            PlayerRuntimeState runtime = null
        )
        {
            return new PlayerState(
                playerId ?? PlayerId.FirstPlayer,
                isActive: true,
                skills ?? new Dictionary<SkillSlotId, SkillState>(),
                statuses ?? new List<StatusState>(),
                runtime ?? new PlayerRuntimeState()
            );
        }

        /// <summary>
        /// Creates a SkillState for tests with cooldown values.
        /// </summary>
        private static SkillState CreateSkillState(
            int coolDownTurns = 3,
            int coolDownRemaining = 2
        )
        {
            return new SkillState(
                SkillId.NormalMovePawn,
                remainingUses: null,
                coolDownTurns: coolDownTurns,
                coolDownRemaining: coolDownRemaining,
                charge: null
            );
        }

        /// <summary>
        /// Creates a StatusState for tests.
        /// </summary>
        private static StatusState CreateStatusState(
            StatusId statusId = StatusId.Sleep,
            int remainingTurns = 3,
            int coolDownTurns = 0,
            int coolDownRemaining = 0
        )
        {
            return new StatusState(
                statusId,
                remainingTurns: remainingTurns,
                coolDownTurns: coolDownTurns,
                coolDownRemaining: coolDownRemaining
            );
        }

        // ---- OnTurnStarted: decrements status RemainingTurns ----

        [Test]
        public void OnTurnStarted_DecrementsStatusRemainingTurnsByOne()
        {
            // Arrange
            var status = CreateStatusState(remainingTurns: 3);
            var player = CreatePlayerState(statuses: new List<StatusState> { status });

            // Act
            player.OnTurnStarted();

            // Assert
            Assert.AreEqual(2, player.Statuses[0].RemainingTurns);
        }

        [Test]
        public void OnTurnStarted_KeepsPermanentStatusRemainingTurnsNull()
        {
            // Arrange: RemainingTurns = null means permanent.
            var status = new StatusState(
                StatusId.Sleep,
                remainingTurns: null,
                coolDownTurns: 0,
                coolDownRemaining: 0
            );
            var player = CreatePlayerState(statuses: new List<StatusState> { status });

            // Act
            player.OnTurnStarted();

            // Assert: permanent statuses are not removed.
            Assert.AreEqual(1, player.Statuses.Count);
            Assert.IsNull(player.Statuses[0].RemainingTurns);
        }

        // ---- OnTurnStarted: removes expired statuses ----

        [Test]
        public void OnTurnStarted_RemovesStatusWithOneRemainingTurnAfterAdvance()
        {
            // Arrange: RemainingTurns = 1 becomes 0 after Advance(), so IsExpired becomes true.
            var status = CreateStatusState(remainingTurns: 1);
            var player = CreatePlayerState(statuses: new List<StatusState> { status });

            // Act
            player.OnTurnStarted();

            // Assert
            Assert.AreEqual(0, player.Statuses.Count);
        }

        [Test]
        public void OnTurnStarted_KeepsStatusWithTwoRemainingTurnsAfterAdvance()
        {
            // Arrange
            var status = CreateStatusState(remainingTurns: 2);
            var player = CreatePlayerState(statuses: new List<StatusState> { status });

            // Act
            player.OnTurnStarted();

            // Assert
            Assert.AreEqual(1, player.Statuses.Count);
        }

        // ---- OnTurnStarted: StatusRemovedEvent publishing rules ----

        [Test]
        public void OnTurnStarted_PublishesStatusRemovedEventWhenExpiredStatusIsRemoved()
        {
            // Arrange
            var status = CreateStatusState(statusId: StatusId.Sleep, remainingTurns: 1);
            var player = CreatePlayerState(statuses: new List<StatusState> { status });

            // Act
            StateChangeResult result = player.OnTurnStarted();

            // Assert
            Assert.IsTrue(result.HasEvents);
            var removedEvent = result.Events.OfType<StatusRemovedEvent>().FirstOrDefault();
            Assert.IsNotNull(removedEvent);
            Assert.AreEqual(StatusId.Sleep, removedEvent.StatusId);
        }

        [Test]
        public void OnTurnStarted_DoesNotPublishEventWhenNoStatusExpires()
        {
            // Arrange: RemainingTurns = 3, so it does not expire yet.
            var status = CreateStatusState(remainingTurns: 3);
            var player = CreatePlayerState(statuses: new List<StatusState> { status });

            // Act
            StateChangeResult result = player.OnTurnStarted();

            // Assert
            var removedEvents = result.Events.OfType<StatusRemovedEvent>().ToList();
            Assert.AreEqual(0, removedEvents.Count);
        }

        [Test]
        public void OnTurnStarted_PublishesStatusRemovedEventOnlyWhenLastStackedStatusIsRemoved()
        {
            // Arrange: add two statuses with the same StatusId to represent stacked statuses.
            // Both have RemainingTurns = 1, so both expire after Advance(); the event is published only once when the final stack is removed.
            var status1 = CreateStatusState(statusId: StatusId.Sleep, remainingTurns: 1);
            var status2 = CreateStatusState(statusId: StatusId.Sleep, remainingTurns: 1);
            var player = CreatePlayerState(statuses: new List<StatusState> { status1, status2 });

            // Act
            StateChangeResult result = player.OnTurnStarted();

            // Assert: StatusRemovedEvent is published only once.
            var removedEvents = result.Events.OfType<StatusRemovedEvent>().ToList();
            Assert.AreEqual(1, removedEvents.Count);
            Assert.AreEqual(StatusId.Sleep, removedEvents[0].StatusId);
        }

        [Test]
        public void OnTurnStarted_DoesNotPublishStatusRemovedEventWhenOnlyPartOfStackedStatusExpires()
        {
            // Arrange: add two statuses with the same StatusId.
            // One has RemainingTurns = 1 and expires; the other has RemainingTurns = 3 and remains.
            var status1 = CreateStatusState(statusId: StatusId.Sleep, remainingTurns: 1);
            var status2 = CreateStatusState(statusId: StatusId.Sleep, remainingTurns: 3);
            var player = CreatePlayerState(statuses: new List<StatusState> { status1, status2 });

            // Act
            StateChangeResult result = player.OnTurnStarted();

            // Assert: the same StatusId still remains, so StatusRemovedEvent is not published.
            var removedEvents = result.Events.OfType<StatusRemovedEvent>().ToList();
            Assert.AreEqual(0, removedEvents.Count);

            // One status remains.
            Assert.AreEqual(1, player.Statuses.Count);
        }

        [Test]
        public void OnTurnStarted_PublishesStatusRemovedEventForEachExpiredDifferentStatusId()
        {
            // Arrange
            var status1 = CreateStatusState(statusId: StatusId.Sleep, remainingTurns: 1);
            var status2 = CreateStatusState(statusId: StatusId.Paralysis, remainingTurns: 1);
            var player = CreatePlayerState(statuses: new List<StatusState> { status1, status2 });

            // Act
            StateChangeResult result = player.OnTurnStarted();

            // Assert: events are published for both status types.
            var removedEvents = result.Events.OfType<StatusRemovedEvent>().ToList();
            Assert.AreEqual(2, removedEvents.Count);
            Assert.IsTrue(removedEvents.Any(e => e.StatusId == StatusId.Sleep));
            Assert.IsTrue(removedEvents.Any(e => e.StatusId == StatusId.Paralysis));
        }

        // ---- OnTurnStarted: advances skill cooldowns ----

        [Test]
        public void OnTurnStarted_DecrementsSkillCoolDownRemainingByOne()
        {
            // Arrange
            var skill = CreateSkillState(coolDownTurns: 3, coolDownRemaining: 2);
            var skills = new Dictionary<SkillSlotId, SkillState>
            {
                { BuiltInSkillSlotIds.MovePawn, skill }
            };
            var player = CreatePlayerState(skills: skills);

            // Act
            player.OnTurnStarted();

            // Assert
            Assert.AreEqual(1, player.Skills[BuiltInSkillSlotIds.MovePawn].CoolDownRemaining);
        }

        [Test]
        public void OnTurnStarted_DoesNotChangeSkillWhenCoolDownRemainingIsZero()
        {
            // Arrange: no active cooldown.
            var skill = CreateSkillState(coolDownTurns: 3, coolDownRemaining: 0);
            var skills = new Dictionary<SkillSlotId, SkillState>
            {
                { BuiltInSkillSlotIds.MovePawn, skill }
            };
            var player = CreatePlayerState(skills: skills);

            // Act
            player.OnTurnStarted();

            // Assert
            Assert.AreEqual(0, player.Skills[BuiltInSkillSlotIds.MovePawn].CoolDownRemaining);
        }

        // ---- AddStatus ----

        [Test]
        public void AddStatus_AddsStatusToStatusesList()
        {
            // Arrange
            var player = CreatePlayerState();
            var status = CreateStatusState(statusId: StatusId.Sleep);

            // Act
            player.AddStatus(status);

            // Assert
            Assert.AreEqual(1, player.Statuses.Count);
            Assert.AreEqual(StatusId.Sleep, player.Statuses[0].StatusId);
        }

        [Test]
        public void AddStatus_PublishesStatusAddedEvent()
        {
            // Arrange
            var player = CreatePlayerState();
            var status = CreateStatusState(statusId: StatusId.Sleep);

            // Act
            StateChangeResult result = player.AddStatus(status);

            // Assert
            Assert.IsTrue(result.HasEvents);
            var addedEvent = result.Events.OfType<StatusAddedEvent>().FirstOrDefault();
            Assert.IsNotNull(addedEvent);
            Assert.AreEqual(StatusId.Sleep, addedEvent.StatusId);
        }

        [Test]
        public void AddStatus_CanAddMultipleStatuses()
        {
            // Arrange
            var player = CreatePlayerState();

            // Act
            player.AddStatus(CreateStatusState(statusId: StatusId.Sleep));
            player.AddStatus(CreateStatusState(statusId: StatusId.Paralysis));

            // Assert
            Assert.AreEqual(2, player.Statuses.Count);
        }

        [Test]
        public void AddStatus_CanAddMultipleStatusesWithSameStatusId()
        {
            // Arrange
            var player = CreatePlayerState();

            // Act
            player.AddStatus(CreateStatusState(statusId: StatusId.Sleep));
            player.AddStatus(CreateStatusState(statusId: StatusId.Sleep));

            // Assert: the stack policy allows two entries.
            Assert.AreEqual(2, player.Statuses.Count);
        }

        // ---- HasStatus ----

        [Test]
        public void HasStatus_ReturnsTrueForExistingStatusId()
        {
            // Arrange
            var player = CreatePlayerState();
            player.AddStatus(CreateStatusState(statusId: StatusId.Sleep));

            // Act & Assert
            Assert.IsTrue(player.HasStatus(StatusId.Sleep));
        }

        [Test]
        public void HasStatus_ReturnsFalseForMissingStatusId()
        {
            // Arrange
            var player = CreatePlayerState();
            player.AddStatus(CreateStatusState(statusId: StatusId.Sleep));

            // Act & Assert
            Assert.IsFalse(player.HasStatus(StatusId.Paralysis));
        }

        [Test]
        public void HasStatus_ReturnsFalseWhenStatusesAreEmpty()
        {
            // Arrange
            var player = CreatePlayerState();

            // Act & Assert
            Assert.IsFalse(player.HasStatus(StatusId.Sleep));
        }

        [Test]
        public void HasStatus_ReturnsFalseAfterStatusIsRemoved()
        {
            // Arrange: RemainingTurns = 1, so it is removed by OnTurnStarted().
            var status = CreateStatusState(statusId: StatusId.Sleep, remainingTurns: 1);
            var player = CreatePlayerState(statuses: new List<StatusState> { status });

            // Act
            player.OnTurnStarted();

            // Assert
            Assert.IsFalse(player.HasStatus(StatusId.Sleep));
        }

        // ---- DeepCopy ----

        [Test]
        public void DeepCopy_CopyIsIndependentFromOriginalInstance()
        {
            // Arrange
            var status = CreateStatusState(statusId: StatusId.Sleep, remainingTurns: 3);
            var original = CreatePlayerState(statuses: new List<StatusState> { status });

            // Act
            PlayerState copy = original.DeepCopy();
            copy.OnTurnStarted(); // Modify the copy.

            // Assert: the original instance does not change.
            Assert.AreEqual(3, original.Statuses[0].RemainingTurns);
            Assert.AreEqual(2, copy.Statuses[0].RemainingTurns);
        }

        [Test]
        public void DeepCopy_StatusAddedToCopyDoesNotAffectOriginal()
        {
            // Arrange
            var original = CreatePlayerState();

            // Act
            PlayerState copy = original.DeepCopy();
            copy.AddStatus(CreateStatusState(statusId: StatusId.Sleep));

            // Assert
            Assert.AreEqual(0, original.Statuses.Count);
            Assert.AreEqual(1, copy.Statuses.Count);
        }

        [Test]
        public void DeepCopy_SkillCoolDownRemainingIsIndependentInCopy()
        {
            // Arrange
            var skill = CreateSkillState(coolDownTurns: 3, coolDownRemaining: 2);
            var skills = new Dictionary<SkillSlotId, SkillState>
            {
                { BuiltInSkillSlotIds.MovePawn, skill }
            };
            var original = CreatePlayerState(skills: skills);

            // Act
            PlayerState copy = original.DeepCopy();
            copy.OnTurnStarted(); // Advance cooldown on the copy.

            // Assert: the original instance does not change.
            Assert.AreEqual(2, original.Skills[BuiltInSkillSlotIds.MovePawn].CoolDownRemaining);
            Assert.AreEqual(1, copy.Skills[BuiltInSkillSlotIds.MovePawn].CoolDownRemaining);
        }

        [Test]
        public void DeepCopy_CopiesPlayerIdAndOtherProperties()
        {
            // Arrange
            var original = CreatePlayerState(playerId: PlayerId.SecondPlayer);

            // Act
            PlayerState copy = original.DeepCopy();

            // Assert
            Assert.AreEqual(PlayerId.SecondPlayer, copy.PlayerId);
            Assert.AreEqual(original.IsActive, copy.IsActive);
        }

        [Test]
        public void DeepCopy_CopiesRuntimeCpuAndAutoProperties()
        {
            // Arrange
            var original = CreatePlayerState(
                runtime: new PlayerRuntimeState(isCpu: true)
            );

            // Act
            PlayerState copy = original.DeepCopy();

            // Assert
            Assert.IsTrue(copy.Runtime.IsCpu);
            Assert.IsTrue(copy.Runtime.IsAuto);
        }

        [Test]
        public void RuntimeReset_RestoresAutoFromCpu()
        {
            // Arrange
            var runtime = new PlayerRuntimeState(isCpu: true);
            runtime.SetAuto(false);
            runtime.ProhibitAction();
            runtime.ProhibitMove();
            runtime.ProhibitWallPlacement();
            runtime.ProhibitSpecialSkill();

            // Act
            runtime.Reset();

            // Assert
            Assert.IsTrue(runtime.CanAct);
            Assert.IsTrue(runtime.CanMove);
            Assert.IsTrue(runtime.CanPlaceWall);
            Assert.IsTrue(runtime.CanUseSpecialSkill);
            Assert.IsTrue(runtime.IsAuto);
        }

        // ---- Event PlayerId verification ----

        [Test]
        public void AddStatus_StatusAddedEventContainsPlayerId()
        {
            // Arrange
            var player = CreatePlayerState(playerId: PlayerId.SecondPlayer);
            var status = CreateStatusState(statusId: StatusId.Sleep);

            // Act
            StateChangeResult result = player.AddStatus(status);

            // Assert
            var addedEvent = result.Events.OfType<StatusAddedEvent>().First();
            Assert.AreEqual(PlayerId.SecondPlayer, addedEvent.PlayerId);
        }

        [Test]
        public void OnTurnStarted_StatusRemovedEventContainsPlayerId()
        {
            // Arrange
            var status = CreateStatusState(statusId: StatusId.Sleep, remainingTurns: 1);
            var player = CreatePlayerState(
                playerId: PlayerId.SecondPlayer,
                statuses: new List<StatusState> { status }
            );

            // Act
            StateChangeResult result = player.OnTurnStarted();

            // Assert
            var removedEvent = result.Events.OfType<StatusRemovedEvent>().First();
            Assert.AreEqual(PlayerId.SecondPlayer, removedEvent.PlayerId);
        }

        // ---- UseSkill ----

        [Test]
        public void UseSkill_PublishesSkillUsedEventWhenSkillIsUsable()
        {
            // Arrange: no cooldown and no remaining-use limit.
            var skill = new SkillState(
                SkillId.NormalMovePawn,
                remainingUses: null,
                coolDownTurns: 0,
                coolDownRemaining: 0,
                charge: null
            );
            var skills = new Dictionary<SkillSlotId, SkillState>
            {
                { BuiltInSkillSlotIds.MovePawn, skill }
            };
            var player = CreatePlayerState(skills: skills);

            // Act
            StateChangeResult result = player.UseSkill(BuiltInSkillSlotIds.MovePawn);

            // Assert
            Assert.IsTrue(result.HasEvents);
            var usedEvent = result.Events.OfType<SkillUsedEvent>().FirstOrDefault();
            Assert.IsNotNull(usedEvent);
            Assert.AreEqual(SkillId.NormalMovePawn, usedEvent.SkillId);
            Assert.AreEqual(BuiltInSkillSlotIds.MovePawn, usedEvent.SkillSlotId);
        }

        [Test]
        public void UseSkill_ThrowsExceptionWhenSkillIsOnCooldown()
        {
            // Arrange: CoolDownRemaining > 0
            var skill = CreateSkillState(coolDownTurns: 3, coolDownRemaining: 2);
            var skills = new Dictionary<SkillSlotId, SkillState>
            {
                { BuiltInSkillSlotIds.MovePawn, skill }
            };
            var player = CreatePlayerState(skills: skills);

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(
                () => player.UseSkill(BuiltInSkillSlotIds.MovePawn)
            );
        }

        [Test]
        public void UseSkill_ThrowsExceptionWhenRemainingUsesIsZero()
        {
            // Arrange: RemainingUses = 0
            var skill = new SkillState(
                SkillId.NormalMovePawn,
                remainingUses: 0,
                coolDownTurns: 0,
                coolDownRemaining: 0,
                charge: null
            );
            var skills = new Dictionary<SkillSlotId, SkillState>
            {
                { BuiltInSkillSlotIds.MovePawn, skill }
            };
            var player = CreatePlayerState(skills: skills);

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(
                () => player.UseSkill(BuiltInSkillSlotIds.MovePawn)
            );
        }

        [Test]
        public void UseSkill_ResetsCoolDownRemainingWhenSkillHasCooldown()
        {
            // Arrange: CoolDownTurns = 3 and CoolDownRemaining = 0, so the skill is usable.
            var skill = new SkillState(
                SkillId.NormalMovePawn,
                remainingUses: null,
                coolDownTurns: 3,
                coolDownRemaining: 0,
                charge: null
            );
            var skills = new Dictionary<SkillSlotId, SkillState>
            {
                { BuiltInSkillSlotIds.MovePawn, skill }
            };
            var player = CreatePlayerState(skills: skills);

            // Act
            player.UseSkill(BuiltInSkillSlotIds.MovePawn);

            // Assert: CoolDownRemaining is reset to CoolDownTurns.
            Assert.AreEqual(3, player.Skills[BuiltInSkillSlotIds.MovePawn].CoolDownRemaining);
        }

        // ---- GetSkill / TryGetSkillBySlotId ----

        [Test]
        public void GetSkill_ReturnsSkillStateForExistingSlotId()
        {
            // Arrange
            var skill = CreateSkillState();
            var skills = new Dictionary<SkillSlotId, SkillState>
            {
                { BuiltInSkillSlotIds.MovePawn, skill }
            };
            var player = CreatePlayerState(skills: skills);

            // Act
            SkillState result = player.GetSkill(BuiltInSkillSlotIds.MovePawn);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(SkillId.NormalMovePawn, result.SkillId);
        }

        [Test]
        public void GetSkill_ThrowsExceptionForMissingSlotId()
        {
            // Arrange: no skills.
            var player = CreatePlayerState();

            // Act & Assert
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                () => player.GetSkill(BuiltInSkillSlotIds.MovePawn)
            );
        }

        [Test]
        public void TryGetSkillBySlotId_ReturnsTrueAndOutputsSkillStateForExistingSlotId()
        {
            // Arrange
            var skill = CreateSkillState();
            var skills = new Dictionary<SkillSlotId, SkillState>
            {
                { BuiltInSkillSlotIds.MovePawn, skill }
            };
            var player = CreatePlayerState(skills: skills);

            // Act
            bool found = player.TryGetSkillBySlotId(BuiltInSkillSlotIds.MovePawn, out SkillState result);

            // Assert
            Assert.IsTrue(found);
            Assert.IsNotNull(result);
        }

        [Test]
        public void TryGetSkillBySlotId_ReturnsFalseForMissingSlotId()
        {
            // Arrange
            var player = CreatePlayerState();

            // Act
            bool found = player.TryGetSkillBySlotId(BuiltInSkillSlotIds.MovePawn, out SkillState result);

            // Assert
            Assert.IsFalse(found);
            Assert.IsNull(result);
        }
    }
}
