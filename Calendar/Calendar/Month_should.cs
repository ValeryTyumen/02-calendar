using System;
using NUnit.Framework;

namespace Calendar
{
    [TestFixture]
    class Month_should
    {
        private Month Month;

        [SetUp]
        public void SetUpMonth()
        {
            var day = new DateTime(2014, 11, 30);
            Month = new Month(day);
        }

        [Test]
        public void determine_first_week()
        {
            Assert.AreEqual(44, Month.FirstWeek);
        }

        [Test]
        public void determine_current_day()
        {
            Assert.AreEqual(Tuple.Create(5, 0), Month.CurrentDay);
        }

        [Test]
        public void determine_first_day()
        {
            Assert.AreEqual(1, Month.Weeks[0][6]);
        }

        [Test]
        public void determine_last_day()
        {
            Assert.AreEqual(30, Month.Weeks[5][0]);
        }
    }
}
