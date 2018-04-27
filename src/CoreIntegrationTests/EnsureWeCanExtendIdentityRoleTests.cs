namespace IntegrationTests
{
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    using NUnit.Framework;

	[TestFixture]
	public class EnsureWeCanExtendIdentityRoleTests : UserIntegrationTestsBase
	{
		private RoleManager<ExtendedIdentityRole> _Manager;
		private ExtendedIdentityRole _Role;

		public class ExtendedIdentityRole : IdentityRole
		{
			public string ExtendedField { get; set; }
		}

		[SetUp]
		public void BeforeEachTestAfterBase()
		{
			_Manager = CreateServiceProvider<IdentityUser, ExtendedIdentityRole>()
				.GetService<RoleManager<ExtendedIdentityRole>>();
			_Role = new ExtendedIdentityRole
			{
				Name = "admin"
			};
		}

		[Test]
		public async Task Create_ExtendedRoleType_SavesExtraFields()
		{
			_Role.ExtendedField = "extendedField";

			await _Manager.CreateAsync(_Role);

			var builder = Builders<IdentityRole>.Filter;
			var filter = builder.Empty;

			var savedRole = Roles.FindAsync<ExtendedIdentityRole>(filter).Result.Single();
			Expect(savedRole.ExtendedField, Is.EqualTo("extendedField"));
		}

		[Test]
		public async Task Create_ExtendedRoleType_ReadsExtraFields()
		{
			_Role.ExtendedField = "extendedField";

			await _Manager.CreateAsync(_Role);

			var savedRole = await _Manager.FindByIdAsync(_Role.Id);
			Expect(savedRole.ExtendedField, Is.EqualTo("extendedField"));
		}
	}
}