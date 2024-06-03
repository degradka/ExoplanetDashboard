using System.Globalization;

namespace ExoplanetDashboard
{
    public class DataPacket
    {
        public string TeamID { get; set; }
        public long Time { get; set; } // in milliseconds
        public double Voltage { get; set; } // in volts
        public double Acceleration { get; set; } // in m/s^2
        public double Altitude { get; set; } // in meters
        public bool StartPoint { get; set; }
        public bool ApogeePoint { get; set; }
        public bool ActivatePoint { get; set; }
        public bool SatPoint { get; set; }
        public bool ParachutePoint { get; set; }
        public bool LandingPoint { get; set; }

        public DataPacket(string teamID, long time, double voltage, double acceleration, double altitude, bool startPoint, bool apogeePoint, bool activatePoint, bool satPoint, bool parachutePoint, bool landingPoint)
        {
            TeamID = teamID;
            Time = time;
            Voltage = voltage;
            Acceleration = acceleration;
            Altitude = altitude;
            StartPoint = startPoint;
            ApogeePoint = apogeePoint;
            ActivatePoint = activatePoint;
            SatPoint = satPoint;
            ParachutePoint = parachutePoint;
            LandingPoint = landingPoint;
        }
    }

    public static class DataPacketParser
    {
        public static DataPacket ParseDataPacket(string data)
        {
            string[] dataValues = data.Split(';');

            if (dataValues.Length >= 11)
            {
                string teamID = dataValues[0].Trim();
                long time = long.Parse(dataValues[1].Trim());
                double voltage = double.Parse(dataValues[2].Trim(), CultureInfo.InvariantCulture);
                double acceleration = double.Parse(dataValues[3].Trim(), CultureInfo.InvariantCulture);
                double altitude = double.Parse(dataValues[4].Trim(), CultureInfo.InvariantCulture);
                bool startPoint = dataValues[5].Trim() == "1";
                bool apogeePoint = dataValues[6].Trim() == "1";
                bool activatePoint = dataValues[7].Trim() == "1";
                bool satPoint = dataValues[8].Trim() == "1";
                bool parachutePoint = dataValues[9].Trim() == "1";
                bool landingPoint = dataValues[10].Trim() == "1";

                return new DataPacket(teamID, time, voltage, acceleration, altitude, startPoint, apogeePoint, activatePoint, satPoint, parachutePoint, landingPoint);
            }
            else
            {
                // Handle insufficient data error
                return null;
            }
        }
    }
}
