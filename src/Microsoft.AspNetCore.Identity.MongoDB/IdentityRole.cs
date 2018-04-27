namespace Microsoft.AspNetCore.Identity.MongoDB
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using global::MongoDB.Bson;
	using global::MongoDB.Bson.Serialization.Attributes;

	public class IdentityRole
	{
		public IdentityRole()
		{
            Id = ObjectId.GenerateNewId().ToString();

            Claims = new List<IdentityRoleClaim>();
		}

		public IdentityRole(string roleName) : this()
		{
			Name = roleName;
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string Name { get; set; }

		public string NormalizedName { get; set; }

        [BsonIgnoreIfNull]
        public virtual List<IdentityRoleClaim> Claims { get; set; }

        public virtual void AddClaim(Claim claim)
        {
            Claims.Add(new IdentityRoleClaim(claim));
        }

        public virtual void RemoveClaim(Claim claim)
        {
            Claims.RemoveAll(c => c.Type == claim.Type && c.Value == claim.Value);
        }

        public virtual void ReplaceClaim(Claim existingClaim, Claim newClaim)
        {
            var claimExists = Claims
                .Any(c => c.Type == existingClaim.Type && c.Value == existingClaim.Value);
            if (!claimExists)
            {
                // note: nothing to update, ignore, no need to throw
                return;
            }
            RemoveClaim(existingClaim);
            AddClaim(newClaim);
        }

		public override string ToString() => Name;
	}
}