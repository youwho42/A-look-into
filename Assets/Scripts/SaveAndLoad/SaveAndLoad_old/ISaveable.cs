using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    object CaptureState();

    void RestoreState(object state);
}
