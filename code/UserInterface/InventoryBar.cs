using System;
using System.Collections.Generic;
using System.Linq;
using Murder.Weapons;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Murder.UserInterface
{
	public class InventoryBar : Panel
	{
		private List<InventoryColumn> _columns = new();
		private List<MurderWeapon> _weapons = new();
		private MurderWeapon _selectedWeapon;
		
		public bool IsOpen { get; set; }

		public InventoryBar()
		{
			StyleSheet.Load( "/UserInterface/InventoryBar.scss" );

			for ( var i = 0; i < 6; i++ )
			{
				_columns.Add( new InventoryColumn( i, this ) );
			}
		}

		public override void Tick()
		{
			base.Tick();
			
			SetClass( "active", IsOpen );

			if ( Local.Pawn is not MurderPlayer player ) return;

			_weapons.Clear();
			_weapons.AddRange( player.Children.Select( x => x as MurderWeapon ).Where( x => x.IsValid() ) );

			foreach ( var weapon in _weapons )
				_columns[weapon.Bucket].UpdateWeapon( weapon );
		}

		[Event.BuildInput]
		public void ClientInput( InputBuilder input )
		{
			bool wantOpen = IsOpen;

			wantOpen = wantOpen || input.MouseWheel != 0;
			wantOpen = wantOpen || input.Pressed( InputButton.Slot1 );
			wantOpen = wantOpen || input.Pressed( InputButton.Slot2 );
			wantOpen = wantOpen || input.Pressed( InputButton.Slot3 );
			wantOpen = wantOpen || input.Pressed( InputButton.Slot4 );
			wantOpen = wantOpen || input.Pressed( InputButton.Slot5 );
			wantOpen = wantOpen || input.Pressed( InputButton.Slot6 );

			if ( _weapons.Count == 0 )
			{
				IsOpen = false;
				return;
			}

			if ( IsOpen != wantOpen )
			{
				_selectedWeapon = Local.Pawn.ActiveChild as MurderWeapon;
				IsOpen = true;
			}

			if ( !IsOpen ) return;

			if ( input.Down( InputButton.Attack1 ) )
			{
				input.SuppressButton( InputButton.Attack1 );
				input.ActiveChild = _selectedWeapon;
				
				IsOpen = false;
				Sound.FromScreen( "dm.ui_select" );
				
				return;
			}

			var oldSelected = _selectedWeapon;
			int selectedIndex = _weapons.IndexOf( _selectedWeapon );

			selectedIndex = SlotPressInput( input, selectedIndex );

			selectedIndex += input.MouseWheel;
			selectedIndex = selectedIndex.UnsignedMod( _weapons.Count );

			_selectedWeapon = _weapons[selectedIndex];

			for ( var i = 0; i < 6; i++ )
				_columns[i].TickSelection( _selectedWeapon );

			input.MouseWheel = 0;

			if ( oldSelected != _selectedWeapon )
				Sound.FromScreen( "dm.ui_tap" );
		}

		private int SlotPressInput( InputBuilder input, int index )
		{
			int columnInput = -1;

			if ( input.Pressed( InputButton.Slot1 ) ) columnInput = 0;
			if ( input.Pressed( InputButton.Slot2 ) ) columnInput = 1;
			if ( input.Pressed( InputButton.Slot3 ) ) columnInput = 2;
			if ( input.Pressed( InputButton.Slot4 ) ) columnInput = 3;
			if ( input.Pressed( InputButton.Slot5 ) ) columnInput = 4;
			if ( input.Pressed( InputButton.Slot6 ) ) columnInput = 5;

			if ( columnInput == -1 ) return index;

			if ( _selectedWeapon.IsValid() && _selectedWeapon.Bucket == columnInput )
				return NextInBucket();

			var firstOfColumn = _weapons.Where( x => x.Bucket == columnInput ).OrderBy( x => x.BucketWeight )
				.FirstOrDefault();

			return firstOfColumn is null ? index : _weapons.IndexOf( firstOfColumn );
		}

		private int NextInBucket()
		{
			Assert.NotNull( _selectedWeapon );

			MurderWeapon first = null;
			MurderWeapon prev = null;

			foreach ( var weapon in _weapons.Where( x => x.Bucket == _selectedWeapon.Bucket )
				.OrderBy( x => x.BucketWeight ) )
			{
				first ??= weapon;
				if ( prev == _selectedWeapon ) return _weapons.IndexOf( weapon );
				prev = weapon;
			}

			return _weapons.IndexOf( first );
		}
	}

	public class InventoryColumn : Panel
	{
		public int Column { get; set; }
		public bool IsSelected { get; set; }
		public Label Header { get; set; }
		public int SelectedIndex { get; set; }

		private List<InventoryIcon> _icons { get; set; } = new();
		
		public InventoryColumn( int index, InventoryBar parent )
		{
			Parent = parent;
			Column = index;
			Header = Add.Label( $"{index + 1}", "slot-number" );
		}

		public void UpdateWeapon( MurderWeapon weapon )
		{
			var icon = ChildrenOfType<InventoryIcon>().FirstOrDefault( x => x.Weapon == weapon );
			if ( icon is not null ) return;

			_icons.Add( new InventoryIcon( weapon ) {Parent = this} );
		}

		public void TickSelection( MurderWeapon weapon )
		{
			SetClass( "active", weapon?.Bucket == Column );

			foreach ( var icon in _icons )
				icon.TickSelection( weapon );
		}
	}

	public class InventoryIcon : Panel
	{
		public MurderWeapon Weapon { get; set; }
		public Panel Icon { get; set; }
		
		public InventoryIcon( MurderWeapon weapon )
		{
			Weapon = weapon;
			Icon = Add.Panel( "icon" );
			
			AddClass( weapon.ClassInfo.Name );
		}

		public void TickSelection( MurderWeapon weapon )
		{
			SetClass( "active", weapon == Weapon );
			SetClass( "empty", !Weapon.IsValid() );
		}

		public override void Tick()
		{
			base.Tick();
			
			if ( !Weapon.IsValid() || Weapon.Owner != Local.Pawn )
				Delete();
		}
	}
}
