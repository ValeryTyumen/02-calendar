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
            Assert.AreEqual("44", Month.Weeks[1].Item1);
        }

        [Test]
        public void determine_current_day()
        {
            Assert.AreEqual(0, Month.CurrentDayOfWeek);
            Assert.AreEqual(5, Month.CurrentWeekIndex);
        }

        [Test]
        public void determine_first_day()
        {
            Assert.AreEqual("01", Month.Weeks[1].Item2[0]);
        }

        [Test]
        public void determine_last_day()
        {
            Assert.AreEqual("30", Month.Weeks[6].Item2[0]);
        }

        [Test]
        public void get_week_titles()
        {
            var expected_titles = new string[] {"SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"};
            CollectionAssert.AreEqual(expected_titles, Month.Weeks[0].Item2);
        }
    }
}
