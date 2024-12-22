using UnityEngine;

namespace Resources
{
    public class DealMeTest : MonoBehaviour
    {
        public TileTrackerClient tileTracker;

        public void Deal()
        {
            for (int tileId = 0; tileId < 143; tileId += 11)
            {
                tileTracker.MoveTile(tileId, CLoc.LocalPrivateRack);
            }
        }
    }
}
