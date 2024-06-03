using LiveCharts.Defaults;
using LiveCharts;
using System.Windows.Controls;

namespace ExoplanetDashboard
{
    public class DataProcessor
    {
        private ChartValues<ObservablePoint> _altitudeData;
        private ChartValues<ObservablePoint> _accelerationData;
        private ChartValues<ObservablePoint> _voltageData;

        private TextBlock _launchDetectionStatus;
        private TextBlock _apogeeDetectionStatus;
        private TextBlock _activatePointStatus;
        private TextBlock _satellitePointStatus;
        private TextBlock _parachutePointStatus;
        private TextBlock _landingPointStatus;

        public DataProcessor(ChartValues<ObservablePoint> altitudeData, ChartValues<ObservablePoint> accelerationData, ChartValues<ObservablePoint> voltageData,
                             TextBlock launchDetectionStatus,
                             TextBlock apogeeDetectionStatus,
                             TextBlock activatePointStatus,
                             TextBlock satellitePointStatus,
                             TextBlock parachutePointStatus,
                             TextBlock landingPointStatus)
        {
            _altitudeData = altitudeData;
            _accelerationData = accelerationData;
            _voltageData = voltageData;

            _launchDetectionStatus = launchDetectionStatus;
            _apogeeDetectionStatus = apogeeDetectionStatus;
            _activatePointStatus = activatePointStatus;
            _satellitePointStatus = satellitePointStatus;
            _parachutePointStatus = parachutePointStatus;
            _landingPointStatus = landingPointStatus;
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

                UpdateStatus(packet.StartPoint, packet.ApogeePoint, packet.ActivatePoint, packet.SatPoint, packet.ParachutePoint, packet.LandingPoint);
            }
            else
            {
                // Set parsingError flag
                parsingError = true;
            }
        }

        private void UpdateStatus(bool launchDetected, bool apogeeDetected, bool activatePoint, bool satellitePoint, bool parachutePoint, bool landingPoint)
        {
            _launchDetectionStatus.Text = launchDetected ? "TRUE" : "FALSE";
            _apogeeDetectionStatus.Text = apogeeDetected ? "TRUE" : "FALSE";
            _activatePointStatus.Text = activatePoint ? "TRUE" : "FALSE";
            _satellitePointStatus.Text = satellitePoint ? "TRUE" : "FALSE";
            _parachutePointStatus.Text = parachutePoint ? "TRUE" : "FALSE";
            _landingPointStatus.Text = landingPoint ? "TRUE" : "FALSE";
        }
    }
}