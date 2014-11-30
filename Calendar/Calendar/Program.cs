using System;
using System.Globalization;
using System.Drawing;
using System.Collections.Generic;
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
                    new CalendarPainter(bitmap.Size, graphics, datetime).Draw();
                }
                bitmap.Save(bitmapName);
            }
        }
    }

    internal class Month
    {
        public string Title { get; private set; }
        public List<int[]> Weeks { get; private set; }
        public Tuple<int, int> CurrentDay { get; private set; }
        public int FirstWeek { get; private set; }

        public Month(DateTime day)
        {
            Title = day.ToString("MMMM yyyy");
            FirstWeek = GetWeekOfYear(new DateTime(day.Year, day.Month, 1));
            CurrentDay = Tuple.Create(GetWeekOfYear(day) - FirstWeek, (int)day.DayOfWeek);
            Weeks = new List<int[]>();
            for (var i = 1; i <= DateTime.DaysInMonth(day.Year, day.Month); i++)
            {
                var dayOfMonth = new DateTime(day.Year, day.Month, i);
                AddDay(dayOfMonth);
            }
        }

        public IEnumerable<int> GetWeekNumbers()
        {
            return Enumerable.Range(0, Weeks.Count).Select(z => z + FirstWeek);
        }

        private void AddDay(DateTime day)
        {
            var week = GetWeekOfYear(day) - FirstWeek;
            if (week >= Weeks.Count)
                Weeks.Add(new int[7]);
            Weeks[week][(int) day.DayOfWeek] = day.Day;
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

        public CalendarPainter(Size contextSize, Graphics graphics, DateTime day)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            ContextSize = contextSize;
            Graphics = graphics;
            Month = new Month(day);
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

        private void SelectDay(Month month)
        {
            var center = MapGrid(new PointF(month.CurrentDay.Item2 + 1,month.CurrentDay.Item1 + 2));
            var radius = Math.Min(ContextSize.Width, ContextSize.Height) /16f;
            var brush = new SolidBrush(Color.PowderBlue);
            Graphics.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }

        private void DrawTitle(Month month)
        {
            var location = new PointF(3.5f, 0f);
            var font = new Font(FontName, 20, FontStyle.Bold);
            DrawText(month.Title, font, Color.Gray, location);
        }

        private void DrawWeekNumbers(Month month)
        {
            foreach(var weekNumber in month.GetWeekNumbers())
            {
                var locationOnGrid = new PointF(0, weekNumber - month.FirstWeek + 2);
                var font = new Font(FontName, 30);
                DrawText(weekNumber.ToString(), font, Color.DodgerBlue, locationOnGrid);
            }
        }

        private void DrawWeekTitles()
        {
            var day = new DateTime(1, 1, 1);
            day = day.AddDays(7 - (int)day.DayOfWeek);
            var font = new Font(FontName, 25);
            var locationOnGrid = new PointF(0, 1);
            DrawText("#", font, Color.Gray, locationOnGrid);
            for (var i = 0; i < 7; i++)
            {
                locationOnGrid = new PointF(i + 1, 1);
                DrawText(day.ToString("ddd").ToUpper(), font, Color.Gray, locationOnGrid);
                day = day.AddDays(1);
            }
        }

        private void DrawDays(Month month)
        {
            for (var i = 0; i < month.Weeks.Count; i++)
                for (var j = 0; j < 7; j++)
                    if (month.Weeks[i][j] != 0)
                    {
                        var locationOnGrid = new PointF(j + 1, i + 2);
                        var color = j == 0 ? Color.LightCoral : Color.Gray;
                        var font = new Font(FontName, 30);
                        DrawText(month.Weeks[i][j].ToString(), font, color, locationOnGrid);
                    }
        }

        public void Draw()
        {
            var brush = new SolidBrush(Color.WhiteSmoke);
            Graphics.FillRectangle(brush, 0, 0, ContextSize.Width, ContextSize.Height);
            DrawTitle(Month);
            DrawWeekTitles();
            SelectDay(Month);
            DrawDays(Month);
            DrawWeekNumbers(Month);
        }

    }
}
