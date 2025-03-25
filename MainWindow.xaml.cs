﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace DrawingApp
{
    public partial class MainWindow : Window
    {
        private Polyline currentLine;
        private Brush currentColor = Brushes.Black;

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
                    StrokeThickness = 2,
                    Points = new PointCollection { e.GetPosition(DrawCanvas) }
                };
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
    }
}
