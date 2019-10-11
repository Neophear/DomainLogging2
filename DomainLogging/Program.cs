using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogging
{
    class Program
    {
        readonly DomainLoggingService.DomainLoggingServiceClient client;
        readonly string args;

        static void Main(string[] args)
        {
            Program p = new Program(args);

            p.Begin();
        }

        public Program(string[] args)
        {
            client = new DomainLoggingService.DomainLoggingServiceClient(new BasicHttpBinding(), new EndpointAddress("http://dlservice:6970/DomainLoggingService.svc"));
            //client = new DomainLoggingService.DomainLoggingServiceClient(new BasicHttpBinding(), new EndpointAddress("http://localhost:2870/DomainLoggingService.svc"));
            this.args = String.Join(" ", args);
        }

        private void Begin()
        {
            try
            {
                string serialNumber = GetWMIValue("Win32_BIOS", "SerialNumber").ToString();

                bool needsUpdate = client.WriteLog(serialNumber, Environment.UserName, args);

                if (needsUpdate)
                {
                    DomainLoggingService.ComputerInfo computerInfo = new DomainLoggingService.ComputerInfo
                    {
                        SerialNumber = serialNumber,
                        Name = Environment.MachineName,
                        Model = GetWMIValue("Win32_ComputerSystem", "Model").ToString(),
                        OS = GetWMIValue("Win32_OperatingSystem", "Caption").ToString(),
                        CPU = GetWMIValue("Win32_Processor", "Name").ToString(),
                        CPUCores = (uint)GetWMIValue("Win32_Processor", "NumberOfCores"),
                        RAMGB = (uint)((ulong)GetWMIValue("Win32_PhysicalMemory", "Capacity") / 1073741824),
                        DiskMediaType = GetDiskType(),
                        DiskSize = (uint)((DriveInfo.GetDrives()).FirstOrDefault(i => i.Name == @"C:\").TotalSize / 1073741824),
                        GFX = GetWMIValue("Win32_DisplayConfiguration", "DeviceName").ToString(),
                        TeamViewerId = GetTeamViewerId()
                    };

                    client.SaveComputerInfo(computerInfo);
                }

                if (String.IsNullOrEmpty(args) && Environment.UserName.ToLower().StartsWith("admin") && client.NeedsLocationUpdate(serialNumber))
                {
                    Console.WriteLine("Der er behov for opdatering af lokation og ejer!");

                    bool inputData = true;

                    while (inputData)
                    {
                        Console.Write("Myndighed: ");
                        var myn = Console.ReadLine().ToUpper();

                        Console.Write("Bygning: ");
                        var byg = Console.ReadLine().ToUpper();

                        Console.Write("Rum: ");
                        var lok = Console.ReadLine().ToUpper();

                        Console.Write("Ejer: ");
                        var ejer = Console.ReadLine().ToUpper();

                        Console.WriteLine();
                        Console.WriteLine("-------------------Info-------------------");
                        Console.WriteLine($"MYN:          {myn}");
                        Console.WriteLine($"BYG:          {byg}");
                        Console.WriteLine($"LOK:          {lok}");
                        Console.WriteLine($"Ejer:         {ejer}");
                        Console.WriteLine("------------------------------------------");
                        Console.WriteLine();

                        inputData = !GetUserConfirmation("Data OK?");

                        if (!inputData)
                        {
                            bool save = GetUserConfirmation("Gem?");

                            while (save)
                            {
                                try
                                {
                                    client.SaveComputerLocation(serialNumber, myn, byg, lok, ejer);
                                    save = false;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Der skete en fejl!");
                                    Console.WriteLine(ex.Message);
                                    save = GetUserConfirmation("Prøv igen?");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!(ex is TimeoutException || ex is EndpointNotFoundException))
                {
                    try
                    {
                        client.WriteErrorLog(Environment.MachineName, ex.ToString());
                    }
                    catch (Exception)
                    { }
                }
            }
            finally
            {
                client.Close();
            }
        }

        private DomainLoggingService.DiskType GetDiskType()
        {
            if (GetWMIValue("Win32_OperatingSystem", "Caption").ToString().Contains("7"))
                return DomainLoggingService.DiskType.Unknown;
            else
            {
                ushort mediaType = (ushort)GetWMIValue("MSFT_PhysicalDisk", "MediaType", @"\\.\root\microsoft\windows\storage");

                if (mediaType == 0)
                    mediaType = 2;

                return (DomainLoggingService.DiskType)mediaType;
            }
        }

        private string GetTeamViewerId()
        {
            object key = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\TeamViewer\", "ClientID", "");
            if (key == null)
                return "TeamViewer ikke installeret";
            else if (key.ToString() == "")
                return "Key 'ClientID' findes ikke";
            else
                return key.ToString();
        }

        private static bool GetUserConfirmation(string message)
        {
            bool? confirmation = null;

            while (confirmation == null)
            {
                Console.Write($"{message} (j/n): ");

                var pressedKey = Console.ReadKey();

                Console.WriteLine();

                if (pressedKey.Key == ConsoleKey.J)
                    confirmation = true;
                else if (pressedKey.Key == ConsoleKey.N)
                    confirmation = false;
                else
                    Console.WriteLine("Forkert input, prøv igen!\n");
            }

            return confirmation.Value;
        }

        private object GetWMIValue(string wminame, string property, string scope = "")
        {
            try
            {
                var sc = new ManagementScope(scope);
                sc.Connect();

                var searcher = new ManagementObjectSearcher($"SELECT {property} FROM {wminame}");
                searcher.Scope = sc;

                return (from x in searcher.Get().Cast<ManagementObject>()
                            select x.GetPropertyValue(property)).FirstOrDefault();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
