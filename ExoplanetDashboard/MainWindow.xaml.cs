using System;
using System.IO.Ports;
using System.Windows;
using LiveCharts;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace ExoplanetDashboard
{
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;
        private bool isDataProcessing = false;
        private StreamWriter _streamWriter;
        private string _filePath;
        private const int FlushInterval = 1000; // milliseconds
        private DataProcessor _dataProcessor;
        private ChartManager _chartManager;

        public ChartValues<ObservablePoint> AltitudeData { get; set; }
        public ChartValues<ObservablePoint> AccelerationData { get; set; }
        public ChartValues<ObservablePoint> VoltageData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            AltitudeData = new ChartValues<ObservablePoint>();
            AccelerationData = new ChartValues<ObservablePoint>();
            VoltageData = new ChartValues<ObservablePoint>();
            _chartManager = new ChartManager(AltitudeChart, AltitudeData, AltitudeDataText, 
                                             AccelerationChart, AccelerationData, AccelerationDataText, 
                                             VoltageChart, VoltageData, VoltageDataText);

            LoadAvailablePorts();
            LoadBaudRates();
            DebugInfoText.Text = "";

            _dataProcessor = new DataProcessor(AltitudeData, AccelerationData, VoltageData);
        }

        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            PortComboBox.ItemsSource = ports;
        }

        private void LoadBaudRates()
        {
            BaudRateComboBox.Items.Add("9600");
            BaudRateComboBox.Items.Add("14400");
            BaudRateComboBox.Items.Add("19200");
            BaudRateComboBox.Items.Add("38400");
            BaudRateComboBox.Items.Add("57600");
            BaudRateComboBox.Items.Add("115200");
            BaudRateComboBox.SelectedIndex = 0;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedPort = PortComboBox.SelectedItem as string;
            string selectedBaudRate = BaudRateComboBox.SelectedItem as string;

            if (selectedPort != null && selectedBaudRate != null)
            {
                _serialPort = new SerialPort(selectedPort, int.Parse(selectedBaudRate));
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                try
                {
                    _serialPort.Open();
                    if (_serialPort.IsOpen)
                    {
                        ConnectionStatusText.Text = "Connected";
                        ConnectionStatusText.Foreground = System.Windows.Media.Brushes.Green;
                        DebugInfoText.Text = "Connected to " + selectedPort + " at " + selectedBaudRate + " baud.";

                        // Generate a unique filename with a timestamp
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        _filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataLogs", $"data_{timestamp}.txt");
                        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)); // Ensure the directory exists
                        _streamWriter = new StreamWriter(new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None), System.Text.Encoding.UTF8);
                        StartFlushing();
                    }
                    else
                    {
                        DebugInfoText.Text = "Failed to open port.";
                    }
                }
                catch (Exception ex)
                {
                    DebugInfoText.Text = "Error: " + ex.Message;
                }
            }
            else
            {
                MessageBox.Show("Please select a port and a baud rate.");
            }
        }
        private async void StartFlushing()
        {
            while (_serialPort != null && _serialPort.IsOpen)
            {
                await Task.Delay(FlushInterval);
                _streamWriter?.Flush();
            }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            isDataProcessing = true;
            SerialPort sp = (SerialPort)sender;
            if (sp.IsOpen)
            {
                string indata = sp.ReadLine();
                Dispatcher.Invoke(() =>
                {
                    IncomingDataTextBox.Text = indata; // Update the Incoming Data Text Box

                    // Write data to file
                    _streamWriter.WriteLine(indata);

                    // Process the data packet
                    bool parsingError;
                    _dataProcessor.ProcessData(indata, out parsingError);
                    if (parsingError)
                    {
                        DebugInfoText.Text = "Error parsing data packet.";
                    }
                });
            }
            isDataProcessing = false;
        }

        private async void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                // Unsubscribe from DataReceived event
                _serialPort.DataReceived -= DataReceivedHandler;

                // Set a timeout for data processing completion
                int timeoutMilliseconds = 1000;
                Task dataProcessingTask = Task.Delay(timeoutMilliseconds);

                // Wait for data processing to complete or timeout
                await Task.WhenAny(dataProcessingTask);

                // Close the serial port
                try
                {
                    _serialPort.Close();
                    ConnectionStatusText.Text = "Disconnected";
                    ConnectionStatusText.Foreground = System.Windows.Media.Brushes.Red;
                    DebugInfoText.Text = "Serial port closed.";

                    // Close the file writer
                    _streamWriter?.Flush();
                    _streamWriter?.Close();
                }
                catch (IOException ex)
                {
                    DebugInfoText.Text = "Error while closing port: " + ex.Message;
                }
            }
            else
            {
                DebugInfoText.Text = "Serial port is not open.";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_serialPort != null)
            {
                _serialPort.DataReceived -= DataReceivedHandler; // Unsubscribe from DataReceived event
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }

            // Close the file writer if the window is closing
            _streamWriter?.Flush();
            _streamWriter?.Close();
        }
    }
}
