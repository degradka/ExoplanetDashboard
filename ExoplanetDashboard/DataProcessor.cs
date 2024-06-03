using LiveCharts.Defaults;
using LiveCharts;

namespace ExoplanetDashboard
{
    public class DataProcessor
    {
        private ChartValues<ObservablePoint> _altitudeData;

        public DataProcessor(ChartValues<ObservablePoint> altitudeData)
        {
            _altitudeData = altitudeData;
        }

        public void ProcessData(string data, out bool parsingError)
        {
            parsingError = false;

            // Parse the data packet
            DataPacket packet = DataPacketParser.ParseDataPacket(data);
            if (packet != null)
            {
                double timeSeconds = packet.Time / 1000.0; // Convert milliseconds to seconds

                // Extract the altitude value
                double altitude = packet.Altitude;

                // Add new altitude data point
                ChartManager.UpdateAltitudeChart(timeSeconds, altitude);
            }
            else
            {
                // Set parsingError flag
                parsingError = true;
            }
        }
    }
}