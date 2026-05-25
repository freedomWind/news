using System;
using NewsFramework.Data.Replay;
using NewsFramework.Simulation;
using UnityEngine;

namespace NewsFramework.Replay
{
    public sealed class NullReplayRuntime : IReplayRuntime
    {
        private RenderTexture texture;

        public string RuntimeId { get; private set; }
        public ReplayRuntimeState State { get; private set; } = ReplayRuntimeState.Empty;
        public ReplayData Data { get; private set; }
        public IGameSimulation Simulation { get; private set; }
        public RenderTexture Texture => texture;
        public int CurrentStepIndex { get; private set; }

        public void Load(ReplayData data, ReplayRuntimeOptions options)
        {
            ReleaseTexture();

            Data = data;
            RuntimeId = data != null && !string.IsNullOrEmpty(data.replayId)
                ? data.replayId
                : Guid.NewGuid().ToString("N");
            CurrentStepIndex = data != null ? data.startStepIndex : 0;

            var width = ResolveTextureWidth(data, options);
            var height = ResolveTextureHeight(data, options);
            texture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32)
            {
                name = "ReplayRT_" + RuntimeId
            };
            texture.Create();
            ClearTexture();

            if (Simulation == null)
            {
                Simulation = GameSimulationFactory.Create(new GameSimulationConfig
                {
                    gameId = data != null ? data.gameId : string.Empty,
                    initialState = data != null ? data.initialState : string.Empty,
                    initialStateFormat = data != null ? data.initialStateFormat : string.Empty
                });
            }

            Simulation.Load(new GameSimulationConfig
            {
                gameId = data != null ? data.gameId : string.Empty,
                initialState = data != null ? data.initialState : string.Empty,
                initialStateFormat = data != null ? data.initialStateFormat : string.Empty
            });

            State = ReplayRuntimeState.Loaded;
        }

        public void BindSimulation(IGameSimulation simulation)
        {
            if (State == ReplayRuntimeState.Disposed)
            {
                return;
            }

            if (Simulation != null && !ReferenceEquals(Simulation, simulation))
            {
                Simulation.Dispose();
            }

            Simulation = simulation;
        }

        public void Play()
        {
            if (State == ReplayRuntimeState.Disposed || State == ReplayRuntimeState.Empty)
            {
                return;
            }

            State = ReplayRuntimeState.Playing;
        }

        public void Pause()
        {
            if (State == ReplayRuntimeState.Playing)
            {
                State = ReplayRuntimeState.Paused;
            }
        }

        public void Stop()
        {
            if (State == ReplayRuntimeState.Disposed)
            {
                return;
            }

            CurrentStepIndex = Data != null ? Data.startStepIndex : 0;
            State = ReplayRuntimeState.Stopped;
        }

        public void Seek(int stepIndex)
        {
            if (State == ReplayRuntimeState.Disposed)
            {
                return;
            }

            CurrentStepIndex = stepIndex;
            ClearTexture();
        }

        public void Tick(float deltaTime)
        {
            // Intentionally empty. Real replay runtimes will advance deterministic simulation here.
        }

        public void Dispose()
        {
            if (State == ReplayRuntimeState.Disposed)
            {
                return;
            }

            if (texture != null)
            {
                ReleaseTexture();
            }

            Simulation?.Dispose();
            Simulation = null;

            State = ReplayRuntimeState.Disposed;
        }

        private static int ResolveTextureWidth(ReplayData data, ReplayRuntimeOptions options)
        {
            if (options != null && options.textureWidth > 0)
            {
                return options.textureWidth;
            }

            return data != null && data.renderProfile != null ? data.renderProfile.textureWidth : 1024;
        }

        private static int ResolveTextureHeight(ReplayData data, ReplayRuntimeOptions options)
        {
            if (options != null && options.textureHeight > 0)
            {
                return options.textureHeight;
            }

            return data != null && data.renderProfile != null ? data.renderProfile.textureHeight : 576;
        }

        private void ReleaseTexture()
        {
            if (texture == null)
            {
                return;
            }

            texture.Release();
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(texture);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }

            texture = null;
        }

        private void ClearTexture()
        {
            if (texture == null)
            {
                return;
            }

            var previous = RenderTexture.active;
            RenderTexture.active = texture;
            GL.Clear(true, true, new Color(0.12f, 0.1f, 0.08f, 1f));
            RenderTexture.active = previous;
        }
    }
}
