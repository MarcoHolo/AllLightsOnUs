using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace LightControllerOnPI2.Common
{
    class AzureToRelayManager
    {

        //The GPIO pin-number of the rasp pi2. In this pin, you have  to connect the "s"-Pin of the relay
        //IMPORTANT - It is NOT the physical number of the pin, it is the number of the GPIO mapping
        private const int RelayPinNumber = 4; //<Insert your GPIO Pin Number here>
        private GpioPin RelayPin;
        private string lastMessageID = string.Empty;
        private const string azureConnectionString = "<Insert your Service Bus Connection String from the portal here>";
        private const string queueName = "<Insert your queuename here";
        

        public AzureToRelayManager()
        {
            initHardware();
            initServiceBusQueue();

        }

        private void initHardware()
        {
            //trying to open the gpio-pins 
            var gpioController = GpioController.GetDefault();
            RelayPin = gpioController.OpenPin(RelayPinNumber);
            RelayPin.SetDriveMode(GpioPinDriveMode.Output);

            //close Relay
            RelayPin.Write(GpioPinValue.Low);
        }

        private void initServiceBusQueue()
        {
            //to get access to the azure queue object, you need to install the azure messaging package with the nuget package manager console
            //install-Package WindowsAzure.Messaging.Managed
            Queue myQueueClient = new Queue(queueName, azureConnectionString);

            myQueueClient.OnMessage((message) =>
            {
                string messageID = message.MessageId;
                if (messageID != lastMessageID)
                {
                
                    lastMessageID = messageID;
                    string messageText = message.GetBody<string>();
                    if (messageText == "turn on")
                    {
                        //open Relay
                        RelayPin.Write(GpioPinValue.High);
                    }
                    else
                    {
                        RelayPin.Write(GpioPinValue.Low);
                    }
                }
            });
        }
    }
}
