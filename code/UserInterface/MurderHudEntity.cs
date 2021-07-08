using Sandbox;
using Sandbox.UI;

namespace Murder.UserInterface
{
	public class MurderHudEntity : HudEntity<RootPanel>
	{
		public MurderHudEntity Current { get; set; }
		
		public MurderHudEntity()
		{
			Current = this;
			
			if ( IsClient )
				RootPanel.SetTemplate( "/UserInterface/MurderHudEntity.htm" );
		}
	}
}
