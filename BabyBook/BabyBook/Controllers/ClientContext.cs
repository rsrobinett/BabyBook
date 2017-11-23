using Amazon;
using Amazon.DynamoDBv2;

namespace BabyBook.Controllers
{
	public class LocalClientContext : AmazonDynamoDBClient
	{
		public LocalClientContext():base(
			new AmazonDynamoDBConfig {ServiceURL = "http://localhost:8001"})
		{
		}
	}

	public class DevClientContext : AmazonDynamoDBClient
	{
		public DevClientContext() : base(new AmazonDynamoDBConfig { RegionEndpoint = RegionEndpoint.EUWest2 })
		{
		}

	}
}