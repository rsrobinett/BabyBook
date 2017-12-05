using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using BabyBook.Controllers;

namespace BabyBook.Models
{
	/// <summary>
	/// Authorized User
	/// </summary>
	[DynamoDBTable(BabyMemoryConstants.UsersTableName)]
	public class User
	{
		/// <summary>
		/// Unique Identifier
		/// </summary>
		[DynamoDBHashKey]
		public string Id { get; set; }
		/// <summary>
		/// Authorized through Identity Provider
		/// </summary>
		[DynamoDBGlobalSecondaryIndexHashKey("UserEmailIndex")]
		public string Email { get; set; }
		///// <summary>
		///// Populated through baby controller.  Left blank on creation of user.
		///// </summary>
		//public List<string> BabyIds { get; set; }
		/// <summary>
		/// Value as admin is treated separately, otherwise all other values are currently ignored. 
		/// </summary>
		public string Role { get; set; }

	}
}
