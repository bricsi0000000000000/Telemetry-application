using NUnit.Framework;
using System;
using PresentationLayer.Tracks;

namespace UnitTest
{
    [TestFixture]
    public partial class ReadTrackFileTest
    {
        [Test]
        [TestCase(GOOD_TRACKS_PATH + "straight_track.json")]
        public void ReadFile(string fileName)
        {
            DriverlessTrackManager.LoadTrack(fileName);
        }
    }
}
