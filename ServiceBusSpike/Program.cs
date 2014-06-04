using System;
using log4net;
using log4net.Config;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace ServiceBusSpike
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure();

                var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString.Send");
                Console.WriteLine(connectionString);

                var client = TopicClient.CreateFromConnectionString(connectionString, "foo/bar");
                client.Send(new BrokeredMessage("this is a test"));

                Log.DebugFormat("Finished OK");

                connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString.Listen");
                var subscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString,
                                                                                       "foo/bar",
                                                                                       "Default");
                var message = subscriptionClient.Receive(TimeSpan.FromSeconds(1));
                while (message != null)
                {
                    Console.WriteLine("Message: {0}", message.GetBody<string>());
                    message.Complete();
                    message = subscriptionClient.Receive(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}