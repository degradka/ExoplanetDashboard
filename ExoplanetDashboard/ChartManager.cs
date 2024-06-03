using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using System.Windows.Controls;

namespace ExoplanetDashboard
{
    public class ChartManager
    {
        private static ChartValues<ObservablePoint> _altitudeData;
        private static Chart _altitudeChart;
        private static TextBlock _altitudeDataText;

        public ChartManager(Chart altitudeChart, ChartValues<ObservablePoint> altitudeData, TextBlock altitudeDataText)
        {
            _altitudeChart = altitudeChart;
            _altitudeData = altitudeData;
            _altitudeDataText = altitudeDataText;

            InitializeAltitudeChart();
        }

        private void InitializeAltitudeChart()
        {
            _altitudeChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Altitude",
                    Values = _altitudeData,
                    LineSmoothness = 0,
                    StrokeThickness = 1.5,
                    PointGeometrySize = 2,
                    Fill = System.Windows.Media.Brushes.Transparent
                }
            };
        }

        public static void UpdateAltitudeChart(double timeSeconds, double altitude)
        {
            // Remove data points older than 10 seconds
            double thresholdTime = timeSeconds - 10;
            for (int i = 0; i < _altitudeData.Count; i++)
            {
                if (_altitudeData[i].X < thresholdTime)
                {
                    _altitudeData.RemoveAt(i);
                    i--; // Decrement i since we removed an element
                }
            }

            // Update X-axis range
            double minTime = timeSeconds - 10;
            double maxTime = timeSeconds;
            _altitudeChart.AxisX[0].MinValue = minTime;
            _altitudeChart.AxisX[0].MaxValue = maxTime;

            // Add new altitude data point
            _altitudeData.Add(new ObservablePoint(timeSeconds, altitude));

            _altitudeDataText.Text = altitude.ToString("0.##") + "m";
        }
    }
}
