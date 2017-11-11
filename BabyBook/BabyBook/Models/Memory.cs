using System;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Controllers;

namespace BabyBook.Models
{
	[DynamoDBTable(BabyMemoryConstants.MemoriesTableName)]
	public class Memory
	{
		[DynamoDBHashKey]
		public string Id { get; set; }
		[DynamoDBGlobalSecondaryIndexRangeKey("BabyIdIndex")]
		public string Description { get; set; }
		public DateTime Date { get; set; }
		[DynamoDBGlobalSecondaryIndexHashKey("BabyIdIndex")]
		public string BabyId { get; set; }

	}
}
