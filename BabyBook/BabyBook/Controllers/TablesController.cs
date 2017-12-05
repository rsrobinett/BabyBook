using System;
using System.Collections.Generic;
using System.Web.Http;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace BabyBook.Controllers
{
	public class TablesController : ApiController
	{
		private readonly AmazonDynamoDBClient _client;

		public TablesController() : this(new LocalClientContext())
		{
		}

		private TablesController(AmazonDynamoDBClient client)
		{
			_client = client;
		}

		// GET api/<controller>
		public IEnumerable<string> Get()
		{

			var tableList = _client.ListTables();
			return tableList.TableNames;
		}

		// GET api/<controller>/5
		public DescribeTableResponse Get(string id)
		{
			var response = _client.ListTables();
			return response.TableNames.Contains(id) 
				? _client.DescribeTable(id) 
				: null;
		}

		// POST api/<controller>
		public void Post([FromBody]string tableName)
		{
			var response = _client.ListTables();
			var lowerCaseTableName = tableName.ToLower();
			if (response.TableNames.Contains(lowerCaseTableName))
			{
				return;
			}

			switch (tableName.ToLower())
			{
				case BabyMemoryConstants.BabiesTableName:
					{
						CreateBabiesTable(_client);
						return;
					}
				case BabyMemoryConstants.MemoriesTableName:
					{
						CreateMemoriesTable(_client);
						return;
					}
				case BabyMemoryConstants.UsersTableName:
					{
						CreateUsersTable(_client);
						return;
					}
				default:
					return;
			}
		}

		// DELETE api/<controller>/5
		public void Delete(string id)
		{
			var response = _client.ListTables();
			var lowerCaseTableName = id.ToLower();
			if (!response.TableNames.Contains(lowerCaseTableName))
			{
				return;
			}
			_client.DeleteTable(lowerCaseTableName);
		}


		private void CreateBabiesTable(AmazonDynamoDBClient client)
		{
			// Build a 'CreateTableRequest' for the new table
			var createRequest = new CreateTableRequest
			{
				TableName = BabyMemoryConstants.BabiesTableName,
				AttributeDefinitions = new List<AttributeDefinition>()
				{
					new AttributeDefinition
					{
						AttributeName = "Id",
						AttributeType = ScalarAttributeType.S
					},
					new AttributeDefinition
					{
						AttributeName = "UserId",
						AttributeType = ScalarAttributeType.S
					}
				},
				KeySchema = new List<KeySchemaElement>()
				{
					new KeySchemaElement
					{
						AttributeName = "Id",
						KeyType = KeyType.HASH
					}
				},
				ProvisionedThroughput = new ProvisionedThroughput(1, 1),
				GlobalSecondaryIndexes =
				{
					new GlobalSecondaryIndex
					{
						IndexName = "UserIdIndex",
						ProvisionedThroughput = new ProvisionedThroughput(1, 1),
						Projection = new Projection {ProjectionType = ProjectionType.ALL},
						KeySchema =
						{
							new KeySchemaElement
							{
								AttributeName = "UserId",
								KeyType = KeyType.HASH
							}
						}

					}
				}
			};

			try
			{
				var createResponse = client.CreateTable(createRequest);
				Console.WriteLine("\n Table Created " + createResponse.TableDescription);
			}
			catch (Exception ex)
			{
				Console.WriteLine("\n Error: failed to create the new table; " + ex.Message);
			}
		}

		private void CreateUsersTable(AmazonDynamoDBClient client)
		{
			var provisionedThroughput = new ProvisionedThroughput(1, 1);

			var attributeDefinition = new List<AttributeDefinition>
			{
				new AttributeDefinition
				{
					AttributeName = "Id",
					AttributeType = ScalarAttributeType.S
				},
				new AttributeDefinition
				{
					AttributeName = "Email",
					AttributeType = ScalarAttributeType.S
				}
			};

			var keySchema = new List<KeySchemaElement>()
			{
				new KeySchemaElement
				{
					AttributeName = "Id",
					KeyType = KeyType.HASH
				}
			};

			var userEmailIndex = new GlobalSecondaryIndex()
			{
				IndexName = "UserEmailIndex",
				ProvisionedThroughput = provisionedThroughput,
				Projection = new Projection { ProjectionType = ProjectionType.ALL },
				KeySchema =
				{
					new KeySchemaElement
					{
						AttributeName = "Email",
						KeyType = KeyType.HASH
					}
				}

			};
			
			var createRequest = new CreateTableRequest
			{
				TableName = BabyMemoryConstants.UsersTableName,
				AttributeDefinitions = attributeDefinition,
				ProvisionedThroughput = provisionedThroughput,
				KeySchema = keySchema,
				GlobalSecondaryIndexes = { userEmailIndex }
			};

			try
			{
				var createResponse = client.CreateTable(createRequest);
				Console.WriteLine("\n Table Created " + createResponse.TableDescription);
			}
			catch (Exception ex)
			{
				Console.WriteLine("\n Error: failed to create the new table; " + ex.Message);
			}
		}

		private void CreateMemoriesTable(AmazonDynamoDBClient client)
		{
			var provisionedThroughput = new ProvisionedThroughput(1, 1);

			var attributeDefinition = new List<AttributeDefinition>
			{
				new AttributeDefinition
				{
					AttributeName = "Id",
					AttributeType = ScalarAttributeType.S
				},
				new AttributeDefinition
				{
					AttributeName = "BabyId",
					AttributeType = ScalarAttributeType.S
				},
				new AttributeDefinition
				{
					AttributeName = "Description",
					AttributeType = ScalarAttributeType.S
				}
			};

			var keySchema = new List<KeySchemaElement>()
			{
				new KeySchemaElement
				{
					AttributeName = "Id",
					KeyType = KeyType.HASH
				}
			};

			var babyIdIndex = new GlobalSecondaryIndex()
			{
				IndexName = "BabyIdIndex",
				ProvisionedThroughput = provisionedThroughput,
				Projection = new Projection { ProjectionType = ProjectionType.ALL },
				KeySchema =
				{
					new KeySchemaElement
					{
						AttributeName = "BabyId",
						KeyType = KeyType.HASH
					},
					new KeySchemaElement
					{
						AttributeName = "Description",
						KeyType = KeyType.RANGE
					}
				}

			};
			

			// Build a 'CreateTableRequest' for the new table

			var createRequest = new CreateTableRequest
			{
				TableName = BabyMemoryConstants.MemoriesTableName,
				AttributeDefinitions = attributeDefinition,
				ProvisionedThroughput = provisionedThroughput,
				KeySchema = keySchema,
				GlobalSecondaryIndexes = {babyIdIndex} 
			};

			try
			{
				var createResponse = client.CreateTable(createRequest);
				Console.WriteLine("\n Table Created " + createResponse.TableDescription);
			}
			catch (Exception ex)
			{
				Console.WriteLine("\n Error: failed to create the new table; " + ex.Message);
			}
		}
	}
}
