using System;
using NewsFramework.Data.Replay;
using NewsFramework.Simulation;
using UnityEngine;

namespace NewsFramework.Replay
{
    public interface IReplayRuntime : IDisposable
    {
        string RuntimeId { get; }
        ReplayRuntimeState State { get; }
        ReplayData Data { get; }
        IGameSimulation Simulation { get; }
        RenderTexture Texture { get; }
        int CurrentStepIndex { get; }

        void Load(ReplayData data, ReplayRuntimeOptions options);
        void BindSimulation(IGameSimulation simulation);
        void Play();
        void Pause();
        void Stop();
        void Seek(int stepIndex);
        void Tick(float deltaTime);
    }
}
