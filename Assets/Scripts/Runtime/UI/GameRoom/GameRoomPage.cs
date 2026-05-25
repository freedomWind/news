using System;
using NewsFramework.Data.GameRoom;
using NewsFramework.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.GameRoom
{
    public sealed class GameRoomPage : MonoBehaviour
    {
        private RectTransform root;
        private RectTransform exitModal;
        private GameRoomData data;
        private Action onBack;

        public void Build(RectTransform parent, GameRoomData roomData, Action backHandler)
        {
            data = roomData ?? new GameRoomData();
            onBack = backHandler;

            var background = AppUIFactory.CreateImage("GameRoomBackground", parent, GameRoomStyle.Walnut);
            AppUIFactory.Stretch(background.rectTransform);

            root = AppUIFactory.CreateRect("GameRoomRoot", parent);
            AppUIFactory.Stretch(root);
            AppUIFactory.AddVerticalLayout(root.gameObject, 0f, new RectOffset(0, 0, 0, 0), TextAnchor.UpperLeft);

            if (data.IsSpectator())
            {
                BuildSpectatorRoom();
            }
            else
            {
                BuildPlayerRoom();
                BuildExitModal(parent);
            }
        }

        private void BuildPlayerRoom()
        {
            BuildPlayerTopBar();
            GameRoomPlayerViews.BuildTrainingOpponentPanel(root, data);
            BuildBoardSlot("PlayerBoardSlot", 500f, false, false);
            GameRoomPlayerViews.BuildTrainingPlayerPanel(root, data);
            GameRoomOverlayViews.BuildStatusBanner(root, data.statusText);
            GameRoomActionBarView.BuildPlayerActions(root, data, HandleAction);
        }

        private void BuildSpectatorRoom()
        {
            BuildSpectatorTopBar();
            GameRoomPlayerViews.BuildSpectatorVersusBar(root, data);
            BuildBoardSlot("SpectatorBoardSlot", 566f, true, true);
            GameRoomActionBarView.BuildSpectatorActions(root, data, HandleAction);
        }

        private void BuildPlayerTopBar()
        {
            var topBar = GameRoomUi.CreatePanel("PlayerTopBar", root, GameRoomStyle.Alpha(GameRoomStyle.WalnutDark, 0.96f));
            AppUIFactory.AddLayoutElement(topBar.gameObject, 48f);
            AppUIFactory.AddHorizontalLayout(topBar.gameObject, 0f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            BuildBackButton(topBar.transform, ConfirmExit);

            var title = GameRoomUi.CreateLabel(
                "Title",
                topBar.transform,
                string.IsNullOrEmpty(data.title) ? "对局" : data.title,
                16f,
                GameRoomStyle.Gold,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            var titleLayout = AppUIFactory.AddLayoutElement(title.gameObject, 48f);
            titleLayout.flexibleWidth = 1f;
            title.maxVisibleLines = 1;

            var spacer = AppUIFactory.CreateRect("RightSpacer", topBar.transform);
            AppUIFactory.AddLayoutElement(spacer.gameObject, 48f, 56f);
        }

        private void BuildSpectatorTopBar()
        {
            var topBar = AppUIFactory.CreateRect("SpectatorTopBar", root);
            AppUIFactory.AddLayoutElement(topBar.gameObject, 64f);
            AppUIFactory.AddHorizontalLayout(topBar.gameObject, 0f, new RectOffset(0, 12, 10, 6), TextAnchor.MiddleLeft);

            BuildBackButton(topBar, () => onBack?.Invoke());

            var spacer = AppUIFactory.CreateRect("Spacer", topBar);
            var spacerLayout = AppUIFactory.AddLayoutElement(spacer.gameObject, 44f);
            spacerLayout.flexibleWidth = 1f;

            var viewerPill = GameRoomUi.CreatePanel("ViewerPill", topBar, GameRoomStyle.Alpha(Color.black, 0.2f));
            AppUIFactory.AddLayoutElement(viewerPill.gameObject, 32f, 76f);
            AppUIFactory.AddHorizontalLayout(viewerPill.gameObject, 4f, new RectOffset(10, 10, 0, 0), TextAnchor.MiddleCenter);

            var icon = GameRoomUi.CreateLabel(
                "Icon",
                viewerPill.transform,
                "◉",
                13f,
                GameRoomStyle.Alpha(Color.white, 0.72f),
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(icon.gameObject, 24f, 18f);

            var count = GameRoomUi.CreateLabel(
                "Count",
                viewerPill.transform,
                data.viewerCount.ToString(),
                14f,
                GameRoomStyle.Alpha(Color.white, 0.72f),
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(count.gameObject, 24f, 30f);
        }

        private void BuildBackButton(Transform parent, Action handler)
        {
            var back = AppUIFactory.CreateButton("BackButton", parent, GameRoomStyle.Alpha(GameRoomStyle.WalnutDark, 0f), () => handler?.Invoke());
            AppUIFactory.AddLayoutElement(back.gameObject, 48f, 56f);
            var icon = GameRoomUi.CreateLabel(
                "Icon",
                back.transform,
                "‹",
                34f,
                GameRoomStyle.Alpha(Color.white, 0.75f),
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(icon.rectTransform);
        }

        private void BuildBoardSlot(string name, float height, bool darkBoard, bool spectator)
        {
            var slot = AppUIFactory.CreateRect(name, root);
            AppUIFactory.AddLayoutElement(slot.gameObject, height);

            var boardView = slot.gameObject.AddComponent<ChessBoardView>();
            boardView.Build(slot, data, darkBoard);

            if (!spectator)
            {
                return;
            }

            GameRoomOverlayViews.BuildCapturedPieces(slot, data);
            GameRoomOverlayViews.BuildDanmaku(boardView.BoardRoot, data);
        }

        private void BuildExitModal(RectTransform parent)
        {
            exitModal = AppUIFactory.CreateRect("GameRoomExitModal", parent);
            AppUIFactory.Stretch(exitModal);
            exitModal.SetAsLastSibling();

            var overlay = AppUIFactory.CreateImage("Overlay", exitModal, GameRoomStyle.Alpha(Color.black, 0.6f));
            AppUIFactory.Stretch(overlay.rectTransform);

            var dialog = GameRoomUi.CreatePanel("Dialog", exitModal, GameRoomStyle.PanelLight);
            dialog.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            dialog.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            dialog.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            dialog.rectTransform.anchoredPosition = Vector2.zero;
            dialog.rectTransform.sizeDelta = new Vector2(280f, 150f);
            AppUIFactory.AddVerticalLayout(dialog.gameObject, 16f, new RectOffset(18, 18, 20, 16), TextAnchor.UpperCenter);

            var title = GameRoomUi.CreateLabel(
                "Title",
                dialog.transform,
                "对局还在进行中，确定离开？",
                18f,
                GameRoomStyle.BlackPiece,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(title.gameObject, 42f);

            var actions = AppUIFactory.CreateRect("Actions", dialog.transform);
            AppUIFactory.AddLayoutElement(actions.gameObject, 44f);
            AppUIFactory.AddHorizontalLayout(actions.gameObject, 12f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleCenter);

            var stay = GameRoomUi.CreateTextButton(
                "StayButton",
                actions,
                "继续对局",
                GameRoomStyle.WalnutDark,
                GameRoomStyle.Gold,
                14f,
                HideExitModal);
            var stayLayout = AppUIFactory.AddLayoutElement(stay.gameObject, 40f);
            stayLayout.flexibleWidth = 1f;

            var leave = GameRoomUi.CreateTextButton(
                "LeaveButton",
                actions,
                "确定离开",
                GameRoomStyle.PanelLight,
                GameRoomStyle.WalnutDark,
                14f,
                () => onBack?.Invoke());
            var leaveLayout = AppUIFactory.AddLayoutElement(leave.gameObject, 40f);
            leaveLayout.flexibleWidth = 1f;

            exitModal.gameObject.SetActive(false);
        }

        private void ConfirmExit()
        {
            if (exitModal == null)
            {
                onBack?.Invoke();
                return;
            }

            exitModal.gameObject.SetActive(true);
        }

        private void HideExitModal()
        {
            if (exitModal != null)
            {
                exitModal.gameObject.SetActive(false);
            }
        }

        private void HandleAction(GameRoomActionData action)
        {
            if (action == null)
            {
                return;
            }

            Debug.Log("GameRoom UI action: " + action.actionId);
        }
    }
}
