using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExoplanetDashboard
{
    public class ChartManager
    {
        private static ChartValues<ObservablePoint> _altitudeData;
        private static Chart _altitudeChart;
        private static TextBlock _altitudeDataText;

        private static ChartValues<ObservablePoint> _accelerationData;
        private static Chart _accelerationChart;
        private static TextBlock _accelerationDataText;

        private static ChartValues<ObservablePoint> _voltageData;
        private static Chart _voltageChart;
        private static TextBlock _voltageDataText;

        public ChartManager(Chart altitudeChart, ChartValues<ObservablePoint> altitudeData, TextBlock altitudeDataText,
                            Chart accelerationChart, ChartValues<ObservablePoint> accelerationData, TextBlock accelerationDataText,
                            Chart voltageChart, ChartValues<ObservablePoint> voltageData, TextBlock voltageDataText)
        {
            _altitudeChart = altitudeChart;
            _altitudeData = altitudeData;
            _altitudeDataText = altitudeDataText;

            _accelerationChart = accelerationChart;
            _accelerationData = accelerationData;
            _accelerationDataText = accelerationDataText;

            _voltageChart = voltageChart;
            _voltageData = voltageData;
            _voltageDataText = voltageDataText;

            InitializeAltitudeChart();
            InitializeAccelerationChart();
            InitializeVoltageChart();
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
                    Fill = System.Windows.Media.Brushes.Transparent,
                    Stroke = Brushes.DodgerBlue,
                }
            };
        }

        private void InitializeAccelerationChart()
        {
            _accelerationChart.Series = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Acceleration",
                Values = _accelerationData,
                LineSmoothness = 0,
                StrokeThickness = 1.5,
                PointGeometrySize = 2,
                Fill = System.Windows.Media.Brushes.Transparent,
                Stroke = Brushes.MediumVioletRed,
            }
        };
        }

        private void InitializeVoltageChart()
        {
            _voltageChart.Series = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Voltage",
                Values = _voltageData,
                LineSmoothness = 0,
                StrokeThickness = 1.5,
                PointGeometrySize = 2,
                Fill = Brushes.Transparent,
                Stroke = Brushes.LawnGreen,
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

            _altitudeDataText.Text = altitude.ToString("0.##") + " m";
        }

        public static void UpdateAccelerationChart(double timeSeconds, double acceleration)
        {
            double thresholdTime = timeSeconds - 10;
            for (int i = 0; i < _accelerationData.Count; i++)
            {
                if (_accelerationData[i].X < thresholdTime)
                {
                    _accelerationData.RemoveAt(i);
                    i--;
                }
            }

            double minTime = timeSeconds - 10;
            double maxTime = timeSeconds;
            _accelerationChart.AxisX[0].MinValue = minTime;
            _accelerationChart.AxisX[0].MaxValue = maxTime;

            _accelerationData.Add(new ObservablePoint(timeSeconds, acceleration));

            _accelerationDataText.Text = acceleration.ToString("0.##") + " m/s²";
        }

        public static void UpdateVoltageChart(double timeSeconds, double voltage)
        {
            double thresholdTime = timeSeconds - 10;
            for (int i = 0; i < _voltageData.Count; i++)
            {
                if (_voltageData[i].X < thresholdTime)
                {
                    _voltageData.RemoveAt(i);
                    i--;
                }
            }

            double minTime = timeSeconds - 10;
            double maxTime = timeSeconds;
            _voltageChart.AxisX[0].MinValue = minTime;
            _voltageChart.AxisX[0].MaxValue = maxTime;

            _voltageData.Add(new ObservablePoint(timeSeconds, voltage));

            _voltageDataText.Text = voltage.ToString("0.##") + " V";
        }
    }
}
