using NUnit.Framework;
using System;
using Telemetry_presentation_layer;

namespace Telemetry_unit_tests
{
    [TestFixture]
    public class ReadFileTest
    {
        [Test]
        [TestCase("../../../test_input_files/all_good.csv")]
        [TestCase("../../../test_input_files/not_equals_row_lengths.csv")] //TODO: Handle not matching column sizes!
        public void ReadFile(string fileName)
        {
            DataReader reader = new DataReader();
            reader.ProcessFile(fileName);
        }

        [Test]
        [TestCase("../../../test_input_files/missing_column_name.csv")]
        [TestCase("../../../test_input_files/not_a_number.csv")]
        [TestCase("../../../test_input_files/empty_file.csv")]
        [TestCase("../../../test_input_files/json.csv")]
        public void ProcessFile(string fileName)
        {
            DataReader reader = new DataReader();
            Assert.Throws<Exception>(() => reader.ProcessFile(fileName));
        }
    }
}
