using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace DrawingApp
{
    public partial class MainWindow : Window
    {
        private Polyline currentLine;
        private Brush currentColor = Brushes.Black;
        private double strokeThickness = 2;
        private string currentTool = "Pen";
        private List<UIElement> elements = new List<UIElement>(); 
        private int undoIndex = -1; 
        private string currentBrushType = "Pencil"; 

        public MainWindow()
        {
            InitializeComponent();
        }

       private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                currentLine = new Polyline
                {
                    Stroke = currentColor,
                    StrokeThickness = strokeThickness,
                    Points = new PointCollection { e.GetPosition(DrawCanvas) }
                };

                if (currentBrushType == "Pencil")
                {
                    
                    currentLine.StrokeThickness = 1;
                }
                else if (currentBrushType == "Brush")
                {
                   
                    currentLine.StrokeThickness = 5;
                }
                else if (currentBrushType == "Marker")
                {
                   
                    currentLine.StrokeThickness = 8;
                }

                DrawCanvas.Children.Add(currentLine);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentLine != null)
            {
                currentLine.Points.Add(e.GetPosition(DrawCanvas));
            }
        }

        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            DrawCanvas.Children.Clear();
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                currentColor = new SolidColorBrush(e.NewValue.Value);
            }
        }

        private void ThicknessComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThicknessComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                strokeThickness = Convert.ToDouble(selectedItem.Tag);
                ThicknessLabel.Content = $"Thickness: {strokeThickness}";
            }
        }

        private void BrushTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrushTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                currentBrushType = selectedItem.Tag.ToString();
            }
        }

      
        private void AddText_Click(object sender, RoutedEventArgs e)
        {
            var textBlock = new TextBlock
            {
                Text = "Text",
                Foreground = currentColor,
                FontSize = 20
            };
            Canvas.SetLeft(textBlock, 100);
            Canvas.SetTop(textBlock, 100);
            DrawCanvas.Children.Add(textBlock);
            elements.Add(textBlock);
        }

        private void DrawRectangle_Click(object sender, RoutedEventArgs e)
        {
            currentTool = "Rectangle";
        }

     
        private void AddToHistory()
        {
            if (undoIndex < elements.Count - 1)
            {
                elements.RemoveRange(undoIndex + 1, elements.Count - undoIndex - 1);
            }
            undoIndex++;
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (undoIndex >= 0)
            {
                var element = elements[undoIndex];
                DrawCanvas.Children.Remove(element);
                undoIndex--;
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (undoIndex < elements.Count - 1)
            {
                undoIndex++;
                var element = elements[undoIndex];
                DrawCanvas.Children.Add(element);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "PNG Image|*.png";
            if (dlg.ShowDialog() == true)
            {
                var filePath = dlg.FileName;
                var renderTargetBitmap = new RenderTargetBitmap((int)DrawCanvas.ActualWidth, (int)DrawCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(DrawCanvas);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                using (var stream = System.IO.File.OpenWrite(filePath))
                {
                    encoder.Save(stream);
                }
            }
        }
    }
}
