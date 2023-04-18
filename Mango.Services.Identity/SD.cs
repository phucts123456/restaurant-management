
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Mango.Services.Identity
{
	public static class SD
	{
		public const string Admin = "Admin";
		public const string Customer = "Customer";

		public static IEnumerable<IdentityResource> IdentityResources =>
			new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Email(),
				new IdentityResources.Profile()
			};//user infor

		public static IEnumerable<ApiScope> ApiScopes =>
			new List<ApiScope> {
				new ApiScope("mango", "Mango Server"),
				new ApiScope(name: "read",   displayName: "Read your data."),
				new ApiScope(name: "write",  displayName: "Write your data."),
				new ApiScope(name: "delete", displayName: "Delete your data.")
			};
		//Client app like web app or mobile app
		public static IEnumerable<Client> Clients =>
			new List<Client>
			{
				new Client
				{
					ClientId="client",
					ClientSecrets= { new Secret("secret".Sha256())},
					AllowedGrantTypes = GrantTypes.ClientCredentials,
					AllowedScopes={ "read", "write","profile"}
				},
				new Client
				{
					ClientId="mango",
					ClientSecrets= { new Secret("secret".Sha256())},
					AllowedGrantTypes = GrantTypes.Code,//how client will interact with token server
					RedirectUris={ "https://localhost:44387/signin-oidc" },
					PostLogoutRedirectUris={"https://localhost:44387/signout-callback-oidc" },
					AllowedScopes=new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						IdentityServerConstants.StandardScopes.Email,
						"mango"
					}
				},
			};
	}
}