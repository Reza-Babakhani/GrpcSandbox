using Greet;
using Grpc.Core;
using Server.Services;

namespace GrpcServer
{
	internal class Program
	{
		const int Port = 50051;
		static void Main(string[] args)
		{

			Grpc.Core.Server? server = null;

			try
			{
				server = new()
				{
					Services = {
						GreetingService.BindService(new GreetingServiceImpl()),

					},
					Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure), }
				};

				server.Start();
				Console.WriteLine("Grpc Server is running...");
				Console.ReadKey();
			}
			catch (IOException e)
			{

				Console.WriteLine(e.ToString());
			}
			finally
			{
				if (server != null)
				{
					server.ShutdownAsync().Wait();
				}
			}
		}
	}
}