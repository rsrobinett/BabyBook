﻿using System;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Controllers;

namespace BabyBook.Models
{
	[DynamoDBTable(BabyMemoryConstants.BabiesTableName)]
	public class Baby
	{
		[DynamoDBHashKey]
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DateOfBirth { get; set; }

	}
}
