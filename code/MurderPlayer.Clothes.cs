using System.Threading.Tasks;
using Murder.Rounds;
using Sandbox;

namespace Murder
{
	public class Clothing : ModelEntity
	{
		public Clothing() : base() { }
		
		public Clothing( MurderPlayer player, string model, bool addColor = false )
		{
			SetModel( model );
			SetParent( player, true );

			EnableShadowInFirstPerson = true;
			EnableHideInFirstPerson = true;
		}

	}
	
	public partial class MurderPlayer
	{
		public Clothing Coat { get; set; }
		private Clothing _shoes;
		private Clothing _hair;
		
		public void Dress()
		{
			Strip();

			Coat = new Clothing( this, "models/citizen_clothes/jacket/labcoat.vmdl", true );
			_shoes = new Clothing( this, "models/citizen_clothes/shoes/shoes.workboots.vmdl" );
			_hair = new Clothing( this, "models/citizen_clothes/hair/hair_malestyle02.vmdl" );
		}

		public void Strip()
		{
			Coat?.Delete();
			_shoes?.Delete();
			_hair?.Delete();
		}
	}
}
