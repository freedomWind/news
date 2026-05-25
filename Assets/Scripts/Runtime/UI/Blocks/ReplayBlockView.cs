using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.GameRoom;
using NewsFramework.Data.Replay;
using NewsFramework.Simulation;
using NewsFramework.Simulation.Chess;

using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class ReplayBlockView : BlockViewBase
    {
        private const float DefaultAspectRatio = 1.777f;

        private LayoutElement rootLayout;
        private LayoutElement surfaceLayout;
        private RectTransform surfaceRect;
        private RectTransform boardArea;
        private Image progressFill;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI statusLabel;
        private TextMeshProUGUI stepLabel;
        private TextMeshProUGUI playButtonLabel;

        private ChessSimulation sim;
        private ReplayData replayData;
        private float stepTimer;
        private int currentPosition;

        private readonly List<RectTransform> activePieces = new List<RectTransform>(32);
        private readonly List<Image> activeMarkers = new List<Image>(2);

        private float BoardHeight => surfaceLayout != null ? surfaceLayout.preferredHeight * 0.9f : 180f;
        private float BoardWidth => BoardHeight * (ChessBoardRenderer.DefaultBoardWidth / ChessBoardRenderer.DefaultBoardHeight);
        private float PieceSize => Mathf.Max(14f, BoardHeight / 12f);

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("ReplayBlock", parent);
            var view = root.gameObject.AddComponent<ReplayBlockView>();
            view.rootLayout = AppUIFactory.AddLayoutElement(root.gameObject, 320f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 8f, new RectOffset(0, 0, 0, 0));

            view.titleLabel = AppUIFactory.CreateText(
                "Title", root, string.Empty, 16f,
                AppTheme.PrimaryText, FontStyles.Bold);
            AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 24f);

            var surface = AppUIFactory.CreateImage("BoardSurface", root, AppTheme.DarkBar);
            view.surfaceRect = surface.rectTransform;
            view.surfaceLayout = AppUIFactory.AddLayoutElement(surface.gameObject, 210f);

            view.statusLabel = AppUIFactory.CreateText(
                "Status", surface.transform, string.Empty, 13f,
                Color.white, FontStyles.Normal, TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.statusLabel.rectTransform);

            var controls = AppUIFactory.CreateRect("Controls", root);
            AppUIFactory.AddLayoutElement(controls.gameObject, 38f);
            AppUIFactory.AddHorizontalLayout(controls.gameObject, 8f,
                new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            CreateIconButton("Previous", controls, "\u25C0", view.StepBackward);
            var playButton = CreateIconButton("PlayPause", controls, "\u25B6", view.TogglePlayback);
            view.playButtonLabel = playButton.GetComponentInChildren<TextMeshProUGUI>();
            CreateIconButton("Next", controls, "\u25B6", view.StepForward);

            view.stepLabel = AppUIFactory.CreateText(
                "Step", controls, string.Empty, 12f,
                AppTheme.SecondaryText, FontStyles.Normal, TextAlignmentOptions.MidlineLeft);
            var stepLayout = AppUIFactory.AddLayoutElement(view.stepLabel.gameObject, 28f, 92f);
            stepLayout.flexibleWidth = 1f;

            var progress = AppUIFactory.CreateImage("Progress", root, AppTheme.Hairline);
            AppUIFactory.AddLayoutElement(progress.gameObject, 4f);
            progress.type = Image.Type.Simple;

            view.progressFill = AppUIFactory.CreateImage("Fill", progress.transform, AppTheme.Accent);
            view.progressFill.type = Image.Type.Filled;
            view.progressFill.fillMethod = Image.FillMethod.Horizontal;
            view.progressFill.fillOrigin = 0;
            view.progressFill.fillAmount = 0f;
            AppUIFactory.Stretch(view.progressFill.rectTransform);

            return view;
        }

        protected override void OnBind(BlockData data)
        {
            DisposeSimulation();

            replayData = data.replay;
            titleLabel.text = string.IsNullOrEmpty(data.title) ? "\u5BF9\u5C40\u56DE\u653E" : data.title;

            var aspectRatio = replayData?.renderProfile != null
                ? ResolveAspectRatio(replayData.renderProfile)
                : DefaultAspectRatio;
            var surfaceHeight = Mathf.Clamp(343f / aspectRatio, 170f, 260f);
            surfaceLayout.preferredHeight = surfaceHeight;
            rootLayout.preferredHeight = surfaceHeight + 78f;

            if (replayData != null)
            {
                sim = new ChessSimulation();
                var config = new GameSimulationConfig
                {
                    gameId = "xiangqi",
                    initialState = replayData.initialState ?? string.Empty,
                    initialStateFormat = "fen"
                };
                sim.Load(config);

                BuildBoard(surfaceRect);
                ApplyStep(0);
            }

            UpdateProgress();
        }

        private void BuildBoard(RectTransform surface)
        {
            boardArea = AppUIFactory.CreateRect("BoardArea", surface);
            AppUIFactory.Stretch(boardArea);

            var boardImage = ChessBoardRenderer.CreateBoardBackground(boardArea, BoardWidth, BoardHeight);
            boardImage.raycastTarget = false;
        }

        private void RenderBoard()
        {
            ClearBoard();
            if (sim == null || !sim.IsLoaded || boardArea == null) return;

            var pieces = BoardToPieces(sim.GetBoard());
            var pieceSize = PieceSize;

            foreach (var p in pieces)
            {
                var icon = ChessBoardRenderer.CreatePieceIcon(boardArea, p, pieceSize, false);
                activePieces.Add(icon);
            }

            if (currentPosition > 0 && sim.GetRecorder() != null && currentPosition - 1 < sim.GetRecorder().MoveCount)
            {
                var lastMove = sim.GetRecorder().Moves[currentPosition - 1];
                ChessBoardRenderer.CreateMoveMarker(
                    boardArea, lastMove.To.Col, lastMove.To.Row,
                    pieceSize, Color.yellow, 0.6f, BoardWidth, BoardHeight);
            }
        }

        private void ApplyStep(int index)
        {
            if (replayData == null || sim == null || !sim.IsLoaded)
            {
                statusLabel.text = string.Empty;
                return;
            }

            currentPosition = Mathf.Clamp(index, 0, Mathf.Max(0, ResolveStepCount() - 1));

            sim.Reset();
            var config = new GameSimulationConfig
            {
                gameId = "xiangqi",
                initialState = replayData.initialState ?? string.Empty,
                initialStateFormat = "fen"
            };
            sim.Load(config);

            if (replayData.steps != null)
            {
                int target = Mathf.Min(currentPosition, replayData.steps.Count);
                for (int i = 0; i < target; i++)
                {
                    var step = replayData.steps[i];
                    string error;
                    ChessMove move;
                    if (!string.IsNullOrEmpty(step.command))
                    {
                        var parsed = ChessGameRecorder.IccsToMove(sim.GetBoard(), step.command);
                        if (parsed.HasValue)
                        {
                            sim.TryMakeMove(parsed.Value.From, parsed.Value.To, out move, out error);
                        }
                    }
                }
            }

            RenderBoard();
            statusLabel.text = FormatStatus();
            UpdateProgress();
        }

        private void StepBackward()
        {
            if (currentPosition <= 0) return;
            StopPlayback();
            ApplyStep(currentPosition - 1);
        }

        private void StepForward()
        {
            if (currentPosition >= ResolveStepCount() - 1) return;
            StopPlayback();
            ApplyStep(currentPosition + 1);
        }

        private bool isPlaying;

        private void TogglePlayback()
        {
            if (isPlaying) StopPlayback();
            else StartPlayback();
        }

        private void StartPlayback()
        {
            if (currentPosition >= ResolveStepCount() - 1)
                ApplyStep(0);
            isPlaying = true;
            stepTimer = 0f;
        }

        private void StopPlayback()
        {
            isPlaying = false;
            stepTimer = 0f;
            if (playButtonLabel != null)
                playButtonLabel.text = "\u25B6";
        }

        private void Update()
        {
            if (!isPlaying || sim == null) return;

            stepTimer += Time.deltaTime;
            var duration = ResolveCurrentDuration();
            if (stepTimer >= duration)
            {
                stepTimer = 0f;
                if (currentPosition >= ResolveStepCount() - 1)
                {
                    StopPlayback();
                    return;
                }
                ApplyStep(currentPosition + 1);
            }
        }

        private void ClearBoard()
        {
            foreach (var p in activePieces)
            {
                if (p != null) Destroy(p.gameObject);
            }
            activePieces.Clear();

            foreach (var m in activeMarkers)
            {
                if (m != null) Destroy(m.gameObject);
            }
            activeMarkers.Clear();
        }

        private static List<GamePieceData> BoardToPieces(ChessBoard board)
        {
            var pieces = new List<GamePieceData>(32);
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    var piece = board[row, col];
                    if (!piece.IsValid) continue;
                    pieces.Add(new GamePieceData
                    {
                        pieceId = row + "_" + col,
                        text = piece.ToChar().ToString(),
                        side = piece.Side == ChessSide.Red ? GamePieceSide.Red : GamePieceSide.Black,
                        x = col,
                        y = row
                    });
                }
            }
            return pieces;
        }

        private int ResolveStepCount()
        {
            if (replayData == null) return 1;
            if (replayData.steps != null && replayData.steps.Count > 0)
                return replayData.steps.Count;
            return Mathf.Max(1, replayData.ResolveEndStepIndex() - replayData.startStepIndex + 1);
        }

        private float ResolveCurrentDuration()
        {
            var fallback = replayData != null && replayData.secondsPerStep > 0f
                ? replayData.secondsPerStep : 0.6f;
            if (replayData?.steps == null || replayData.steps.Count == 0) return fallback;
            var step = replayData.steps[Mathf.Clamp(currentPosition, 0, replayData.steps.Count - 1)];
            return step.duration > 0f ? step.duration : fallback;
        }

        private void UpdateProgress()
        {
            var stepCount = ResolveStepCount();
            var normalized = stepCount <= 1 ? 1f : currentPosition / (float)(stepCount - 1);

            if (progressFill != null)
                progressFill.fillAmount = Mathf.Clamp01(normalized);

            if (stepLabel != null)
                stepLabel.text = (currentPosition + 1) + " / " + stepCount;

            if (playButtonLabel != null)
                playButtonLabel.text = isPlaying ? "\u23F8" : "\u25B6";
        }

        private string FormatStatus()
        {
            if (replayData?.steps != null && replayData.steps.Count > 0)
            {
                var step = replayData.steps[Mathf.Clamp(currentPosition, 0, replayData.steps.Count - 1)];
                return string.IsNullOrEmpty(step.notation) ? step.command : step.notation;
            }
            return string.Empty;
        }

        private static float ResolveAspectRatio(ReplayRenderProfileData rp)
        {
            if (rp == null || rp.textureWidth <= 0 || rp.textureHeight <= 0)
                return DefaultAspectRatio;
            return rp.textureWidth / (float)rp.textureHeight;
        }

        private static Button CreateIconButton(string name, Transform parent, string icon,
            UnityEngine.Events.UnityAction onClick)
        {
            var button = AppUIFactory.CreateButton(name, parent, AppTheme.SurfaceMuted, onClick);
            AppUIFactory.AddLayoutElement(button.gameObject, 34f, 44f);
            var label = AppUIFactory.CreateText("Icon", button.transform, icon,
                18f, AppTheme.PrimaryText, FontStyles.Bold, TextAlignmentOptions.Center);
            AppUIFactory.Stretch(label.rectTransform);
            return button;
        }

        private void OnDestroy()
        {
            DisposeSimulation();
        }

        private void DisposeSimulation()
        {
            ClearBoard();
            if (boardArea != null) { Destroy(boardArea.gameObject); boardArea = null; }
            sim?.Dispose();
            sim = null;
        }
    }
}