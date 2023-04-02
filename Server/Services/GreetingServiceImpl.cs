using Greet;
using Grpc.Core;
using static Greet.GreetingService;

namespace Server.Services
{
	internal class GreetingServiceImpl : GreetingServiceBase
	{
		public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
		{
			GreetingResponse response = new()
			{
				Result = String.Format("Hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName),
			};
			return Task.FromResult(response);
		}

		public override async Task GreetManyTimes(GreetingManyTimesRequest request, IServerStreamWriter<GreetingManyTimesResponse> responseStream, ServerCallContext context)
		{
			Console.WriteLine(request.ToString());

			GreetingManyTimesResponse response = new()
			{
				Result = String.Format("Hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName),
			};

			foreach (int i in Enumerable.Range(1, 5))
			{
				await responseStream.WriteAsync(response);
			}


			GreetingManyTimesResponse finalResponse = new()
			{
				Result = String.Format("Goodbye {0} {1}", request.Greeting.FirstName, request.Greeting.LastName),
			};

			await responseStream.WriteAsync(finalResponse);
		}

		public override async Task<LongGreetingResponse> LongGreet(IAsyncStreamReader<LongGreetingRequest> requestStream, ServerCallContext context)
		{

			while (await requestStream.MoveNext())
			{

				Console.WriteLine("Data Recived: {0} {1}", requestStream.Current.Greeting.FirstName, requestStream.Current.Greeting.LastName);

			}

			var response = new LongGreetingResponse()
			{
				Result = "Done"
			};
			return response;
		}

		public override async Task GreetEveryone(IAsyncStreamReader<GreetEveryoneRequest> requestStream, IServerStreamWriter<GreetEveryoneResponse> responseStream, ServerCallContext context)
		{
			while (await requestStream.MoveNext())
			{
				Console.WriteLine("Recived: " + requestStream.Current.Greeting.ToString());

				GreetEveryoneResponse response = new()
				{
					Result = "Response: " + requestStream.Current.Greeting.FirstName + " " + requestStream.Current.Greeting.LastName
				};

				await responseStream.WriteAsync(response);
			}


		}

		public override Task<GreetListResponse> GreetList(GreetListRequest request, ServerCallContext context)
		{
			var response = new GreetListResponse()
			{
				GreetList =
				{
					new Greeting() {FirstName="Reza",LastName="Babakhani"},
					new Greeting() {FirstName="2Reza",LastName="Babakhani"},
					new Greeting() {FirstName="3Reza",LastName="Babakhani"},
					new Greeting() {FirstName="4Reza",LastName="Babakhani"},
					new Greeting() {FirstName="5Reza",LastName="Babakhani"}

				}
			};

			return Task.FromResult(response);
		}
	}
}
