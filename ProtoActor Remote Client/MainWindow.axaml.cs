using Avalonia.Controls;
using Avalonia.Threading;
using chat.messages;
using Grpc.Core;
using Proto;
using Proto.Remote;
using Proto.Remote.GrpcNet;
using ProtoActor_Remote_Client.Common;
using System;
using System.Threading.Tasks;
using static Proto.Remote.GrpcNet.GrpcNetRemoteConfig;

namespace ProtoActor_Remote_Client
{
    public partial class MainWindow : Window
    {
        private static RootContext context;
        //private static PID server;
        public static Window window;

        public MainWindow()
        {
            window = this;
            InitializeComponent();
            InitializeActorSystem();
            SpawnClient();
        }

        ~MainWindow()
        {
            context.System.Remote().ShutdownAsync().GetAwaiter().GetResult();
        }

        //private static void ObtainServerPid() =>
        //server = PID.FromAddress("127.0.0.1:8000", "chatserver");

        //private static void ConnectToServer() =>
        //context.Send(
        //    server,
        //    new Connect
        //    {
        //        Sender = client
        //    }
        //);

        private static void InitializeActorSystem()
        {
            var remotesystem = new ActorSystem()
                .WithClientRemote(BindToAllInterfaces()
                .WithProtoMessages(ChatReflection.Descriptor));
            remotesystem.Remote().StartAsync();

            context = remotesystem.Root;
        }

        //private static void SpawnClient() =>
        //client = context.Spawn(
        //    Props.FromFunc(
        //        ctx => {
        //            switch (ctx.Message)
        //            {
        //                case Connected connected:
        //                    Console.WriteLine(connected.Message);
        //                    break;
        //                case SayResponse sayResponse:
        //                    Console.WriteLine($"{sayResponse.UserName} {sayResponse.Message}");
        //                    break;
        //                case NickResponse nickResponse:
        //                    Console.WriteLine($"{nickResponse.OldUserName} is now {nickResponse.NewUserName}");
        //                    break;
        //            }

        //            return Task.CompletedTask;
        //        }
        //    )
        //);

        private static void SpawnClient()
        {
            // Data from Server
            PID client = context.Spawn(
                Props.FromFunc(
                    ctx =>
                    {
                        var remoteContext = ctx;
                        var msg = ctx.Message;

                        switch (ctx.Message)
                        {
                            case Connected connected:
                                Dispatcher.UIThread.InvokeAsync(new Action(() =>
                                {
                                    window.FindControl<TextBox>("tb_request").Text = connected.Message;
                                }));
                                break;
                            case Connect connect:
                                break;
                            case SayResponse sayResponse:
                                Console.WriteLine($"{sayResponse.UserName} {sayResponse.Message}");
                                break;
                            default:
                                break;
                        }

                        return Task.CompletedTask;
                    }
                )
            );

            new PeriodicTask().Run(() =>
            {
                context.Send(
                    PID.FromAddress($"{Globals.serverIP}:{Globals.serverPort}", $"{Globals.serverActorName}"),
                    new Connect
                    {
                        Sender = client
                    }
                );
            }, TimeSpan.FromMilliseconds(2000));
        }
    }
}
