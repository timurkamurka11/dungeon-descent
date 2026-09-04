using System;
using System.Collections.Generic;

namespace DungeonDescent.World
{
    public sealed class RunLayout
    {
        public IReadOnlyList<string> RoomIds => roomIds;
        private readonly List<string> roomIds;
        private RunLayout(List<string> ids) { roomIds = ids; }

        public static RunLayout Generate(int seed)
        {
            var rng = new Random(seed);
            var ids = new List<string>
            {
                "safe-room", "descent", "floor-1-old-catacombs"
            };
            ids.Add(rng.Next(0, 2) == 0 ? "f1-sarcophagus-gallery" : "f1-broken-chapel");
            ids.Add("floor-2-flooded-depths");
            ids.Add(rng.Next(0, 2) == 0 ? "f2-drowned-crossing" : "f2-cistern");
            ids.Add("floor-3-forgotten-temple");
            ids.Add("elite-ancient-guard");
            ids.Add("boss-crypt-warden");
            ids.Add("reward-extraction");
            return new RunLayout(ids);
        }
    }
}
