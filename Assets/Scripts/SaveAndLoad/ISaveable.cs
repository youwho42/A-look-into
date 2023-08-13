using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.SaveSystem
{
	public interface ISaveable
	{
		object CaptureState();

		void RestoreState(object state);
	}

}