using UnityEngine;

// Movement variables
namespace PMoveUtil{
	
	[System.Serializable]
	public class movevars_t
	{
		public float gravity = 20.0f;
		public float maxspeed = 320.0f;
		public float accelerate = 14.0f; // Run accel
		public float deaccelerate = 10.0f; // Run deaccel
		public float airaccelerate = 2.0f;
		public float airdeaccelerate = 2.0f;
		public float aircontrol = 0.75f;
		public float friction = 6.0f;
		public float jumpspeed = 8.0f;
		public float movespeed = 8.35f;
		public float entgravity = 1.0f;

		public float strafe_boost = 1.3f;

		public float waterfriction = 1.0f;
		public float wateraccelerate = 10.0f;
	};


	[System.Serializable]
	public class playermove_t {
		// player state
		public Vector3 origin = Vector3.zero;
		public Vector3 angles = Vector3.zero;
		public Vector3 velocity = Vector3.zero;

		public float waterjumptime = 0;
		public bool dead = false;
	};

	[System.Serializable]
	public class _cmd
	{
		public float fmove = 0;
		public float smove = 0;
		public float umove = 0;
		public float m_rotX = 0;
		public float m_rotY = 0;

		public bool wishjump = false;

	};

	[System.Serializable]
	public class _options
	{
		public float opt_m_sensX = 30.0f;
		public float opt_m_sensY = 30.0f;
		public float opt_cam_offset = 0.6f;

	};
}