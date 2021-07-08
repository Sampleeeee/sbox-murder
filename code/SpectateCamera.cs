using Sandbox;

namespace Murder
{
	public class SpectateCamera : Camera
	{
		private Angles _lookAngles;
		private Vector3 _moveInput;
		private Vector3 _targetPos;
		private Rotation _targetRot;
		
		private float _moveSpeed;
		private float _fovOverride;
		private float _lerpMode = 0;

		public override void Activated()
		{
			base.Activated();

			_targetPos = CurrentView.Position;
			_targetRot = CurrentView.Rotation;

			Pos = _targetPos;
			Rot = _targetRot;
			_lookAngles = Rot.Angles();
			_fovOverride = 80;

			DoFPoint = 0f;
			DoFBlurSize = 0f;
		}

		public override void Update()
		{
			var player = Local.Client;
			if ( player is null ) return;

			var tr = Trace.Ray( Pos, Pos + Rot.Forward * 4096 ).UseHitboxes().Run();

			FieldOfView = _fovOverride;

			Viewer = null;

			float lerpTarget = tr.EndPos.Distance( Pos );
			DoFPoint = lerpTarget;

			FreeMove();
		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );

			_moveInput = input.AnalogMove;

			_moveSpeed = 1;
			if ( input.Down( InputButton.Run ) )
				_moveSpeed = 5;

			_lookAngles += input.AnalogLook * (_fovOverride / 80f);
			_lookAngles.roll = 0;

			input.Clear();
			input.StopProcessing = true;
		}

		private void FreeMove()
		{
			var move = _moveInput.Normal * 300 * RealTime.Delta * Rot * _moveSpeed;

			_targetRot = Rotation.From( _lookAngles );
			_targetPos += move;

			Pos = Vector3.Lerp( Pos, _targetPos, 10 * RealTime.Delta * (1 - _lerpMode) );
			Rot = Rotation.Slerp( Rot, _targetRot, 10 * RealTime.Delta * (1 - _lerpMode) );
		}
	}
}
