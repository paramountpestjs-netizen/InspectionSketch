using InspectionSketch.Drawing;
using InspectionSketch.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace InspectionSketch
{
    public partial class MainWindow : Window
    {
        private Point? lastPoint = null;
        private Sketch currentSketch = new Sketch();
        private readonly SnapEngine snapEngine = new SnapEngine();
        public MainWindow()
        {
            InitializeComponent();

            DrawingCanvas.SelectedWallChanged += DrawingCanvas_SelectedWallChanged;
        }
        private void DrawingCanvas_SelectedWallChanged(Wall? wall)
        {
            if (wall == null)
            {
                SelectedWallLengthText.Text = "No wall selected";
                EditWallLengthButton.IsEnabled = false;
                DeleteWallButton.IsEnabled = false;
                return;
            }

            double feet =
                wall.Length / 25.0;

            int wholeFeet =
                (int)System.Math.Floor(feet);

            int inches =
                (int)System.Math.Round(
                    (feet - wholeFeet) * 12);

            if (inches == 12)
            {
                wholeFeet++;
                inches = 0;
            }

            SelectedWallLengthText.Text =
                $"{wholeFeet}'-{inches}\"";

            EditWallLengthButton.IsEnabled = true;
            DeleteWallButton.IsEnabled = true;
        }
        private void EditWallLengthButton_Click(
             object sender,
             RoutedEventArgs e)
        {
            DrawingCanvas.EditSelectedWallLength();
        }
        private void DeleteWallButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DrawingCanvas.DeleteSelectedWall();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // The SketchCanvas now draws its own grid.
        }


        private void DrawGrid()
        {
            double spacing = 25;

            for (double x = 0; x < ActualWidth; x += spacing)
            {
                Line line = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = 800,
                    Stroke = Brushes.DimGray,
                    StrokeThickness = 0.5
                };

                DrawingCanvas.Children.Add(line);
            }


            for (double y = 0; y < ActualHeight; y += spacing)
            {
                Line line = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = 1200,
                    Y2 = y,
                    Stroke = Brushes.DimGray,
                    StrokeThickness = 0.5
                };

                DrawingCanvas.Children.Add(line);
            }
        }
        private Point SnapToGrid(Point point)
        {
            snapEngine.GridSpacing = currentSketch.Settings.GridSpacing;
            return snapEngine.SnapToGrid(point);
        }

        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }


        private void DrawPoint(Point point)
        {
            Ellipse dot = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(dot, point.X - 5);
            Canvas.SetTop(dot, point.Y - 5);

            DrawingCanvas.Children.Add(dot);
        }


        private void DrawWall(Point start, Point end)
        {
            Wall newWall = new Wall
            {
                StartPoint = start,
                EndPoint = end
            };

            currentSketch.Walls.Add(newWall);
            

            Line wallLine = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = Brushes.White,
                StrokeThickness = 3
            };

            DrawingCanvas.Children.Add(wallLine);
            TextBlock lengthText = new TextBlock
            {
                Text = $"Length: {(newWall.Length / currentSketch.Settings.PixelsPerFoot):F1} ft",
                Foreground = Brushes.Yellow,
                FontSize = 14
            };

            Canvas.SetLeft(lengthText, (start.X + end.X) / 2);
            Canvas.SetTop(lengthText, (start.Y + end.Y) / 2);

            DrawingCanvas.Children.Add(lengthText);
        }
    }
    }
