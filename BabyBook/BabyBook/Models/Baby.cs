using System;
using Amazon.DynamoDBv2.DataModel;

namespace BabyBook.Models
{
	[DynamoDBTable("Babies")]
	public class Baby
	{
		[DynamoDBHashKey]
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DateOfBirth { get; set; }

	}
}
