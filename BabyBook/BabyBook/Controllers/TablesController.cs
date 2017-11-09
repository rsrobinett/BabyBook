using System;
using System.Collections.Generic;
using System.Web.Http;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace BabyBook.Controllers
{
	class TableController : ApiController
	{
		private readonly AmazonDynamoDBClient _client;
		public TableController() : this(new LocalClientContext())
		{
		}

		public TableController(AmazonDynamoDBClient client)
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
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<controller>
		public void Post([FromBody]string tableName)
		{

			if (tableName.ToLower() == "babies")
			{
				var response = _client.ListTables();
				if (!response.TableNames.Contains("Babies"))
				{
					CreateBabiesTable(_client);
				}
			}

			if (tableName.ToLower() == "memories")
			{
				var response = _client.ListTables();
				if (!response.TableNames.Contains("Memories"))
				{
					CreateMemoriesTable(_client);
				}
			}
		}

		// PUT api/<controller>/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/<controller>/5
		public void Delete(int id, [FromBody]string tableName)
		{
			if (tableName.ToLower() == "babies")
			{
				var response = _client.ListTables();
				if (response.TableNames.Contains("Babies"))
				{
					_client.DeleteTable("Babies");
				}
			}

		}


		private void CreateBabiesTable(AmazonDynamoDBClient client)
		{
			// Build a 'CreateTableRequest' for the new table
			var createRequest = new CreateTableRequest
			{
				TableName = "Babies",
				AttributeDefinitions = new List<AttributeDefinition>()
				{
					new AttributeDefinition
					{
						AttributeName = "Id",
						AttributeType = ScalarAttributeType.S
					}/*,
					new AttributeDefinition
					{
						AttributeName = "LastName",
						AttributeType = ScalarAttributeType.S
					}
					,
					new AttributeDefinition
					{
						AttributeName = "FirstName",
						AttributeType = ScalarAttributeType.S
					},
					new AttributeDefinition
					{
						AttributeName = "DateOfBirth",
						AttributeType = ScalarAttributeType.S
					}*/
				},
				KeySchema = new List<KeySchemaElement>()
				{
					new KeySchemaElement
					{
						AttributeName = "Id",
						KeyType = KeyType.HASH
					}/*,

					new KeySchemaElement
					{
						AttributeName = "LastName",
						KeyType = KeyType.RANGE
					}*/
				},
				ProvisionedThroughput = new ProvisionedThroughput(1, 1),
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
			// Build a 'CreateTableRequest' for the new table
			var createRequest = new CreateTableRequest
			{
				TableName = "Memories",
				AttributeDefinitions = new List<AttributeDefinition>()
				{
					new AttributeDefinition
					{
						AttributeName = "Id",
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
