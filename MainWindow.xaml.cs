using Microsoft.Win32;
using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DrawingApp
{
    public partial class MainWindow : Window
    {
        private Polyline currentLine;
        private Brush currentColor = Brushes.Black;
        private double strokeThickness = 2;
        private byte currentOpacity = 128;
        private bool isErasing = false;
        private double currentZoom = 1.0;
        private Tool currentTool = Tool.Pencil;

        public MainWindow()
        {
            InitializeComponent();
            PlayStartupSound();
            this.Opacity = 0;
            Loaded += (s, e) => FadeIn();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await FadeIn();
        }
      
        private async Task FadeIn()
        {
            for (double i = 0; i <= 1; i += 0.008)
            {
                this.Opacity = i;
                await Task.Delay(20);
            }

            
            LoadingScreen.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;
        }


        private void PlayStartupSound()
        {
            try
            {
                  SoundPlayer player = new SoundPlayer("Assets\\oxp.wav");
                  player.Play();
            
            }
            catch { }
        }

        public enum Tool
        {
            Pencil,
            Marker,
            BristleBrush,
            Eraser,
            Fill
        }

        private void BrushSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrushSelectionComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                isErasing = false;
                switch (selectedItem.Content.ToString())
                {
                    case "✏ Pencil":
                        currentColor = new SolidColorBrush(ColorSelectionButton.SelectedColor ?? Colors.Black);
                        strokeThickness = 1;
                        break;
                    case "🖊 Marker":
                        currentColor = new SolidColorBrush(ColorSelectionButton.SelectedColor ?? Colors.Black);
                        strokeThickness = 5;
                        break;
                    case "🖌 Bristle Brush":
                        currentColor = Brushes.SaddleBrown;
                        strokeThickness = 3;
                        break;
                    case "🧽 Eraser":
                        isErasing = true;
                        currentColor = Brushes.White; 
                        strokeThickness = 10;
                        break;
                    case "🖍 Fill":
                        currentTool = Tool.Fill;
                        FillCanvas();
                        break;
                }
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
               
                if (isErasing)
                {
                    currentColor = Brushes.White;
                }

                currentLine = new Polyline
                {
                    Stroke = currentColor,
                    StrokeThickness = strokeThickness,
                    Points = new PointCollection { e.GetPosition(DrawCanvas) }
                };
                DrawCanvas.Children.Add(currentLine);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentLine != null)
            {
               
                if (isErasing)
                {
                    currentColor = Brushes.White;
                }

                currentLine.Points.Add(e.GetPosition(DrawCanvas));
            }
        }


        private void FillCanvas()
        {
            var fillColor = ColorSelectionButton.SelectedColor;

            if (!fillColor.HasValue)
            {
                MessageBox.Show("Please select a color first.");
                return;
            }

            var currentBackgroundColor = (DrawCanvas.Background as SolidColorBrush)?.Color;

            if (currentBackgroundColor != fillColor.Value)
            {
                Brush fillBrush = new SolidColorBrush(fillColor.Value); 
                DrawCanvas.Background = fillBrush;
            }
        }


        private void ThicknessSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThicknessSelectionComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "▫ Very Thin": strokeThickness = 0.5; break;
                    case "▪ Thin": strokeThickness = 2; break;
                    case "◼ Normal": strokeThickness = 4; break;
                    case "⬛ Thick": strokeThickness = 6; break;
                    case "🔲 Extra Thick": strokeThickness = 8; break;
                }
            }
        }

        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            DrawCanvas.Children.Clear();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "PNG Image|*.png";
            if (dlg.ShowDialog() == true)
            {
                var renderTargetBitmap = new RenderTargetBitmap((int)DrawCanvas.ActualWidth, (int)DrawCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(DrawCanvas);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                using (var stream = System.IO.File.OpenWrite(dlg.FileName))
                {
                    encoder.Save(stream);
                }
            }
        }

        private void ColorSelection_Click(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                currentColor = new SolidColorBrush(Color.FromArgb(currentOpacity, e.NewValue.Value.R, e.NewValue.Value.G, e.NewValue.Value.B));
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            currentZoom += 0.1;
            ApplyZoom();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            currentZoom = Math.Max(0.1, currentZoom - 0.1);
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            var transform = new ScaleTransform(currentZoom, currentZoom);
            DrawCanvas.RenderTransform = transform;
            CanvasScrollViewer.ScrollToVerticalOffset(0);
            CanvasScrollViewer.ScrollToHorizontalOffset(0);
        }
     

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
           
            BrushSelectionComboBox.SelectedIndex = 0;
            ThicknessSelectionComboBox.SelectedIndex = 2;
            ColorSelectionButton.SelectedColor = Colors.Black;
        }

    }
}
