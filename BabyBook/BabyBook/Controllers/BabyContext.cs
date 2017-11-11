using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace BabyBook.Controllers
{
	public class BabyContext : DynamoDBContext
	{
		public BabyContext() : this(new LocalClientContext())
		{
			
		}

		public BabyContext(AmazonDynamoDBClient client) :base(client)
		{
			
		}
	}
}