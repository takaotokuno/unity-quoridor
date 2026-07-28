using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Quoridor.Tests
{
    public sealed class MatchEventInterpreterTests
    {
        [TestCase(typeof(MatchEventInterpreter))]
        [TestCase(typeof(MatchEventLogObserver))]
        public void ScopedEventObserver_IsDisposableAndReceivesEventBusThroughConstructor(Type type)
        {
            Assert.That(typeof(IDisposable).IsAssignableFrom(type), Is.True);
            Assert.That(
                type.GetConstructors().Single().GetParameters()
                    .Any(parameter => parameter.ParameterType == typeof(IMatchEventBus)),
                Is.True
            );
        }

        [Test]
        public void Lifetime_SubscribesOnConstructionAndUnsubscribesOnDispose()
        {
            var eventBus = new RecordingEventBus();
            var interpreter = new MatchEventInterpreter(
                eventBus,
                new StubSoundService(),
                new StubTimeEffectService(),
                new StubBackgroundEffectService()
            );

            Assert.That(eventBus.SubscribeCount, Is.EqualTo(3));
            Assert.That(eventBus.UnsubscribeCount, Is.Zero);

            interpreter.Dispose();

            Assert.That(eventBus.UnsubscribeCount, Is.EqualTo(3));

            interpreter.Dispose();
            Assert.That(eventBus.UnsubscribeCount, Is.EqualTo(3));
        }

        private sealed class RecordingEventBus : IMatchEventBus
        {
            public int SubscribeCount { get; private set; }
            public int UnsubscribeCount { get; private set; }

            public void Subscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent
            {
                SubscribeCount++;
            }

            public void Unsubscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent
            {
                UnsubscribeCount++;
            }

            public void DispatchEvent<T>(T e) where T : IMatchEvent { }
        }

        private sealed class StubSoundService : ISoundService
        {
            public void PlayBgm(BgmId bgmId, float fadeSeconds = 0f) { }
            public void StopBgm(float fadeSeconds = 0f) { }
            public void PlaySe(SeId seId, float volumeScale = 1f) { }
            public void SetMasterVolume(float volume) { }
            public void SetBgmVolume(float volume) { }
            public void SetSeVolume(float volume) { }
            public void SetMute(bool isMuted) { }
        }

        private sealed class StubTimeEffectService : ITimeEffectService
        {
            public void ApplyHitStop(float duration, float timeScale) { }
        }

        private sealed class StubBackgroundEffectService : IBackgroundEffectService
        {
            public void ApplyPreset(BackgroundEffectPresetId presetId) { }
            public void SetIntensity(float intensity) { }
            public void Flash(Color color, float duration) { }
            public void TransitionTo(BackgroundEffectState state, float duration) { }
            public void ResetToDefault(float duration = 0f) { }
        }
    }
}
