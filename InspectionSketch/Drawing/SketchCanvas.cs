using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using InspectionSketch.Models;


namespace InspectionSketch.Drawing
{
    public class SketchCanvas : System.Windows.Controls.Canvas
    {
        private readonly Camera camera = new Camera();
        private readonly DrawingRenderer drawingRenderer = new();
        private readonly SketchRenderer sketchRenderer = new();
        private readonly PreviewRenderer previewRenderer = new();
        private bool hasUnsavedChanges = false;
        private bool isPanning = false;
        private Point lastPanPoint;
        private Point? wallStartPoint = null;
        private Point? wallPreviewPoint = null;
        private readonly Sketch sketch = new();
        private Wall? selectedWall = null;

        private readonly Stack<(Action Undo, Action Redo)> undoStack = new();
        private readonly Stack<(Action Undo, Action Redo)> redoStack = new();

        public event Action<Wall?>? SelectedWallChanged;
        public Sketch CurrentSketch => sketch;
        public bool HasUnsavedChanges => hasUnsavedChanges;
        public void MarkSaved()
        {
            hasUnsavedChanges = false;
        }
        public void LoadSketch(Sketch loadedSketch)
        {
            sketch.Walls.Clear();

            foreach (Wall wall in loadedSketch.Walls)
            {
                sketch.Walls.Add(wall);
            }

            sketch.Settings = loadedSketch.Settings;

            selectedWall = null;

            undoStack.Clear();
            redoStack.Clear();

            SelectedWallChanged?.Invoke(null);
            hasUnsavedChanges = false;
            InvalidateVisual();
        }
        public void NewSketch()
        {
            sketch.Walls.Clear();
            sketch.Rooms.Clear();

            selectedWall = null;
            wallStartPoint = null;
            wallPreviewPoint = null;

            undoStack.Clear();
            redoStack.Clear();

            camera.Reset();

            SelectedWallChanged?.Invoke(null);
            hasUnsavedChanges = false;
            InvalidateVisual();
        }
        public SketchCanvas()
        {
            ClipToBounds = true;
            Focusable = true;
            Cursor = System.Windows.Input.Cursors.Cross;

            drawingRenderer.GridSpacing = DrawingRenderer.DefaultGridSpacing;

            MouseWheel += SketchCanvas_MouseWheel;

            MouseDown += SketchCanvas_MouseDown;
            MouseMove += SketchCanvas_MouseMove;
            MouseUp += SketchCanvas_MouseUp;
            MouseRightButtonDown += SketchCanvas_MouseRightButtonDown;
            KeyDown += SketchCanvas_KeyDown;
        }





        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingRenderer.Zoom = camera.Zoom;
            drawingRenderer.Offset = camera.Offset;
            drawingRenderer.Draw(drawingContext, RenderSize);

            sketchRenderer.Draw(
               drawingContext,
               sketch,
               camera,
               selectedWall);

            if (wallStartPoint != null && wallPreviewPoint != null)
            {
                Point previewStart = new Point(
                    camera.Offset.X + wallStartPoint.Value.X * camera.Zoom,
                    camera.Offset.Y + wallStartPoint.Value.Y * camera.Zoom);

                Point previewEnd = new Point(
                    camera.Offset.X + wallPreviewPoint.Value.X * camera.Zoom,
                    camera.Offset.Y + wallPreviewPoint.Value.Y * camera.Zoom);

                previewRenderer.Draw(
                    drawingContext,
                    previewStart,
                    previewEnd);
            }
        }
        private void SketchCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);

            if (e.Delta > 0)
            {
                camera.ZoomAt(mousePosition, 1.1);
            }
            else
            {
                camera.ZoomAt(mousePosition, 1 / 1.1);
            }

            InvalidateVisual();
        }
        private void SketchCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Input.Keyboard.Focus(this);

            if (e.ChangedButton == System.Windows.Input.MouseButton.Middle)
            {
                isPanning = true;
                lastPanPoint = e.GetPosition(this);
                CaptureMouse();
            }

            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                Point clickPoint = SnapToGrid(e.GetPosition(this));

                if (wallStartPoint == null)
                {
                    wallStartPoint = clickPoint;
                }
                else
                {
                    Wall newWall = new Wall
                    {
                        StartPoint = wallStartPoint.Value,
                        EndPoint = clickPoint
                    };

                    sketch.Walls.Add(newWall);
                    
                    hasUnsavedChanges = true;

                    undoStack.Push((
                        Undo: () =>
                        {
                            sketch.Walls.Remove(newWall);
                        },
                        Redo: () =>
                        {
                            sketch.Walls.Add(newWall);
                        }
                    ));

                    redoStack.Clear();

                    wallStartPoint = clickPoint;
                    wallPreviewPoint = clickPoint;

                    InvalidateVisual();
                }
            }
        }

        private void SketchCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isPanning)
            {
                Point currentPoint = e.GetPosition(this);

                Vector delta = currentPoint - lastPanPoint;

                camera.Pan(delta);

                lastPanPoint = currentPoint;

                InvalidateVisual();
            }
            if (wallStartPoint != null && !isPanning)
            {
                wallPreviewPoint = SnapToGrid(e.GetPosition(this));
                InvalidateVisual();
            }
        }

        private void SketchCanvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Middle)
            {
                isPanning = false;
                ReleaseMouseCapture();
            }
        }
        private Point SnapToGrid(Point point)
        {
            double worldX = (point.X - camera.Offset.X) / camera.Zoom;
            double worldY = (point.Y - camera.Offset.Y) / camera.Zoom;

            double spacing = DrawingRenderer.DefaultGridSpacing;

            double snappedX =
                Math.Round(worldX / spacing) * spacing;

            double snappedY =
                Math.Round(worldY / spacing) * spacing;

            return new Point(snappedX, snappedY);
        }
        private void SketchCanvas_MouseRightButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(this);

            selectedWall = FindWallAtPoint(clickPoint);
            
            SelectedWallChanged?.Invoke(selectedWall);

            InvalidateVisual();

            e.Handled = true;
        }
        private Wall? 
            FindWallAtPoint(Point screenPoint)
        {
            const double selectionTolerance = 8.0;

            foreach (var wall in sketch.Walls)
            {
                Point start = new Point(
                    camera.Offset.X + wall.StartPoint.X * camera.Zoom,
                    camera.Offset.Y + wall.StartPoint.Y * camera.Zoom);

                Point end = new Point(
                    camera.Offset.X + wall.EndPoint.X * camera.Zoom,
                    camera.Offset.Y + wall.EndPoint.Y * camera.Zoom);

                Vector wallVector = end - start;
                Vector pointVector = screenPoint - start;

                double wallLengthSquared = wallVector.LengthSquared;

                if (wallLengthSquared == 0)
                    continue;

                double t = Vector.Multiply(
                    pointVector,
                    wallVector) / wallLengthSquared;

                t = Math.Max(0, Math.Min(1, t));

                Point closestPoint = start + wallVector * t;

                double distance = (screenPoint - closestPoint).Length;

                if (distance <= selectionTolerance)
                    return wall;
            }

            return null;
        }
        private Wall? FindWallConnectedToEnd(Wall wall)
        {
            foreach (var otherWall in sketch.Walls)
            {
                if (otherWall == wall)
                    continue;

                if (otherWall.StartPoint == wall.EndPoint)
                    return otherWall;
            }

            return null;
        }
        private List<(Wall Wall, Point Start, Point End)> CaptureWallStates()
        {
            List<(Wall Wall, Point Start, Point End)> states = new();

            foreach (Wall wall in sketch.Walls)
            {
                states.Add((
                    wall,
                    wall.StartPoint,
                    wall.EndPoint));
            }

            return states;
        }

        private void RestoreWallStates(
            List<(Wall Wall, Point Start, Point End)> states)
        {
            foreach (var state in states)
            {
                state.Wall.StartPoint = state.Start;
                state.Wall.EndPoint = state.End;
            }

            InvalidateVisual();
        }
        private void SetWallLength(Wall wall, double newLengthInFeet)

        {
            double currentLength = wall.Length;

            if (currentLength <= 0)
                return;

            Wall? connectedWall = FindWallConnectedToEnd(wall);

            Wall? followingWall = null;

            if (connectedWall != null)
            {
                followingWall = FindWallConnectedToEnd(connectedWall);
            }

            double newLengthInPixels =
                newLengthInFeet * sketch.Settings.PixelsPerFoot;

            Vector direction = wall.EndPoint - wall.StartPoint;

            // Keep walls that are mostly horizontal or vertical perfectly straight.
            if (Math.Abs(direction.X) >= Math.Abs(direction.Y))
            {
                direction = new Vector(
                    Math.Sign(direction.X),
                    0);
            }
            else
            {
                direction = new Vector(
                    0,
                    Math.Sign(direction.Y));
            }

            Point oldEndPoint = wall.EndPoint;

            Point newEndPoint =
                wall.StartPoint +
                direction * newLengthInPixels;

            Vector movement = newEndPoint - oldEndPoint;

            wall.EndPoint = newEndPoint;

            if (connectedWall != null)
            {
                connectedWall.StartPoint =
                    connectedWall.StartPoint + movement;

                connectedWall.EndPoint =
                    connectedWall.EndPoint + movement;

                if (followingWall != null &&
                    followingWall != wall)
                {
                    followingWall.StartPoint =
                        connectedWall.EndPoint;
                }
            }

            InvalidateVisual();
        }
        public void DeleteSelectedWall()
        {
            if (selectedWall == null)
                return;

            Wall wallToDelete = selectedWall;

            int wallIndex = sketch.Walls.IndexOf(wallToDelete);

            sketch.Walls.Remove(wallToDelete);
           
            hasUnsavedChanges = true;

            undoStack.Push((
                Undo: () =>
                {
                    if (wallIndex >= 0 &&
                        wallIndex <= sketch.Walls.Count)
                    {
                        sketch.Walls.Insert(
                            wallIndex,
                            wallToDelete);
                    }
                    else
                    {
                        sketch.Walls.Add(wallToDelete);
                    }
                },
                Redo: () =>
                {
                    sketch.Walls.Remove(wallToDelete);
                }
            ));

            redoStack.Clear();

            selectedWall = null;

            SelectedWallChanged?.Invoke(null);

            InvalidateVisual();
        }
        public void EditSelectedWallLength()
{
    if (selectedWall == null)
        return;
           
            Wall wallBeingEdited = selectedWall;

            var beforeStates = CaptureWallStates();

            double currentFeet =
               wallBeingEdited.Length / sketch.Settings.PixelsPerFoot;

            int wholeFeet = (int)Math.Floor(currentFeet);

    int inches = (int)Math.Round(
        (currentFeet - wholeFeet) * 12);

    if (inches == 12)
    {
        wholeFeet++;
        inches = 0;
    }

    string input = Microsoft.VisualBasic.Interaction.InputBox(
        "Enter wall length (example: 10 6 for 10'-6\"):",
        "Set Wall Length",
        $"{wholeFeet} {inches}");

    string[] parts = input
        .Replace("'", " ")
        .Replace("\"", " ")
        .Replace("-", " ")
        .Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length >= 1 &&
        int.TryParse(parts[0], out int enteredFeet))
    {
        int enteredInches = 0;

        if (parts.Length >= 2)
        {
            int.TryParse(parts[1], out enteredInches);
        }

        if (enteredFeet >= 0 &&
            enteredInches >= 0 &&
            enteredInches < 12)
        {
            double newLengthInFeet =
                enteredFeet + enteredInches / 12.0;

            if (newLengthInFeet > 0)
            {
                        SetWallLength(
                           wallBeingEdited,
                           newLengthInFeet);
                           hasUnsavedChanges = true;
                        var afterStates = CaptureWallStates();

                        undoStack.Push((
                            Undo: () =>
                            {
                                RestoreWallStates(beforeStates);
                                SelectedWallChanged?.Invoke(wallBeingEdited);
                            },
                            Redo: () =>
                            {
                                RestoreWallStates(afterStates);
                                SelectedWallChanged?.Invoke(wallBeingEdited);
                            }
                        ));

                        redoStack.Clear();


                        SelectedWallChanged?.Invoke(wallBeingEdited);
                    }
        }
    }
}
        public void Undo()
        {
            if (undoStack.Count == 0)
                return;

            var action = undoStack.Pop();

            action.Undo();

            redoStack.Push(action);

            InvalidateVisual();
        }

        public void Redo()
        {
            if (redoStack.Count == 0)
                return;

            var action = redoStack.Pop();

            action.Redo();

            undoStack.Push(action);

            InvalidateVisual();
        }
        private void SketchCanvas_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
           
            if (e.Key == System.Windows.Input.Key.F)
            {
                camera.Reset();
                InvalidateVisual();
            }

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                wallStartPoint = null;
                wallPreviewPoint = null;
                InvalidateVisual();
            }
            if (e.Key == System.Windows.Input.Key.M && selectedWall != null)
            {
                EditSelectedWallLength();
            }
        }
    }
    }
