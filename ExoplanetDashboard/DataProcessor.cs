using LiveCharts.Defaults;
using LiveCharts;

namespace ExoplanetDashboard
{
    public class DataProcessor
    {
        private ChartValues<ObservablePoint> _altitudeData;
        private ChartValues<ObservablePoint> _accelerationData;
        private ChartValues<ObservablePoint> _voltageData;

        public DataProcessor(ChartValues<ObservablePoint> altitudeData, ChartValues<ObservablePoint> accelerationData, ChartValues<ObservablePoint> voltageData)
        {
            _altitudeData = altitudeData;
            _accelerationData = accelerationData;
            _voltageData = voltageData;
        }

        public void ProcessData(string data, out bool parsingError)
        {
            parsingError = false;

            // Parse the data packet
            DataPacket packet = DataPacketParser.ParseDataPacket(data);
            if (packet != null)
            {
                double timeSeconds = packet.Time / 1000.0; // Convert milliseconds to seconds

                // Extract the values
                double altitude = packet.Altitude;
                double acceleration = packet.Acceleration;
                double voltage = packet.Voltage;

                // Add new data points
                ChartManager.UpdateAltitudeChart(timeSeconds, altitude);
                ChartManager.UpdateAccelerationChart(timeSeconds, acceleration);
                ChartManager.UpdateVoltageChart(timeSeconds, voltage);
            }
            else
            {
                // Set parsingError flag
                parsingError = true;
            }
        }
    }
}