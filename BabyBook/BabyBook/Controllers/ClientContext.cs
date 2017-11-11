using Amazon;
using Amazon.DynamoDBv2;

namespace BabyBook.Controllers
{
	public class LocalClientContext : AmazonDynamoDBClient
	{
		public LocalClientContext():base(
			new AmazonDynamoDBConfig {ServiceURL = "http://localhost:8001"})
		{
			/*
			AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
			// Set the endpoint URL
			clientConfig.ServiceURL = "http://localhost:8000";
			return new AmazonDynamoDBConfig(clientConfig)
			*/

		}
	}

	public class DevClientContext : AmazonDynamoDBClient
	{
		public DevClientContext() : base(new AmazonDynamoDBConfig { RegionEndpoint = RegionEndpoint.EUWest2 })
		{
			/*
			AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
			// This client will access the US East 1 region.
			clientConfig.RegionEndpoint = RegionEndpoint.EUWest2;
			AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);
			*/
		}

	}
}