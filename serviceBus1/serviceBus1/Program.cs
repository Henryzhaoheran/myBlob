using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;


namespace serviceBus1
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Endpoint=sb://hellodt.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=MReq3gs9m14ChDiIccuUejQAlD3ATzonuvyeUKeuQnk=";
            var queueName = "queue1";

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            // Send a message to the queue
            // var message = new BrokeredMessage("This is a test message!");
            // client.Send(message);

            // Receive a Message from the queue
            client.OnMessage(message =>
            {
                Console.WriteLine(String.Format("Message body: {0}", message.GetBody<String>()));
                Console.WriteLine(String.Format("Message id: {0}", message.MessageId));
            });

            Console.ReadKey();
        }
    }
}
