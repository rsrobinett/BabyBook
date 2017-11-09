using System;
using Amazon.DynamoDBv2.DataModel;

namespace BabyBook.Models
{
	[DynamoDBTable("Memories")]
	public class Memory
	{
		[DynamoDBHashKey]
		public string Id { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set; }
		public string BabyId { get; set; }
	}
}
