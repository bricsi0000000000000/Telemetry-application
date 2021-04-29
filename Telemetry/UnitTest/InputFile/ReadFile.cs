using NUnit.Framework;
using LogicLayer;

namespace UnitTest
{
    [TestFixture]
    public partial class ReadFileTest
    {
        [Test]
        [TestCase(TEST_INPUT_FILES_PATH + "all_good.csv")]
        [TestCase(TEST_INPUT_FILES_PATH + "not_equals_row_lengths.csv")] //TODO: Handle not matching column sizes!
        public void ReadFile(string fileName)
        {
            DataReader reader = new DataReader();
            Assert.DoesNotThrow(() => reader.ProcessFile(fileName));
        }
    }
}
