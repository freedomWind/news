namespace NewsFramework.Simulation
{
    public static class GameSimulationFactory
    {
        public static IGameSimulation Create(GameSimulationConfig config)
        {
            var safeConfig = config ?? new GameSimulationConfig();
            return safeConfig.gameId switch
            {
                "xiangqi" => new ChessSimulation(),
                _ => new NullGameSimulation()
            };
        }
    }
}
