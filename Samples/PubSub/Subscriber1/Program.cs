using System;
using Common.Logging;
using Messages;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Unicast.Config;
using NServiceBus.Unicast.Transport.Msmq.Config;
using ObjectBuilder;

namespace Subscriber1
{
    class Program
    {
        static void Main()
        {
            LogManager.GetLogger("hello").Debug("Started.");
            ObjectBuilder.SpringFramework.Builder builder = new ObjectBuilder.SpringFramework.Builder();

            builder.ConfigureComponent<MessageMapper>(ComponentCallModelEnum.Singleton);

            NServiceBus.Serializers.Configure.InterfaceToXMLSerializer.With(builder);
            //NServiceBus.Serializers.Configure.XmlSerializer.With(builder);

            new ConfigMsmqTransport(builder)
                .IsTransactional(false)
                .PurgeOnStartup(false);

            new ConfigUnicastBus(builder)
                .ImpersonateSender(false)
                .DoNotAutoSubscribe()
                .SetMessageHandlersFromAssembliesInOrder(
                    typeof(EventMessageHandler).Assembly
                );

            IBus bus = builder.Build<IBus>();

            bus.Start();

            //if we were to auto subscribe, this would happen automatically in bus.Start
            bus.Subscribe(typeof(EventMessage));

            Console.WriteLine("Listening for events. To exit, press 'q' and then 'Enter'.");
            while (Console.ReadLine().ToLower() != "q")
            {
            }
        }
    }
}
