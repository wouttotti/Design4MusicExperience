using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class MyoDataCreator : MonoBehaviour {

    public GameObject myo = null;

    public GameObject Orientation = null;
    public JointOrientation OrientationScript = null;

    public int ArmVertical;


	// Use this for initialization
	void Start () {
        OrientationScript = (JointOrientation)Orientation.GetComponent("JointOrientation");
	}
	
	// Update is called once per frame
	void Update () {
	}
}
