using System.Linq;
using Murder.Rounds;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Murder.UserInterface
{
	public partial class MurderChat : Panel
	{
		public static MurderChat Current { get; set; }
		
		public Panel Canvas { get; protected set; }
		public TextEntry Input { get; protected set; }

		public MurderChat()
		{
			Current = this;
			
			StyleSheet.Load( "/UserInterface/MurderChat.scss" );

			Canvas = Add.Panel( "chat_canvas" );

			Input = Add.TextEntry( "" );
			
			Input.AddEventListener( "onsubmit", () => Submit() );
			Input.AddEventListener( "onblur", () => Close() );

			Input.AcceptsFocus = true;
			Input.AllowEmojiReplace = true;

			Sandbox.Hooks.Chat.OnOpenChat += Open;
		}

		private void Open()
		{
			Log.Info( "OPening" );
			
			AddClass( "open" );
			Input.Focus();
		}

		private void Close()
		{
			RemoveClass( "open" );
			Input.Blur();
		}

		private void Submit()
		{
			Close();

			string message = Input.Text.Trim();
			Input.Text = string.Empty;

			if ( string.IsNullOrWhiteSpace( message ) ) return;

			Say( message );
		}

		public void AddEntry( string name, string message, string avatar = null )
		{
			var entry = Canvas.AddChild<MurderChatEntry>();

			entry.Message.Text = message;
			entry.NameLabel.Text = name;
			entry.Avatar.SetTexture( avatar );

			entry.SetClass( "noname", string.IsNullOrEmpty( name ) );
			entry.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );
		}

		public void AddEntry( string name, string message, Color color )
		{
			var entry = Canvas.AddChild<MurderChatEntry>();

			entry.Message.Text = message;
			entry.NameLabel.Text = name;
			entry.NameLabel.Style.FontColor = color;

			entry.SetClass( "noname", string.IsNullOrEmpty( name ) );
			entry.SetClass( "noavatar", true );
		}

		[ClientRpc]
		public static void AddChatEntry( string name, string message, string avatar = null )
		{
			Current?.AddEntry( name, message, avatar );
		}

		[ClientRpc]
		public static void AddChatEntry( string name, string message, Color color )
		{
			Current?.AddEntry( name, message, color );
		}

		[ClientRpc]
		public static void AddInformation( string message )
		{
			Current?.AddEntry( null, message );
		}

		[ServerCmd( "say" )]
		public static void Say( string message )
		{
			Assert.NotNull( ConsoleSystem.Caller );
			
			if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
				return;
			
			var game = Game.Current as MurderGame;
			var caller = ConsoleSystem.Caller.Pawn as MurderPlayer;
			
			Assert.NotNull( game );
			Assert.NotNull( caller );
			
			if ( game.Round is Playing )
			{
				if ( caller.Dead )
					AddChatEntry( To.Multiple( Client.All.Where( x => (x as MurderPlayer).Dead ) ),
						ConsoleSystem.Caller.Name,
						message, $"avatar:{ConsoleSystem.Caller.SteamId}" );
				else
					AddChatEntry( To.Everyone, caller.FakeName, message, caller.FakeColor.ToColor() );
			}
			else
			{
				AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message,
					$"avatar:{ConsoleSystem.Caller.SteamId}" );
			}
		}
	}

	public class MurderChatEntry : Panel
	{
		public Label NameLabel { get; internal set; }
		public Label Message { get; internal set; }
		public Image Avatar { get; internal set; }

		public RealTimeSince TimeSinceBorn = 0;

		public MurderChatEntry()
		{
			Avatar = Add.Image();
			NameLabel = Add.Label( "Name", "name" );
			Message = Add.Label( "Message", "message" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( TimeSinceBorn > 10 )
			{
				Delete();
			}
		}
	}
}
