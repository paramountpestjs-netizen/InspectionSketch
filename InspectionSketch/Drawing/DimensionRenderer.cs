using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace InspectionSketch.Drawing
{
    public class DimensionRenderer
    {
        public Rect Draw(
    DrawingContext drawingContext,
    Point start,
    Point end,
    double feet,
    double normalDirection,
    List<Rect> occupiedDimensionBounds)
        {
            double deltaX = end.X - start.X;
            double deltaY = end.Y - start.Y;

            double length = Math.Sqrt(
                deltaX * deltaX +
                deltaY * deltaY);

            if (length <= 0)
                return Rect.Empty;

            double normalX = (-deltaY / length) * normalDirection;
            double normalY = (deltaX / length) * normalDirection;

            FormattedText measurementText = new FormattedText(
                FormatFeetAndInches(feet),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                14,
                Brushes.Black,
                1.0);

            double offsetDistance = 50;
            double laneSpacing = 30;

            Rect finalDimensionBounds = Rect.Empty;

            while (true)
            {
                Point testDimensionStart = new Point(
                    start.X + normalX * offsetDistance,
                    start.Y + normalY * offsetDistance);

                Point testDimensionEnd = new Point(
                    end.X + normalX * offsetDistance,
                    end.Y + normalY * offsetDistance);

                double minX = Math.Min(
                    testDimensionStart.X,
                    testDimensionEnd.X);

                double maxX = Math.Max(
                    testDimensionStart.X,
                    testDimensionEnd.X);

                double minY = Math.Min(
                    testDimensionStart.Y,
                    testDimensionEnd.Y);

                double maxY = Math.Max(
                    testDimensionStart.Y,
                    testDimensionEnd.Y);

                double collisionPadding = 20;

                Rect testBounds = new Rect(
                    minX - collisionPadding,
                    minY - collisionPadding,
                    Math.Max(
                        maxX - minX,
                        measurementText.Width) + collisionPadding * 2,
                    Math.Max(
                        maxY - minY,
                        measurementText.Height) + collisionPadding * 2);

                bool overlaps = false;

                foreach (Rect occupied in occupiedDimensionBounds)
                {
                    if (testBounds.IntersectsWith(occupied))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    finalDimensionBounds = testBounds;
                    break;
                }

                offsetDistance += laneSpacing;
            }

            Point dimensionStart = new Point(
                start.X + normalX * offsetDistance,
                start.Y + normalY * offsetDistance);

            Point dimensionEnd = new Point(
                end.X + normalX * offsetDistance,
                end.Y + normalY * offsetDistance);

            Pen dimensionPen = new Pen(Brushes.Black, 1);

            

            Point midpoint = new Point(
                (dimensionStart.X + dimensionEnd.X) / 2,
                (dimensionStart.Y + dimensionEnd.Y) / 2);

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

            return finalDimensionBounds;
        }
        private string FormatFeetAndInches(double feet)
        {
            int wholeFeet = (int)Math.Floor(feet);

            int inches = (int)Math.Round(
                (feet - wholeFeet) * 12);

            if (inches == 12)
            {
                wholeFeet++;
                inches = 0;
            }

            return $"{wholeFeet}'-{inches}\"";
        }
    }
    }