using NewsFramework.Data.Replay;

namespace NewsFramework.Data.Mock
{
    public static class ReplayMockData
    {
        public static ReplayData CreateArticlePreview()
        {
            var replay = new ReplayData
            {
                replayId = "replay_article_001",
                gameId = "xiangqi",
                initialState = "mock-fen",
                initialStateFormat = "fen",
                startStepIndex = 12,
                endStepIndex = 18,
                autoPlay = false,
                loop = false,
                secondsPerStep = 0.6f,
                renderProfile = new ReplayRenderProfileData
                {
                    mode = ReplayRenderModes.Canvas2D,
                    textureWidth = 1024,
                    textureHeight = 576,
                    cameraProfile = "article_preview",
                    background = "paper"
                }
            };

            replay.steps.Add(new ReplayStepData
            {
                index = 12,
                command = "move:cannon_2_5",
                notation = "炮二平五",
                comment = "红方抢占中路。"
            });

            replay.steps.Add(new ReplayStepData
            {
                index = 13,
                command = "move:horse_8_7",
                notation = "马8进7",
                comment = "黑方补强中防。"
            });

            replay.steps.Add(new ReplayStepData
            {
                index = 14,
                command = "move:horse_2_3",
                notation = "马二进三",
                comment = "红方继续出子。"
            });

            return replay;
        }
    }
}
