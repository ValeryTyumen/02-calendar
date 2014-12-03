using System;
using System.Globalization;
using System.Drawing;
using System.Linq;

namespace Calendar
{
    internal class Program
    {
        private static void Main(string[] args)
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
                    new CalendarPainter(bitmap.Size, graphics, new Month(datetime)).Draw();
                }
                bitmap.Save(bitmapName);
            }
        }
    }

    internal class Month
    {
        public string Title { get; private set; }
        public Tuple<string, string[]>[] Weeks { get; private set; }
        public int CurrentWeekIndex { get; private set; }
        public int CurrentDayOfWeek { get; private set; }
        private DateTime Day;
        
        public Month(DateTime day)
        {
            Title = day.ToString("MMMM yyyy");
            Day = day;
            CurrentDayOfWeek = (int)Day.DayOfWeek;
            var firstWeek = GetWeekOfYear(new DateTime(day.Year, day.Month, 1));
            CurrentWeekIndex = GetWeekOfYear(Day) - firstWeek;
            ArrangeWeeks();
        }

        private void ArrangeWeeks()
        {
            var day = new DateTime(1, 1, 1);
            day = day.AddDays(7 - (int) day.DayOfWeek);
            var weekTitles = Enumerable.Range(0, 7)
                .Select(i => day.AddDays(i).ToString("ddd").ToUpper())
                .ToArray();
            var weeksFirstElement = new Tuple<string, string[]>[]
            {
                Tuple.Create("#", weekTitles)
            };
            Weeks = weeksFirstElement.Concat(
                Enumerable.Range(1, DateTime.DaysInMonth(Day.Year, Day.Month))
                    .Select(z => new DateTime(Day.Year, Day.Month, z))
                    .GroupBy(GetWeekOfYear)
                    .Select(z => Tuple.Create(z.Key, z
                        .Select(x => x.ToString("dd"))
                        .ToArray()))
                    .OrderBy(z => z.Item1)
                    .Select(z => Tuple.Create(z.Item1.ToString(), z.Item2))
                ).ToArray();
        }

        private int GetWeekOfYear(DateTime day)
        {
            var firstDayOfYear = new DateTime(day.Year, 1, 1);
            return (day.DayOfYear + (int)firstDayOfYear.DayOfWeek - 1) / 7 + 1;
        }
    }

    internal class CalendarPainter
    {
        private Size ContextSize;
        private const string FontName = "Segoe WP";
        private Graphics Graphics;
        private Month Month; 

        public CalendarPainter(Size contextSize, Graphics graphics, Month month)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            ContextSize = contextSize;
            Graphics = graphics;
            Month = month;
        }

        public PointF MapGrid(PointF locationOnGrid)
        {
            return new PointF((locationOnGrid.X + 0.5f)*ContextSize.Width/8f, 
                (locationOnGrid.Y + 0.5f)*ContextSize.Height/8f);
        }

        private void DrawText(string text, Font font, Color color, PointF locationOnGrid)
        {
            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            Graphics.DrawString(text, font, new SolidBrush(color), MapGrid(locationOnGrid), stringFormat);
        }

        private void SelectDay()
        {
            var center = MapGrid(new PointF(Month.CurrentDayOfWeek + 1, Month.CurrentWeekIndex + 2));
            var radius = Math.Min(ContextSize.Width, ContextSize.Height) /16f;
            var brush = new SolidBrush(Color.PowderBlue);
            Graphics.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }

        private void DrawTitle()
        {
            var location = new PointF(3.5f, 0f);
            var font = new Font(FontName, 20, FontStyle.Bold);
            DrawText(Month.Title, font, Color.Gray, location);
        }

        private void DrawWeekNumbers()
        {
            var gridY = 1;
            foreach(var weekNumber in Month.Weeks.Select(z => z.Item1))
            {
                var locationOnGrid = new PointF(0, gridY);
                gridY++;
                var font = new Font(FontName, 30);
                DrawText(weekNumber.ToString(), font, Color.DodgerBlue, locationOnGrid);
            }
        }

        private void DrawDays()
        {
            for (var i = 0; i < Month.Weeks.Length; i++)
            {
                var start = 0;
                if (i == 1)
                    start = 7 - Month.Weeks[i].Item2.Length;
                for (var j = 0; j < Month.Weeks[i].Item2.Length; j++)
                {
                    var font = new Font(FontName, 30);
                    var color = (j + start == 0) ? Color.LightCoral : Color.Gray;
                    var locationOnGrid = new PointF(1 + j + start, 1 + i);
                    DrawText(Month.Weeks[i].Item2[j], font, color, locationOnGrid);
                }
            }
        }

        public void Draw()
        {
            var brush = new SolidBrush(Color.WhiteSmoke);
            Graphics.FillRectangle(brush, 0, 0, ContextSize.Width, ContextSize.Height);
            DrawTitle();
            SelectDay();
            DrawDays();
            DrawWeekNumbers();
        }
    }
}
