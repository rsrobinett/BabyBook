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
		public string Email { get; set; }
		public List<string> BabyIds { get; set; }
		public string Role { get; set; }

	}
}
