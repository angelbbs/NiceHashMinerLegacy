namespace NiceHashMiner.Devices.Querying
{
    public class VideoControllerData
    {
        public int ID { get; set; }
        public int BusID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PnpDeviceID { get; set; }
        public string VEN_ { get; set; }
        public string DEV_ { get; set; }
        public string SUBSYS_ { get; set; }
        public string REV_ { get; set; }
        public string fakeID_ { get; set; }
        public bool NvidiaLHR { get; set; }
        public string DeviceID { get; set; }
        public string DriverVersion { get; set; }
        public string Status { get; set; }
        public string InfSection { get; set; } // get arhitecture
        public ulong AdapterRam { get; set; }
        public string CurrentRefreshRate { get; set; }
        public string Manufacturer { get; internal set; }
        public string VideoProcessor { get; internal set; }
    }
}
