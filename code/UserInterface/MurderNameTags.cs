using System;
using System.Collections.Generic;
using System.Linq;
using Murder.Rounds;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Murder.UserInterface
{
	public class BaseMurderNameTag : Panel
	{
		public Label Name { get; set; }
		public Image Avatar { get; set; }

		private MurderPlayer _player;

		public BaseMurderNameTag( MurderPlayer player )
		{
			_player = player;
			UpdateFromPlayer( player );
		}

		public void UpdateFromPlayer( MurderPlayer player )
		{
			if ( Game.Current is not MurderGame game ) return;

			Name ??= Add.Label();
			Avatar ??= Add.Image();

			if ( game.Round is Playing && !player.Dead )
			{
				Name.Text = $"{player.FakeName}";
				Name.Style.FontColor = player.FakeColor.ToColor();
				
				Avatar?.Delete();
				Avatar = null;
				Style.Dirty();
			}
			else if ( game.Round is not Playing )
			{
				Name.Text = $"{player.GetClientOwner().Name}";
				Avatar.SetTexture( $"avatar:{player.GetClientOwner().SteamId}" );
			}
		}
	}
	
	public class MurderNameTags : Panel
	{
		private Dictionary<MurderPlayer, BaseMurderNameTag> _activeTags = new();

		public float MaxDrawDistance = 400;
		public int MaxTagsToShow = 5;
		
		public MurderNameTags()
		{
			StyleSheet.Load( "/UserInterface/MurderNameTags.scss" );
		}

		public override void Tick()
		{
			base.Tick();

			var deleteList = new List<MurderPlayer>();
			deleteList.AddRange( _activeTags.Keys );

			int count = 0;
			foreach ( var player in Entity.All.OfType<MurderPlayer>().OrderBy( x => Vector3.DistanceBetween( x.Position, CurrentView.Position ) ) )
			{
				if ( UpdateNameTag( player ) )
				{
					deleteList.Remove( player );
					count++;
				}

				if ( count >= MaxTagsToShow )
					break;
			}

			foreach( var player in deleteList )
			{
				_activeTags[player].Delete();
				_activeTags.Remove( player );
			}
		}

		public virtual BaseMurderNameTag CreateNameTag( MurderPlayer player )
		{
			if ( player.GetClientOwner() is null )
				return null;

			var tag = new BaseMurderNameTag( player );
			tag.Parent = this;
			return tag;
		}

		public bool UpdateNameTag( MurderPlayer player )
		{
			// Don't draw local player
			if ( player == Local.Pawn )
				return false;

			if ( player.LifeState != LifeState.Alive )
				return false;

			//
			// Where we putting the label, in world coords
			//
			var head = player.GetAttachment( "hat" ) ?? new Transform( player.EyePos );

			var labelPos = head.Position + head.Rotation.Up * 5;


			//
			// Are we too far away?
			//
			float dist = labelPos.Distance( CurrentView.Position );
			if ( dist > MaxDrawDistance )
				return false;

			//
			// Are we looking in this direction?
			//
			var lookDir = (labelPos - CurrentView.Position).Normal;
			if ( CurrentView.Rotation.Forward.Dot( lookDir ) < 0.5 )
				return false;

			// TODO - can we see them


			MaxDrawDistance = 400;

			// Max Draw Distance


			var alpha = dist.LerpInverse( MaxDrawDistance, MaxDrawDistance * 0.1f, true );

			// If I understood this I'd make it proper function
			var objectSize = 0.05f / dist / (2.0f * MathF.Tan( (CurrentView.FieldOfView / 2.0f).DegreeToRadian() )) * 1500.0f;

			objectSize = objectSize.Clamp( 0.05f, 1.0f );

			if ( !_activeTags.TryGetValue( player, out var tag ) )
			{
				tag = CreateNameTag( player );
				if ( tag != null )
				{
					_activeTags[player] = tag;
				}
			}

			tag.UpdateFromPlayer( player );

			var screenPos = labelPos.ToScreen();

			tag.Style.Left = Length.Fraction( screenPos.x );
			tag.Style.Top = Length.Fraction( screenPos.y );
			tag.Style.Opacity = alpha;

			var transform = new PanelTransform();
			transform.AddTranslateY( Length.Fraction( -1.0f ) );
			transform.AddScale( objectSize );
			transform.AddTranslateX( Length.Fraction( -0.5f ) );

			tag.Style.Transform = transform;
			tag.Style.Dirty();

			return true;
		}
	}
}
