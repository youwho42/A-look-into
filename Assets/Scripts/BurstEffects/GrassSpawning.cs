using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

using math = Unity.Mathematics.math;
using random = Unity.Mathematics.Random;
using Unity.Jobs;
using UnityEngine.Tilemaps;

public class GrassSpawning : MonoBehaviour
{
    public int maxAmount;
    private NativeArray<Vector3> grassPositions;

    private TransformAccessArray grassTransformAccessArray;

    private void Start()
    {
        grassPositions = new NativeArray<Vector3>(maxAmount, Allocator.Persistent);
        grassTransformAccessArray = new TransformAccessArray(maxAmount);
    }

}

