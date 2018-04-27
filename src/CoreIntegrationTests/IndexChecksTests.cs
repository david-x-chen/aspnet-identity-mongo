namespace IntegrationTests
{
	using System;
    using System.Collections.Generic;
    using System.Linq;
	using Microsoft.AspNetCore.Identity.MongoDB;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
	using NUnit.Framework;

	[TestFixture]
	public class IndexChecksTests : UserIntegrationTestsBase
	{
		[Test]
		public void EnsureUniqueIndexes()
		{
			EnsureUniqueIndex<IdentityUser>(IndexChecks.OptionalIndexChecks.EnsureUniqueIndexOnUserName, "UserName");
			EnsureUniqueIndex<IdentityUser>(IndexChecks.OptionalIndexChecks.EnsureUniqueIndexOnEmail, "Email");
			EnsureUniqueIndex<IdentityRole>(IndexChecks.OptionalIndexChecks.EnsureUniqueIndexOnRoleName, "Name");

			EnsureUniqueIndex<IdentityUser>(IndexChecks.EnsureUniqueIndexOnNormalizedUserName, "NormalizedUserName");
			EnsureUniqueIndex<IdentityUser>(IndexChecks.EnsureUniqueIndexOnNormalizedEmail, "NormalizedEmail");
			EnsureUniqueIndex<IdentityRole>(IndexChecks.EnsureUniqueIndexOnNormalizedRoleName, "NormalizedName");
		}

		private void EnsureUniqueIndex<TCollection>(Action<IMongoCollection<TCollection>> addIndex, string indexedField)
		{
			var testCollectionName = "indextest";
			Database.DropCollection(testCollectionName);
			var testCollection = DatabaseNewApi.GetCollection<TCollection>(testCollectionName);

			addIndex(testCollection);

			var legacyCollectionInterface = Database.GetCollection<TCollection>(testCollectionName);
			var indexManager = legacyCollectionInterface.Indexes;
			var indexList = indexManager.List();
			var actualIndexList = new List<MongoDbIndex>();

			while(indexList.MoveNext())
			{
				var currentIndex = indexList.Current;
				foreach(var doc in currentIndex)
				{
					var acutalIndex = BsonSerializer.Deserialize<MongoDbIndex>(doc);
					
					if (currentIndex != null)
					{
						actualIndexList.Add(acutalIndex);
					}
				}
			}

			var index = actualIndexList
				.Where(i => i.unique)
				.Where(i => i.key.Count() == 1)
				.FirstOrDefault(i => i.key.ContainsKey(indexedField));
			var failureMessage = $"No unique index found on {indexedField}";
			Expect(index, Is.Not.Null, failureMessage);
			Expect(index.key.Count(), Is.EqualTo(1), failureMessage);
		}

		internal class MongoDbIndex
		{
			public int v {get;set;}

			public string name {get;set;}

			public bool unique {get;set;}

			public string ns {get;set;}

			public Dictionary<string, int> key {get;set;}
		}
	}
}