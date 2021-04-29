using NUnit.Framework;
using System;
using LogicLayer;

namespace UnitTest
{
    [TestFixture]
    public partial class ReadFileTest
    {
        [Test]
        [TestCase(TEST_INPUT_FILES_PATH + "missing_column_name.csv")]
        [TestCase(TEST_INPUT_FILES_PATH + "not_a_number.csv")]
        [TestCase(TEST_INPUT_FILES_PATH + "empty_file.csv")]
        [TestCase(TEST_INPUT_FILES_PATH + "json.csv")]
        public void ProcessFile(string fileName)
        {
            DataReader reader = new DataReader();
            Assert.Throws<Exception>(() => reader.ProcessFile(fileName));
        }
    }
}
