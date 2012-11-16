/*
 * -----------------------------------------------------------------
 * Copyright (c) 2012 Intel Corporation
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *
 *     * Redistributions in binary form must reproduce the above
 *       copyright notice, this list of conditions and the following
 *       disclaimer in the documentation and/or other materials provided
 *       with the distribution.
 *
 *     * Neither the name of the Intel Corporation nor the names of its
 *       contributors may be used to endorse or promote products derived
 *       from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE INTEL OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * EXPORT LAWS: THIS LICENSE ADDS NO RESTRICTIONS TO THE EXPORT LAWS OF
 * YOUR JURISDICTION. It is licensee's responsibility to comply with any
 * export regulations applicable in licensee's jurisdiction. Under
 * CURRENT (May 2000) U.S. export regulations this software is eligible
 * for export from the U.S. and can be downloaded by or otherwise
 * exported or reexported worldwide EXCEPT to U.S. embargoed destinations
 * which include Cuba, Iraq, Libya, North Korea, Iran, Syria, Sudan,
 * Afghanistan and any other country to which the U.S. has embargoed
 * goods and services.
 * -----------------------------------------------------------------
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;
using log4net;
using OpenMetaverse;
using OpenSim.Framework;
using OpenSim.Region.Physics.Manager;
//using DSG.RegionSync;

namespace DSG.PEPlugin
{
    [Serializable]
public sealed class PEPrim : PhysicsActor
{
    private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private bool _stopped;
    private Vector3 _size;
    private PrimitiveBaseShape _pbs;
    private uint _localID = 0;
    private bool _grabbed;
    private bool _selected;
    private Vector3 _position;
    private float _mass;
    private Vector3 _force;
    private Vector3 _targetVelocity;
    private Vector3 _velocity;
    private Vector3 _torque;
    private float _collisionScore;
    private Vector3 _acceleration;
    private Quaternion _orientation;
    private int _physicsActorType;
    private bool _isPhysical;
    private bool _flying;
    private bool _setAlwaysRun;
    private bool _throttleUpdates;
    private bool _isColliding;
    private bool _collidingGround;
    private bool _collidingObj;
    private bool _floatOnWater;
    private Vector3 _rotationalVelocity;
    private bool _kinematic;
    private float _buoyancy;

    private Vector3 _PIDTarget;
    private bool _usePID;
    private float _PIDTau;
    private bool _useHoverPID;
    private float _PIDHoverHeight;
    private PIDHoverType _PIDHoverType;
    private float _PIDHoverTao;

    public PEPrim(String primName, PEScene parent_scene, Vector3 pos, Vector3 size,
                       Quaternion rotation, IMesh mesh, PrimitiveBaseShape pbs, bool pisPhysical, CollisionLocker dode)
    {
        _position = pos;
        _size = size;
        _orientation = rotation;
        // m_log.DebugFormat("[REMOTE PRIM ENGINE] PEPrim creation of {0}", primName);
    }
    
    public override bool Stopped { 
        get { return _stopped; } 
    }
    public override Vector3 Size { 
        get { return _size; } 
        set { _size = value;
            // m_log.Debug("[REMOTE PRIM ENGINE] PEPrim set Size");
            // SyncUpdated = true;
        } 
    }
    public override PrimitiveBaseShape Shape { 
        set { _pbs = value; 
            // m_log.Debug("[REMOTE PRIM ENGINE] PEPrim set Shape");
            // SyncUpdated = true;
        } 
    }
    public override uint LocalID { 
        set { _localID = value; 
            // m_log.Debug("[REMOTE PRIM ENGINE] PEPrim set LocalID");
            // SyncUpdated = true;
        }
        get { return _localID; }
    }
    public override bool Grabbed { 
        set { _grabbed = value; 
            // m_log.Debug("[REMOTE PRIM ENGINE] PEPrim set Grabbed");
        } 
    }
    public override bool Selected { 
        set { _selected = value; 
            // m_log.Debug("[REMOTE PRIM ENGINE] PEPrim set Selected");
        } 
    }
    public override void CrossingFailure() { return; }
    public override void link(PhysicsActor obj) { return; }
    public override void delink() { return; }
    public override void LockAngularMotion(Vector3 axis) { return; }

    public override Vector3 Position { 
        get { return _position; } 
        set { _position = value; 
            // SyncUpdated = true;
            // m_log.Debug("[REMOTE PRIM ENGINE] PEPrim set Position");
        } 
    }
    public override float Mass { 
        get { return _mass; } 
    }
    public override Vector3 Force { 
        get { return _force; } 
        set { _force = value; 
            // SyncUpdated = true;
            // m_log.Debug("[REMOTE PRIM ENGINE] PEPrim set Force");
        } 
    }

    public override int VehicleType { 
        get { return 0; } 
        set { return; } 
    }
    public override void VehicleFloatParam(int param, float value) { }
    public override void VehicleVectorParam(int param, Vector3 value) {}
    public override void VehicleRotationParam(int param, Quaternion rotation) { }
    public override void VehicleFlags(int param, bool remove) { }

    // Allows the detection of collisions with inherently non-physical prims. see llVolumeDetect for more
    public override void SetVolumeDetect(int param) { return; }

    public override Vector3 GeometricCenter { get { return Vector3.Zero; } }
    public override Vector3 CenterOfMass { get { return Vector3.Zero; } }
    public override Vector3 TargetVelocity
    {
        get { return _targetVelocity; }
        set { _targetVelocity = value; }
    }
    public override Vector3 Velocity 
    { 
        get { return _velocity; } 
        set { _velocity = value; } 
    }
    public override Vector3 Torque { 
        get { return _torque; } 
        set { _torque = value; } 
    }
    public override float CollisionScore { 
        get { return _collisionScore; } 
        set { _collisionScore = value; 
            // SyncUpdated = true;
        } 
    }
    public override Vector3 Acceleration { 
        get { return _acceleration; } 
        set { _acceleration = value; } 
    }
    public override Quaternion Orientation { 
        get { return _orientation; } 
        set { _orientation = value; 
            // SyncUpdated = true;
        } 
    }
    public override int PhysicsActorType { 
        get { return _physicsActorType; } 
        set { _physicsActorType = value; 
            // SyncUpdated = true;
        } 
    }
    public override bool IsPhysical { 
        get { return _isPhysical; } 
        set { _isPhysical = value; 
            // SyncUpdated = true;
        } 
    }
    public override bool Flying { 
        get { return _flying; } 
        set { _flying = value; 
            // SyncUpdated = true;
        } 
    }
    public override bool 
        SetAlwaysRun { 
        get { return _setAlwaysRun; } 
        set { _setAlwaysRun = value; } 
    }
    public override bool ThrottleUpdates { 
        get { return _throttleUpdates; } 
        set { _throttleUpdates = value; } 
    }
    public override bool IsColliding { 
        get { return _isColliding; } 
        set { _isColliding = value; } 
    }
    public override bool CollidingGround { 
        get { return _collidingGround; } 
        set { _collidingGround = value; } 
    }
    public override bool CollidingObj { 
        get { return _collidingObj; } 
        set { _collidingObj = value; } 
    }
    public override bool FloatOnWater { 
        set { _floatOnWater = value; } 
    }
    public override Vector3 RotationalVelocity { 
        get { return _rotationalVelocity; } 
        set { _rotationalVelocity = value; } 
    }
    public override bool Kinematic { 
        get { return _kinematic; } 
        set { _kinematic = value; } 
    }
    public override float Buoyancy { 
        get { return _buoyancy; } 
        set { _buoyancy = value; } 
    }

    // Used for MoveTo
    public override Vector3 PIDTarget { 
        set { _PIDTarget = value; } 
    }
    public override bool PIDActive { 
        set { _usePID = value; } 
    }
    public override float PIDTau { 
        set { _PIDTau = value; } 
    }

    // Used for llSetHoverHeight and maybe vehicle height
    // Hover Height will override MoveTo target's Z
    public override bool PIDHoverActive { 
        set { _useHoverPID = value; }
    }
    public override float PIDHoverHeight { 
        set { _PIDHoverHeight = value; }
    }
    public override PIDHoverType PIDHoverType { 
        set { _PIDHoverType = value; }
    }
    public override float PIDHoverTau { 
        set { _PIDHoverTao = value; }
    }

    // For RotLookAt
    public override Quaternion APIDTarget { set { return; } }
    public override bool APIDActive { set { return; } }
    public override float APIDStrength { set { return; } }
    public override float APIDDamping { set { return; } }

    public override void AddForce(Vector3 force, bool pushforce) { 
    }
    public override void AddAngularForce(Vector3 force, bool pushforce) { 
    }
    public override void SetMomentum(Vector3 momentum) { 
    }
    public override void SubscribeEvents(int ms) { 
    }
    public override void UnSubscribeEvents() { 
    }
    public override bool SubscribedEvents() { 
        return false; 
    }
}
}
