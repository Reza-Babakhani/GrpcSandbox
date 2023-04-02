using Greet;
using Grpc.Core;

namespace GrpcClient
{
	internal class Program
	{
		const string server = "127.0.0.1:50051";
		static async Task Main(string[] args)
		{
			Channel channel = new Channel(server, ChannelCredentials.Insecure);

			await channel.ConnectAsync().ContinueWith((t) =>
				{
					if (t.Status == TaskStatus.RanToCompletion)
					{
						Console.WriteLine("Connected to server.");
					}
				});


			var client = new GreetingService.GreetingServiceClient(channel);



			//UnaryCall(client);
			//await ServerStreamingCall(client);
			//await ClientStreamingCall(client);
			//await BiDirectionalStreamingCall(client);


			UnaryWithListCall(client);


			channel.ShutdownAsync().Wait();
			Console.ReadKey();

		}


		static void UnaryCall(GreetingService.GreetingServiceClient client)
		{
			Console.WriteLine("Unary Request-------------");
			#region Unary
			var req = new GreetingRequest()
			{
				Greeting = new()
				{
					FirstName = "Reza",
					LastName = "Babakhani"
				}
			};

			var unaryResponse = client.Greet(req);
			Console.WriteLine(unaryResponse.Result);
			#endregion
		}

		static async Task ServerStreamingCall(GreetingService.GreetingServiceClient client)
		{
			Console.WriteLine("Server Streaming Request-------------");

			#region Server Streaming
			var req2 = new GreetingManyTimesRequest()
			{
				Greeting = new()
				{
					FirstName = "Reza",
					LastName = "Babakhani"
				}
			};

			var ssResponse = client.GreetManyTimes(req2);

			while (await ssResponse.ResponseStream.MoveNext())
			{
				Console.WriteLine(ssResponse.ResponseStream.Current.Result);
				await Task.Delay(200);
			}
			#endregion
		}


		static async Task ClientStreamingCall(GreetingService.GreetingServiceClient client)
		{
			Console.WriteLine("Client Streaming Request-------------");

			#region Client Streaming

			var stream = client.LongGreet();

			foreach (int i in Enumerable.Range(0, 10))
			{

				Console.WriteLine("Sending Data: " + i);
				LongGreetingRequest req3 = new LongGreetingRequest()
				{
					Greeting = new()
					{
						FirstName = "Reza",
						LastName = "Babakhani " + i
					}
				};

				await stream.RequestStream.WriteAsync(req3);
			}

			await stream.RequestStream.CompleteAsync();

			Console.WriteLine(stream.ResponseAsync.Result.Result);

			#endregion
		}


		static async Task BiDirectionalStreamingCall(GreetingService.GreetingServiceClient client)
		{
			Console.WriteLine("Bi Directional Streaming Request-------------");

			#region Bi Directional Streaming

			var biStream = client.GreetEveryone();

			foreach (int i in Enumerable.Range(0, 10))
			{

				var req4 = new GreetEveryoneRequest()
				{
					Greeting = new()
					{
						FirstName = "Reza",
						LastName = "Babakhani " + i
					}
				};

				await biStream.RequestStream.WriteAsync(req4);

				if (await biStream.ResponseStream.MoveNext())
				{
					Console.WriteLine(biStream.ResponseStream.Current.Result);
				}
			}

			#endregion

		}


		static void UnaryWithListCall(GreetingService.GreetingServiceClient client)
		{
			Console.WriteLine("List Call--------------");

			var response = client.GreetList(new());

			foreach (var item in response.GreetList)
			{
				Console.WriteLine(item);

			}
		}
	}
}