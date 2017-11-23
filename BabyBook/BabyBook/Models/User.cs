using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Controllers;

namespace BabyBook.Models
{
	[DynamoDBTable(BabyMemoryConstants.UsersTableName)]
	public class User
	{
		[DynamoDBHashKey]
		public string Id { get; set; }
		[DynamoDBGlobalSecondaryIndexHashKey("UserEmailIndex")]
		public string EmailAddress { get; set; }
		public List<string> BabyIds { get; set; }

	}
}
