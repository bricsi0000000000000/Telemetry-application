using NUnit.Framework;
using System;
using DataLayer.Tracks;

namespace Telemetry_unit_tests
{
    [TestFixture]
    class ReadTrackFileTest
    {
        [Test]
        [TestCase("../../../good_input_files/straight_track.json")]
        public void ReadFile(string fileName)
        {
            DriverlessTrackManager.LoadTrack(fileName);
        }

        [Test]
        [TestCase("../../../test_track_input_files/missing_curly_bracket.json")]
        [TestCase("../../../test_track_input_files/missing_track.json")]
        [TestCase("../../../test_track_input_files/missing_track1.json")]
        [TestCase("../../../test_track_input_files/empty.json")]
        [TestCase("../../../test_track_input_files/empty1.json")]
        [TestCase("../../../test_track_input_files/empty_center.json")]
        [TestCase("../../../test_track_input_files/empty_right_side.json")]
        [TestCase("../../../test_track_input_files/empty_left_side.json")]
        [TestCase("../../../test_track_input_files/empty_name.json")]
        [TestCase("../../../test_track_input_files/empty_width.json")]
        [TestCase("../../../test_track_input_files/empty_x_value.json")]
        [TestCase("../../../test_track_input_files/empty_y_value.json")]
        [TestCase("../../../test_track_input_files/null_center.json")]
        [TestCase("../../../test_track_input_files/null_right_side.json")]
        [TestCase("../../../test_track_input_files/null_left_side.json")]
        [TestCase("../../../test_track_input_files/null_name.json")]
        [TestCase("../../../test_track_input_files/null_width.json")]
        [TestCase("../../../test_track_input_files/null_x_value.json")]
        [TestCase("../../../test_track_input_files/null_y_value.json")]
        [TestCase("../../../test_track_input_files/string_length.json")]
        [TestCase("../../../test_track_input_files/string_length1.json")]
        [TestCase("../../../test_track_input_files/string_width.json")]
        [TestCase("../../../test_track_input_files/string_width1.json")]
        [TestCase("../../../test_track_input_files/string_x_value.json")]
        [TestCase("../../../test_track_input_files/string_x_value1.json")]
        [TestCase("../../../test_track_input_files/string_y_value.json")]
        [TestCase("../../../test_track_input_files/string_y_value1.json")]
        public void ProcessFile(string fileName)
        {
            Assert.Throws<Exception>(() => DriverlessTrackManager.LoadTrack(fileName));
        }
    }
}
