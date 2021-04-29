using NUnit.Framework;
using System;
using PresentationLayer.Tracks;

namespace UnitTest
{
    [TestFixture]
    public partial class ReadTrackFileTest
    {
        [Test]
        [TestCase(TEST_TRACKS_PATH + "missing_curly_bracket.json")]
        [TestCase(TEST_TRACKS_PATH + "missing_track.json")]
        [TestCase(TEST_TRACKS_PATH + "missing_track1.json")]
        [TestCase(TEST_TRACKS_PATH + "empty.json")]
        [TestCase(TEST_TRACKS_PATH + "empty1.json")]
        [TestCase(TEST_TRACKS_PATH + "empty_center.json")]
        [TestCase(TEST_TRACKS_PATH + "empty_right_side.json")]
        [TestCase(TEST_TRACKS_PATH + "empty_left_side.json")]
        [TestCase(TEST_TRACKS_PATH + "empty_name.json")]
        [TestCase(TEST_TRACKS_PATH + "empty_width.json")]
        [TestCase(TEST_TRACKS_PATH + "empty_x_value.json")]
        [TestCase(TEST_TRACKS_PATH + "empty_y_value.json")]
        [TestCase(TEST_TRACKS_PATH + "null_center.json")]
        [TestCase(TEST_TRACKS_PATH + "null_right_side.json")]
        [TestCase(TEST_TRACKS_PATH + "null_left_side.json")]
        [TestCase(TEST_TRACKS_PATH + "null_name.json")]
        [TestCase(TEST_TRACKS_PATH + "null_width.json")]
        [TestCase(TEST_TRACKS_PATH + "null_x_value.json")]
        [TestCase(TEST_TRACKS_PATH + "null_y_value.json")]
        [TestCase(TEST_TRACKS_PATH + "string_length.json")]
        [TestCase(TEST_TRACKS_PATH + "string_length1.json")]
        [TestCase(TEST_TRACKS_PATH + "string_width.json")]
        [TestCase(TEST_TRACKS_PATH + "string_width1.json")]
        [TestCase(TEST_TRACKS_PATH + "string_x_value.json")]
        [TestCase(TEST_TRACKS_PATH + "string_x_value1.json")]
        [TestCase(TEST_TRACKS_PATH + "string_y_value.json")]
        [TestCase(TEST_TRACKS_PATH + "string_y_value1.json")]
        public void ProcessFile(string fileName)
        {
            Assert.Throws<Exception>(() => DriverlessTrackManager.LoadTrack(fileName));
        }
    }
}
