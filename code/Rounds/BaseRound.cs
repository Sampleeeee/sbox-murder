using System;
using System.Collections.Generic;
using Sandbox;

namespace Murder.Rounds
{
	public abstract partial class BaseRound : NetworkComponent
	{
		public abstract int RoundDuration { get; }
		public abstract string RoundName { get; }

		public MurderGame Game => 
			Sandbox.Game.Current as MurderGame;

		public List<MurderPlayer> Players { get; set; } = new();
		
		public float RoundEndTime { get; set; }
		public float TimeLeft => RoundEndTime - Time.Now;

		[Net] public string TimeLeftFormatted { get; set; }

		public void Start()
		{
			if ( Host.IsServer && RoundDuration > 0 )
				RoundEndTime = Time.Now + RoundDuration;

			OnStart();
		}

		public void Finish()
		{
			if ( Host.IsServer )
			{
				RoundEndTime = 0f;
				Players.Clear();
			}

			OnFinish();
		}

		public void AddPlayer( MurderPlayer player )
		{
			Host.AssertServer();

			if ( !Players.Contains( player ) )
				Players.Add( player );
		}

		public virtual void OnPlayerLeave( MurderPlayer player )
		{
			if ( Players.Contains( player ) )
				Players.Remove( player );
		}

		public virtual void OnSecond()
		{
			if ( !Host.IsServer )
				return;

			if ( RoundEndTime > 0 && Time.Now >= RoundEndTime )
			{
				RoundEndTime = 0f;
				OnTimeUp();
			}
			else
				TimeLeftFormatted = TimeSpan.FromSeconds( TimeLeft ).ToString( "mm\\:ss" );
		}
		
		public virtual void OnPlayerSpawn( MurderPlayer player ) { }
		public virtual void OnPlayerKilled( MurderPlayer player ) { }

		public virtual void OnTick() { }
		protected virtual void OnTimeUp() { }
		protected virtual void OnStart() { }
		protected virtual void OnFinish() { }
	}
}
