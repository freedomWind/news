using System.Collections.Generic;


namespace NewsFramework.Simulation
{
    public sealed class GameTickInput
    {
        public int tick;
        public float deltaTime;
        public List<GameCommandData> commands = new List<GameCommandData>();
    }
}
