using System;
using System.Globalization;
using System.Drawing;

namespace Calendar
{
    class Program
    {
        static void Main(string[] args)
        {
            var datetime = DateTime.ParseExact(args[0], "dd.MM.yyyy", CultureInfo.InvariantCulture);
            var calendar = CultureInfo.InvariantCulture.Calendar;

            var bitmapName = "Calendar.bmp";
            if (args.Length > 1)
                bitmapName = args[1];

            using (var bitmap = new Bitmap(800, 600))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    new CalendarPainter(bitmap.Size, graphics, datetime).Draw();
                }
                bitmap.Save(bitmapName);
            }
        }
    }

    internal class CalendarPoint
    {
        private PointF point;
        public float X
        {
            get { return point.X; }
        }
        public float Y
        {
            get { return point.Y; }
        }

        public CalendarPoint(float x, float y)
        {
            point = new PointF(x, y);
        }
    }

    internal class CalendarPainter
    {
        private Size ContextSize;
        private const string FontName = "Segoe WP";
        private Graphics Graphics;
        private DateTime Day;

        public CalendarPainter(Size contextSize, Graphics graphics, DateTime day)
        {
            ContextSize = contextSize;
            Graphics = graphics;
            Day = day;
        }

        private PointF GetContextCoords(CalendarPoint point)
        {
            var x = ContextSize.Width*point.X/8;
            var y = ContextSize.Height*point.Y/8;
            return new PointF(x, y);
        }

        private void DrawText(string text, Font font, Color color, CalendarPoint cPoint)
        {
            var point = GetContextCoords(cPoint);
            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            Graphics.DrawString(text, font, new SolidBrush(color), point, stringFormat);
        }

        private int GetWeekOfYear(DateTime day)
        {
            var firstDayOfYear = new DateTime(day.Year, 1, 1);
            return (day.DayOfYear + (int)firstDayOfYear.DayOfWeek - 1)/7 + 1;
        }

        private DateTime GetFirstDayOfMonth(DateTime day)
        {
            var firstDay = new DateTime(day.Year, day.Month, 1);
            return firstDay;
        }

        private DateTime GetLastDayOfMonth(DateTime day)
        {
            var lastDay = new DateTime(day.Year, day.Month, DateTime.DaysInMonth(day.Year, day.Month));
            return lastDay;
        }

        private CalendarPoint GetCalendarPoint(DateTime day)
        {
            var firstDayOfMonth = new DateTime(day.Year, day.Month, 1);
            var firstWeek = GetWeekOfYear(firstDayOfMonth);
            var week = GetWeekOfYear(day);
            return new CalendarPoint((float)day.DayOfWeek + 1.5f, week - firstWeek + 2.5f);
        }

        private void DrawSelection(DateTime day)
        {
            var center = GetContextCoords(GetCalendarPoint(day));
            var radius = Math.Min(ContextSize.Width, ContextSize.Height) /16f;
            var brush = new SolidBrush(Color.PowderBlue);
            Graphics.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }

        private void DrawMonth(DateTime day)
        {
            var cPoint = new CalendarPoint(4f, 0.5f);
            var text = day.ToString("MMMM yyyy");
            var font = new Font(FontName, 20, FontStyle.Bold);
            DrawText(text, font, Color.Gray, cPoint);
        }

        private void DrawWeekNumbers(DateTime day)
        {
            var firstWeek = GetWeekOfYear(GetFirstDayOfMonth(day));
            var lastWeek = GetWeekOfYear(GetLastDayOfMonth(day));
            for (var i = firstWeek; i <= lastWeek; i++)
            {
                var cPoint = new CalendarPoint(0.5f, i - firstWeek + 2.5f);
                var font = new Font(FontName, 30);
                DrawText(i.ToString(), font, Color.DodgerBlue, cPoint);
            }
        }

        private void DrawWeeksTitle()
        {
            var day = new DateTime(1, 1, 1);
            day.AddDays(7 - (int)day.DayOfWeek);
            var cPoint = new CalendarPoint(0.5f, 1.5f);
            var font = new Font(FontName, 30);
            DrawText("#", font, Color.LightCoral, cPoint);
            for (var i = 0; i < 7; i++)
            {
                cPoint = new CalendarPoint(cPoint.X + 1f, cPoint.Y);
                DrawText(day.ToString("ddd").ToUpper(), font, Color.LightCoral, cPoint);
                day = day.AddDays(1);
            }
        }

        private void DrawDays(DateTime day)
        {
            for (var i = 1; i <= DateTime.DaysInMonth(day.Year, day.Month); i++)
            {
                var monthDay = new DateTime(day.Year, day.Month, i);
                CalendarPoint cPoint = GetCalendarPoint(monthDay);
                var color = monthDay.DayOfWeek == DayOfWeek.Sunday ? Color.LightCoral : Color.Gray;
                var font = new Font(FontName, 30);
                DrawText(monthDay.Day.ToString(), font, color, cPoint);
            }
        }

        public void Draw()
        {
            var brush = new SolidBrush(Color.WhiteSmoke);
            Graphics.FillRectangle(brush, 0, 0, ContextSize.Width, ContextSize.Height);
            DrawSelection(Day);
            DrawMonth(Day);
            DrawWeeksTitle();
            DrawWeekNumbers(Day);
            DrawDays(Day);
        }

    }
}
