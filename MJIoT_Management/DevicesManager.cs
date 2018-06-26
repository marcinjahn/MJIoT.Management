using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.Entity;
using System.Threading.Tasks;
using MJIoT_DBModel;
using Microsoft.Azure.Devices;

namespace MjIot.Tools
{
    public class DevicesManager
    {

        public string IotHubCS { get; private set; } = "HostName=MJIoT-Hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=SzQKdF1y6bAEgGfZei2bmq1Jd83odc+B2x197n2MtxA=";

        //CONNECTIONS BETWEEN DEVICES
        public void CreateConnection(string senderId, string senderPropertyName, string listenerId, string listenerPropertyName, 
            ConnectionFilter filter, string filterValue, ConnectionCalculation calculation, string calculationValue)
        {
            using (var context = new MJIoTDBContext())
            {
                var sender = context.Devices
                    .Include(n => n.DeviceType)
                    .Where(n => n.Id.ToString() == senderId)
                    .FirstOrDefault();

                var listener = context.Devices
                    .Include(n => n.DeviceType)
                    .Where(n => n.Id.ToString() == listenerId)
                    .FirstOrDefault();

                var senderProperty = context.PropertyTypes
                    .Include(n => n.DeviceType)
                    .Where(n => n.DeviceType.Id == sender.DeviceType.Id && n.Name == senderPropertyName)
                    .FirstOrDefault();

                var listenerProperty = context.PropertyTypes
                    .Include(n => n.DeviceType)
                    .Where(n => n.DeviceType.Id == listener.DeviceType.Id && n.Name == listenerPropertyName)
                    .FirstOrDefault();

                var connection = new Connection
                {
                    SenderDevice = sender,
                    SenderProperty = senderProperty,
                    ListenerDevice = listener,
                    ListenerProperty = listenerProperty,
                    Filter = filter,
                    FilterValue = filterValue,
                    Calculation = calculation,
                    CalculationValue = calculationValue
                };

                context.Connections.Add(connection);
                context.SaveChanges();
            }
        }


        //EQUIPMENT TYPE
        public DeviceType CreateDeviceType(string name, int? baseTypeId = 1, bool isAbstract = false, bool offlineMessagesEnabled = true)
        {
            using (var context = new MJIoTDBContext())
            {
                //CREATE TYPE
                var type = new DeviceType();
                type.Name = name;
                if (baseTypeId != null)
                {
                    var baseType = context.DeviceTypes.Where(n => n.Id == baseTypeId).FirstOrDefault();
                    type.BaseDeviceType = baseType ?? throw new NullReferenceException("Device type of given ID does not exist");
                }

                type.IsAbstract = isAbstract;
                type.OfflineMessagesEnabled = offlineMessagesEnabled;

                //SAVE TYPE
                context.DeviceTypes.Add(type);
                context.SaveChanges();

                return type;
            }
        }

        public DeviceType UpdateDeviceType(int id, string name = null, int? baseTypeId = null, bool isAbstract = false, bool offlineMessagesEnabled = true)
        {
            using (var context = new MJIoTDBContext())
            {
                //CREATE TYPE
                var type = context.DeviceTypes.Where(n => n.Id == id).FirstOrDefault();
                if (type == null)
                {
                    throw new NullReferenceException("Device type of given ID does not exist");
                }

                if (name != null)
                    type.Name = name;

                if (baseTypeId != null)
                {
                    var baseType = context.DeviceTypes.Where(n => n.Id == baseTypeId).FirstOrDefault();
                    type.BaseDeviceType = baseType ?? throw new NullReferenceException("Device type of given ID does not exist");
                }

                type.IsAbstract = isAbstract ? true : false;
                type.OfflineMessagesEnabled = offlineMessagesEnabled ? true : false;

                //SAVE TYPE
                context.SaveChanges();

                return type;
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
        public PropertyType CreatePropertyType(DeviceType deviceType, string propertyName, PropertyFormat propertyType, bool uiConfigurable, bool isSenderProperty, bool isListenerProperty)
        {
            using (var context = new MJIoTDBContext())
            {
                //GET ID OF EQUIPMENT TYPE
                var deviceTypeId = context.DeviceTypes.Where(n => n.Id == deviceType.Id).FirstOrDefault().Id;
                return CreatePropertyType(deviceTypeId, propertyName, propertyType, uiConfigurable, isSenderProperty, isListenerProperty);
            }
        }

        public PropertyType CreatePropertyType( int deviceTypeId, string propertyName, PropertyFormat propertyType, bool uiConfigurable, bool isSenderProperty, bool isListenerProperty)
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
                    UIConfigurable = uiConfigurable,
                    IsListenerProperty = isListenerProperty,
                    IsSenderProperty = isSenderProperty
                };

                //SAVE PROEPRTY
                context.PropertyTypes.Add(property);
                context.SaveChanges();

                return property;
            }
        }

        public PropertyType UpdatePropertyType(int id, int? deviceTypeId = null, string propertyName = null, PropertyFormat? propertyType = null, bool? uiConfigurable = null, bool? isSenderProperty = null, bool? isListenerProperty = null)
        {
            using (var context = new MJIoTDBContext())
            {
                var property = context.PropertyTypes.Where(n => n.Id == id).FirstOrDefault();
                if (property == null)
                {
                    throw new NullReferenceException("Property type of given ID does not exist");
                }

                if (deviceTypeId != null)
                {
                    var type = context.DeviceTypes.Where(n => n.Id == deviceTypeId).FirstOrDefault();
                    property.DeviceType = type ?? throw new NullReferenceException("Device type of given ID does not exist");
                }

                if (propertyName != null)
                    property.Name = propertyName;
                if (propertyType != null)
                    property.Format = propertyType.Value;
                if (uiConfigurable != null)
                    property.UIConfigurable = uiConfigurable.Value;
                if (isSenderProperty != null)
                    property.IsSenderProperty = isSenderProperty.Value;
                if (isListenerProperty != null)
                    property.IsListenerProperty = isListenerProperty.Value;

                //SAVE PROEPRTY
                context.SaveChanges();

                return property;
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


        //DEPRECATED
        [Obsolete("Storing properties in SQL is not a supported solution anymore")]
        public void CreateDeviceProperty(int propertyTypeId, int deviceId, string value)
        {
            var property = new DeviceProperty();

            using (var context = new MJIoTDBContext())
            {
                var propertyType = context.PropertyTypes.Where(n => n.Id == propertyTypeId).FirstOrDefault();
                var device = context.Devices.Where(n => n.Id == deviceId).FirstOrDefault();
                property.Device = device ?? throw new NullReferenceException("Device of given ID does not exist");
                property.PropertyType = propertyType ?? throw new NullReferenceException("Property type of given ID does not exist");
                property.PropertyValue = value;

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

        public async Task<int> CreateDeviceAsync(string name, DeviceType deviceType, int userId = 1)
        {
            using (var context = new MJIoTDBContext())
            {
                var deviceTypeId = context.DeviceTypes.Where(n => n.Id == deviceType.Id).FirstOrDefault().Id;
                return await CreateDeviceAsync(name, deviceTypeId, userId);
            }
        }

        public async Task<int> CreateDeviceAsync(string name, int deviceTypeId, int userId = 1)
        {
            int deviceId;

            using (var context = new MJIoTDBContext())
            {
                var type = context.DeviceTypes.Where(n => n.Id == deviceTypeId).FirstOrDefault();
                var user = context.Users.Where(n => n.Id == userId).FirstOrDefault();
                var device = new MJIoT_DBModel.Device
                {
                    DeviceType = type ?? throw new NullReferenceException("Device type of given ID does not exist"),
                    IoTHubKey = "(new device)",  //temporary value
                    User = user ?? throw new NullReferenceException("User of given ID does not exist")
                };

                context.Devices.Add(device);
                context.SaveChanges();

                deviceId = device.Id;

                //NAME
                //if (name != null)
                //{
                //    var namePropertyId = 1; //SHOULD BE FETCHED FROM DB
                //    CreateDeviceProperty(namePropertyId, deviceId, name);
                //}
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

        //DEPRECATED
        [Obsolete("Storing properties in SQL is not a supported solution anymore")]
        public async void CreateDeviceWithPropertiesAsync(string name, int deviceTypeId, int userId = 1, List<MJIoT_DBModel.Device> listenerDevices = null)
        {
            var deviceId = await CreateDeviceAsync(name, deviceTypeId, userId);
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
