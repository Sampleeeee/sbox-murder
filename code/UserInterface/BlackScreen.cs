using Sandbox;
using Sandbox.UI;

namespace Murder.UserInterface
{
	[UseTemplate("/UserInterface/BlackScreen.htm")]
	public partial class BlackScreen : Panel
	{
		public static BlackScreen Current { get; set; }
		
		public Label Title { get; set; }
		public Label Description { get; set; }

		public BlackScreen()
		{
			Current = this;

			Title.Text = "Winner";
			Hide();
		}
		
		public void Hide()
		{
			AddClass( "hide" );
		}

		public void Show()
		{
			RemoveClass( "hide" );
		}

		[ClientRpc]
		public static void ShowTitle( string message, Color color )
		{
			Log.Info( "Got Client Rpc" );
			
			Current.Title.Text = message;
			Current.Title.Style.FontColor = color;
			Current.Show();
		}

		[ClientRpc]
		public static void ShowDescription( string message, Color color )
		{
			Current.Description.Text = message;
			Current.Description.Style.FontColor = color;
			Current.Show();
		}

		[ClientRpc]
		public static void HideScreen()
		{
			Current.Hide();
		}
	}
}
