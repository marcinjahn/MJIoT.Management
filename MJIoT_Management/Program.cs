using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MJIoT_DBModel;
using Microsoft.Azure.Devices;

namespace MJIoT_Management
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new Manager();

            manager.CreateDeviceWithPropertiesAsync("Test1", 2);

            //manager.CreateDeviceProperty(3, 8, "false");

            Console.ReadLine();
        }


    }

    public class Manager
    {

        private string _iotHubCS = "HostName=MJIoT-IoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=ieEi5UNBx6C7js/+e6/G+oM3K/isI4WRARv2bBNd270=";
        
        public string IotHubCS
        {
            get { return _iotHubCS; }
            private set { _iotHubCS = value; }
        }
        public Manager()
        {

        }


        //EQUIPMENT TYPE
        public void CreateDeviceType(string name, int? baseTypeId = null, int? senderPropertyId = null, int? listenerPropertyId = null, bool isAbstract = false)
        {
            using (var context = new MJIoTDBContext())
            {
                //CREATE TYPE
                var type = new DeviceType();
                type.Name = name;
                if (baseTypeId != null)
                {
                    var baseType = context.DeviceTypes.Where(n => n.Id == baseTypeId).FirstOrDefault();
                    if (baseType == null)
                        throw new NullReferenceException("Device type of given ID does not exist");
                    type.BaseDeviceType = baseType;
                }
                if (senderPropertyId != null)
                {
                    var senderProperty = context.PropertyTypes.Where(n => n.Id == senderPropertyId).FirstOrDefault();
                    if (senderProperty == null)
                        throw new NullReferenceException("Property type of given ID does not exist");
                    type.SenderProperty = senderProperty;
                }
                if (listenerPropertyId != null)
                {
                    var listenerProperty = context.PropertyTypes.Where(n => n.Id == listenerPropertyId).FirstOrDefault();
                    if (listenerProperty == null)
                        throw new NullReferenceException("Property type of given ID does not exist");
                    type.ListenerProperty = listenerProperty;
                }

                type.IsAbstract = isAbstract;

                //SAVE TYPE
                context.DeviceTypes.Add(type);
                context.SaveChanges();
            }
        }

        async public Task CreateDeviceTypeAsync(string name, int? baseTypeId = null, int? senderPropertyId = null, int? listenerPropertyId = null, bool isAbstract = false)
        {
            await Task.Run(() => CreateDeviceType(name, baseTypeId, senderPropertyId , listenerPropertyId, isAbstract));
        }

        public void UpdateDeviceType(int id, string name, int? baseTypeId = null, int? senderPropertyId = null, int? listenerPropertyId = null, bool isAbstract = false)
        {
            using (var context = new MJIoTDBContext())
            {
                //CREATE TYPE
                var type = context.DeviceTypes.Where(n => n.Id == id).FirstOrDefault();
                if (type == null)
                {
                    throw new NullReferenceException("Device type of given ID does not exist");
                }

                type.Name = name;
                if (baseTypeId != null)
                {
                    var baseType = context.DeviceTypes.Where(n => n.Id == baseTypeId).FirstOrDefault();
                    if (baseType == null)
                        throw new NullReferenceException("Device type of given ID does not exist");
                    type.BaseDeviceType = baseType;
                }
                if (senderPropertyId != null)
                {
                    var senderProperty = context.PropertyTypes.Where(n => n.Id == senderPropertyId).FirstOrDefault();
                    if (senderProperty == null)
                        throw new NullReferenceException("Property type of given ID does not exist");
                    type.SenderProperty = senderProperty;
                }
                if (listenerPropertyId != null)
                {
                    var listenerProperty = context.PropertyTypes.Where(n => n.Id == listenerPropertyId).FirstOrDefault();
                    if (listenerProperty == null)
                        throw new NullReferenceException("Property type of given ID does not exist");
                    type.ListenerProperty = listenerProperty;
                }

                type.IsAbstract = isAbstract;

                //SAVE TYPE
                context.SaveChanges();
            }
        }

        public void RemoveDeviceType(int id)
        {
            using (var context = new MJIoTDBContext())
            {
                var type = context.DeviceTypes.Where(n => n.Id == id).FirstOrDefault();
                if (type == null)
                {
                    throw new NullReferenceException("Device type of given ID does not exist");
                }
                else
                {
                    context.DeviceTypes.Remove(type);
                    context.SaveChanges();
                }
            }
        }

        ////EQUIPMENT PROPERTY
        public void CreatePropertyType( int deviceTypeId, string propertyName, PropertyTypeFormats propertyType, bool uiConfigurable)
        {
            using (var context = new MJIoTDBContext())
            {
                //GET ID OF EQUIPMENT TYPE
                var type = context.DeviceTypes.Where(n => n.Id == deviceTypeId).FirstOrDefault();
                if (type == null)
                {
                    throw new NullReferenceException("Device type of given ID does not exist");
                }

                //CREATE PROEPRTY
                var property = new PropertyType
                {
                    DeviceType = type,
                    Name = propertyName,
                    Format = propertyType,
                    UIConfigurable = uiConfigurable
                };

                //SAVE PROEPRTY
                context.PropertyTypes.Add(property);
                context.SaveChanges();
            }
        }

        public void UpdatePropertyType(int id, int deviceTypeId, string propertyName, PropertyTypeFormats propertyType, bool uiConfigurable)
        {
            using (var context = new MJIoTDBContext())
            {
                var property = context.PropertyTypes.Where(n => n.Id == id).FirstOrDefault();
                if (property == null)
                {
                    throw new NullReferenceException("Property type of given ID does not exist");
                }

                var type = context.DeviceTypes.Where(n => n.Id == deviceTypeId).FirstOrDefault();
                if (type == null)
                {
                    throw new NullReferenceException("Device type of given ID does not exist");
                }

                property.DeviceType = type;
                property.Name = propertyName;
                property.Format = propertyType;
                property.UIConfigurable = uiConfigurable;

                //SAVE PROEPRTY
                context.SaveChanges();
            }
        }

        public void RemovePropertyType(int id)
        {
            using (var context = new MJIoTDBContext())
            {
                var property = context.PropertyTypes.Where(n => n.Id == id).FirstOrDefault();
                if (property == null)
                {
                    throw new NullReferenceException("Property type of given ID does not exist");
                }

                context.PropertyTypes.Remove(property);
                context.SaveChanges();
            }
        }

        public void CreateDeviceProperty(int propertyTypeId, int deviceId, string value)
        {
            var property = new DeviceProperty();

            using (var context = new MJIoTDBContext())
            {
                var propertyType = context.PropertyTypes.Where(n => n.Id == propertyTypeId).FirstOrDefault();
                if (propertyType == null)
                {
                    throw new NullReferenceException("Property type of given ID does not exist");
                }

                var device = context.Devices.Where(n => n.Id == deviceId).FirstOrDefault();
                if (device == null)
                {
                    throw new NullReferenceException("Device of given ID does not exist");
                }

                property.Device = device;
                property.PropertyType = propertyType;
                property.Value = value;

                context.DeviceProperties.Add(property);
                context.SaveChanges();
            }
        }


        //DEVICE
        private async Task<string> AddDeviceAsync(int deviceId)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IotHubCS);
            Microsoft.Azure.Devices.Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Microsoft.Azure.Devices.Device(deviceId.ToString()));
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceAlreadyExistsException)
            {
                throw new Exception();
                //device = await registryManager.GetDeviceAsync(deviceId);
            }

            return device.Authentication.SymmetricKey.PrimaryKey;
            //Console.WriteLine("Generated device key:" + device.Authentication.SymmetricKey.PrimaryKey);

        }

        public async Task<int> CreateDeviceAsync(string name, int deviceTypeId, int userId = 1, List<MJIoT_DBModel.Device> listenerDevices = null)
        {
            int deviceId;

            using (var context = new MJIoTDBContext())
            {
                var type = context.DeviceTypes.Where(n => n.Id == deviceTypeId).FirstOrDefault();
                if (type == null)
                {
                    throw new NullReferenceException("Device type of given ID does not exist");
                }

                var user = context.Users.Where(n => n.Id == userId).FirstOrDefault();
                if (user == null)
                {
                    throw new NullReferenceException("User of given ID does not exist");
                }


                var device = new MJIoT_DBModel.Device();

                device.DeviceType = type;
                device.IoTHubKey = "(new device)";  //temporary value
                device.ListenerDevices = listenerDevices;
                device.User = user;

                context.Devices.Add(device);
                context.SaveChanges();

                deviceId = device.Id;

                //NAME
                if (name != null)
                {
                    var namePropertyId = 1; //SHOULD BE FETCHED FROM DB
                    CreateDeviceProperty(namePropertyId, deviceId, name);
                }
            }

            var deviceKey = await AddDeviceAsync(deviceId);

            using (var context = new MJIoTDBContext())
            {
                var device = context.Devices.Where(n => n.Id == deviceId).FirstOrDefault();
                device.IoTHubKey = deviceKey;

                context.SaveChanges();
            }

            return deviceId;
        }

        public async void CreateDeviceWithPropertiesAsync(string name, int deviceTypeId, int userId = 1, List<MJIoT_DBModel.Device> listenerDevices = null)
        {
            var deviceId = await CreateDeviceAsync(name, deviceTypeId, userId, listenerDevices);
            var properties = new List<int>();
            using (var context = new MJIoTDBContext())
            {
                var type = deviceTypeId;

                while (true)
                {
                    //get all properties of the type, which a new device is instance of and these that are inherited from base types
                    properties.AddRange(
                            context.PropertyTypes.Include("DeviceType")
                            .Where(n => n.DeviceType.Id == type && n.Name != "DisplayName")
                            .Select(n => n.Id)
                            .ToList()
                        );

                    var typeObject = context.DeviceTypes.Include("BaseDeviceType")
                        .Where(n => n.Id == type)
                        .Select(n => n.BaseDeviceType)
                        .FirstOrDefault();

                    if (typeObject == null)
                        break;

                    type = typeObject.Id;
                    //.Select(n => n.BaseDeviceType.Id)
                    //.FirstOrDefault();

                }
            }
            foreach (var property in properties)
            {
                CreateDeviceProperty(property, deviceId, "none");  //na razie "none" - generalnie każdy PropertyType powinien mieć jakis default value, który trzebaby pobrać i zapisać do nowje propercji
            }
                
        }

        public void RemoveDevice(int id)
        {
            //JESCZE USUWNIE Z IOTHUBA

            using (var context = new MJIoTDBContext())
            {
                var device = context.Devices.Where(n => n.Id == id).FirstOrDefault();
                if (device == null)
                {
                    throw new NullReferenceException("Device of given ID does not exist");
                }

                context.Devices.Remove(device);
                context.SaveChanges();
            }
        }
    }
}
