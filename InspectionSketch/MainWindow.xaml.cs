using InspectionSketch.Services;
using Microsoft.Win32;
using InspectionSketch.Drawing;
using InspectionSketch.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;


namespace InspectionSketch
{
    public partial class MainWindow : Window
    {
        private Point? lastPoint = null;
        private Sketch currentSketch = new Sketch();
        private readonly SnapEngine snapEngine = new SnapEngine();
        private readonly SketchFileService sketchFileService = new();
        private readonly SketchMeasurementService measurementService = new();
        private string? currentFilePath = null;
        private bool isLoadingReportInfo = false;
        private bool ConfirmDiscardUnsavedChanges()
        {
            if (!DrawingCanvas.HasUnsavedChanges)
                return true;

            MessageBoxResult result = MessageBox.Show(
                "You have unsaved changes. Would you like to save before continuing?",
                "Unsaved Changes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Cancel)
                return false;

            if (result == MessageBoxResult.No)
                return true;

            if (result == MessageBoxResult.Yes)
            {
                // If this sketch already has a file, save directly to it.
                if (!string.IsNullOrWhiteSpace(currentFilePath))
                {
                    sketchFileService.Save(
                        DrawingCanvas.CurrentSketch,
                        currentFilePath);

                    DrawingCanvas.MarkSaved();

                    return true;
                }

                // This sketch has never been saved, so ask where to save it.
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Save Inspection Sketch",
                    Filter = "Inspection Sketch (*.isketch)|*.isketch",
                    DefaultExt = ".isketch",
                    AddExtension = true
                };

                if (saveDialog.ShowDialog() != true)
                    return false;

                currentFilePath = saveDialog.FileName;

                sketchFileService.Save(
                    DrawingCanvas.CurrentSketch,
                    currentFilePath);

                DrawingCanvas.MarkSaved();

                return true;
            }

            return false;
        }
        private void UpdateSketchMeasurements()
        {
            double perimeterFeet =
                measurementService.CalculatePerimeterFeet(
                    DrawingCanvas.CurrentSketch);

            int wholeFeet = (int)System.Math.Floor(perimeterFeet);

            int inches = (int)System.Math.Round(
                (perimeterFeet - wholeFeet) * 12);

            if (inches == 12)
            {
                wholeFeet++;
                inches = 0;
            }

            PerimeterText.Text =
                $"{wholeFeet}'-{inches}\"";
           
            bool isClosed =
              measurementService.IsClosedOutline(
                   DrawingCanvas.CurrentSketch);

            OutlineStatusText.Text =
                isClosed ? "Closed" : "Open";
            
            double areaSquareFeet =
               measurementService.CalculateAreaSquareFeet(
                   DrawingCanvas.CurrentSketch);

            if (isClosed)
            {
                AreaText.Text =
                    $"{areaSquareFeet:F1} sq ft";
            }
            else
            {
                AreaText.Text = "—";
            }
        }
        private void UpdateWindowTitle()
        {
            string unsavedMarker =
                DrawingCanvas.HasUnsavedChanges ? " *" : "";

            if (string.IsNullOrWhiteSpace(currentFilePath))
            {
                Title = $"Inspection Sketch{unsavedMarker}";
                return;
            }

            string fileName =
                System.IO.Path.GetFileName(currentFilePath);

            Title =
                $"Inspection Sketch — {fileName}{unsavedMarker}";
        }
        public MainWindow()
        {
            InitializeComponent();


            DrawingCanvas.SelectedWallChanged += DrawingCanvas_SelectedWallChanged;
            DrawingCanvas.UnsavedChangesChanged += DrawingCanvas_UnsavedChangesChanged;
            PreviewKeyDown += MainWindow_PreviewKeyDown;

            PropertyAddressTextBox.TextChanged += ReportInfoTextBox_TextChanged;
            InspectorNameTextBox.TextChanged += ReportInfoTextBox_TextChanged;
            CustomerNameTextBox.TextChanged += ReportInfoTextBox_TextChanged;
            InspectionDatePicker.SelectedDateChanged += InspectionDatePicker_SelectedDateChanged;
            NotesTextBox.TextChanged += ReportInfoTextBox_TextChanged;
        }

        private void ReportInfoTextBox_TextChanged(
    object sender,
    TextChangedEventArgs e)
        {
            if (isLoadingReportInfo)
                return;

            DrawingCanvas.CurrentSketch.ReportInfo.CustomerName =
                CustomerNameTextBox.Text;
            
            DrawingCanvas.CurrentSketch.ReportInfo.PropertyAddress =
                PropertyAddressTextBox.Text;

            DrawingCanvas.CurrentSketch.ReportInfo.InspectorName =
                InspectorNameTextBox.Text;

            DrawingCanvas.CurrentSketch.ReportInfo.Notes =
               NotesTextBox.Text;

        }
        private void InspectionDatePicker_SelectedDateChanged(
          object? sender,
          SelectionChangedEventArgs e)
        {
            if (isLoadingReportInfo)
                return;

            if (InspectionDatePicker.SelectedDate.HasValue)
            {
                DrawingCanvas.CurrentSketch.ReportInfo.InspectionDate =
                    InspectionDatePicker.SelectedDate.Value;
            }
        }
        private void DrawingCanvas_UnsavedChangesChanged()
        {
            UpdateWindowTitle();
            UpdateSketchMeasurements();
        }
        private void SaveSketchButton_Click(
           object sender,
            RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(currentFilePath))
            {
                sketchFileService.Save(
                    DrawingCanvas.CurrentSketch,
                    currentFilePath);

                DrawingCanvas.MarkSaved();
                UpdateWindowTitle();

                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = "Save Inspection Sketch",
                Filter = "Inspection Sketch (*.isketch)|*.isketch",
                DefaultExt = ".isketch",
                AddExtension = true
            };

            if (saveDialog.ShowDialog() == true)
            {
                currentFilePath = saveDialog.FileName;

                sketchFileService.Save(
                    DrawingCanvas.CurrentSketch,
                    currentFilePath);

                DrawingCanvas.MarkSaved();
            }
        }
        private void SaveAsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = "Save Inspection Sketch As",
                Filter = "Inspection Sketch (*.isketch)|*.isketch",
                DefaultExt = ".isketch",
                AddExtension = true
            };

            if (!string.IsNullOrWhiteSpace(currentFilePath))
            {
                saveDialog.FileName =
                    System.IO.Path.GetFileName(currentFilePath);
            }

            if (saveDialog.ShowDialog() == true)
            {
                currentFilePath = saveDialog.FileName;

                sketchFileService.Save(
                    DrawingCanvas.CurrentSketch,
                    currentFilePath);

                DrawingCanvas.MarkSaved();
                UpdateWindowTitle();
            }
        }
        private void NewSketchButton_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (!ConfirmDiscardUnsavedChanges())
                return;

            currentFilePath = null;

            UpdateWindowTitle();

            DrawingCanvas.NewSketch();

            isLoadingReportInfo = true;

            InspectionDatePicker.SelectedDate =
                DrawingCanvas.CurrentSketch.ReportInfo.InspectionDate;

            CustomerNameTextBox.Text = "";
            PropertyAddressTextBox.Text = "";
            InspectorNameTextBox.Text = "";
            NotesTextBox.Text = "";

            isLoadingReportInfo = false;
        }
        private void OpenSketchButton_Click(
            object sender,
             RoutedEventArgs e)
        {
            if (!ConfirmDiscardUnsavedChanges())
                return;

            OpenFileDialog openDialog = new OpenFileDialog
            {
                Title = "Open Inspection Sketch",
                Filter = "Inspection Sketch (*.isketch)|*.isketch",
                DefaultExt = ".isketch"
            };

            if (openDialog.ShowDialog() == true)
            {
                Sketch? loadedSketch =
                    sketchFileService.Load(openDialog.FileName);

                if (loadedSketch != null)
                {
                    currentFilePath = openDialog.FileName;

                    DrawingCanvas.LoadSketch(loadedSketch);

                    isLoadingReportInfo = true;

                    InspectionDatePicker.SelectedDate =
                        DrawingCanvas.CurrentSketch.ReportInfo.InspectionDate;

                    CustomerNameTextBox.Text =
                        DrawingCanvas.CurrentSketch.ReportInfo.CustomerName;

                    PropertyAddressTextBox.Text =
                        DrawingCanvas.CurrentSketch.ReportInfo.PropertyAddress;

                    InspectorNameTextBox.Text =
                        DrawingCanvas.CurrentSketch.ReportInfo.InspectorName;

                    NotesTextBox.Text =
                        DrawingCanvas.CurrentSketch.ReportInfo.Notes;
                   
                    isLoadingReportInfo = false;

                    UpdateWindowTitle();
                }
            }
        }
        private void MainWindow_PreviewKeyDown(
           object sender,
           KeyEventArgs e)
        {
            if (e.Key == Key.Z &&
                Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                DrawingCanvas.Undo();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Y &&
                Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                DrawingCanvas.Redo();
                e.Handled = true;
                return;
            }
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
        private void UndoButton_Click(
                object sender,
                RoutedEventArgs e)
        {
            DrawingCanvas.Undo();
        }

        private void RedoButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DrawingCanvas.Redo();
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
        private void Window_Closing(
            object? sender,
            CancelEventArgs e)
        {
            if (!ConfirmDiscardUnsavedChanges())
            {
                e.Cancel = true;
            }
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
