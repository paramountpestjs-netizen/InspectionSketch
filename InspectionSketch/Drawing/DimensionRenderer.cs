using System;
using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class DimensionRenderer
    {
        public void Draw(
    DrawingContext drawingContext,
    Point start,
    Point end,
    double feet)
        {
            double deltaX = end.X - start.X;
            double deltaY = end.Y - start.Y;

            double length = Math.Sqrt(
                deltaX * deltaX +
                deltaY * deltaY);

            if (length <= 0)
                return;

            double normalX = -deltaY / length;
            double normalY = deltaX / length;

            double offsetDistance = 50;

            Point dimensionStart = new Point(
                start.X + normalX * offsetDistance,
                start.Y + normalY * offsetDistance);

            Point dimensionEnd = new Point(
                end.X + normalX * offsetDistance,
                end.Y + normalY * offsetDistance);

            Pen dimensionPen = new Pen(Brushes.Black, 1);

            double extensionGap = 8;
            double extensionPast = 10;

            Point extensionStart1 = new Point(
                start.X + normalX * extensionGap,
                start.Y + normalY * extensionGap);

            Point extensionEnd1 = new Point(
                dimensionStart.X + normalX * extensionPast,
                dimensionStart.Y + normalY * extensionPast);

            Point extensionStart2 = new Point(
                end.X + normalX * extensionGap,
                end.Y + normalY * extensionGap);

            Point extensionEnd2 = new Point(
                dimensionEnd.X + normalX * extensionPast,
                dimensionEnd.Y + normalY * extensionPast);

            drawingContext.DrawLine(
                dimensionPen,
                extensionStart1,
                extensionEnd1);

            drawingContext.DrawLine(
                dimensionPen,
                extensionStart2,
                extensionEnd2);


            Point midpoint = new Point(
                (dimensionStart.X + dimensionEnd.X) / 2,
                (dimensionStart.Y + dimensionEnd.Y) / 2);

            FormattedText measurementText = new FormattedText(
                $"{feet:F1} ft",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                14,
                Brushes.Black,
                1.0);
            double textPadding = 8;

            double unitX = deltaX / length;
            double unitY = deltaY / length;

            double gapLength =
              Math.Abs(unitX) * measurementText.Width +
              Math.Abs(unitY) * measurementText.Height +
              textPadding * 2;

            Point gapStart = new Point(
                midpoint.X - unitX * gapLength / 2,
                midpoint.Y - unitY * gapLength / 2);

            Point gapEnd = new Point(
                midpoint.X + unitX * gapLength / 2,
                midpoint.Y + unitY * gapLength / 2);

            drawingContext.DrawLine(
                dimensionPen,
                dimensionStart,
                gapStart);

            drawingContext.DrawLine(
                dimensionPen,
                gapEnd,
                dimensionEnd);

            double tickSize = 6;

            Point tickStart1 = new Point(
                dimensionStart.X - tickSize,
                dimensionStart.Y + tickSize);

            Point tickEnd1 = new Point(
                dimensionStart.X + tickSize,
                dimensionStart.Y - tickSize);

            Point tickStart2 = new Point(
                dimensionEnd.X - tickSize,
                dimensionEnd.Y + tickSize);

            Point tickEnd2 = new Point(
                dimensionEnd.X + tickSize,
                dimensionEnd.Y - tickSize);

            drawingContext.DrawLine(
                dimensionPen,
                tickStart1,
                tickEnd1);

            drawingContext.DrawLine(
                dimensionPen,
                tickStart2,
                tickEnd2);

            drawingContext.DrawText(
                measurementText,
                new Point(
                    midpoint.X - measurementText.Width / 2,
                    midpoint.Y - measurementText.Height / 2));
        }
    }
    }