using BabyBook;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace BabyBook
{
	using Owin;

	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureOAuth(app);
		}
	}
}