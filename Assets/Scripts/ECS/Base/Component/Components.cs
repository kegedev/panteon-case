﻿using UnityEngine;
using Unity.Mathematics;
using System.ComponentModel;
using Unity.Collections;


namespace Game.ECS.Base.Components
{
    public struct CoordinateComponent//TODO: create custom Coordinate type to encapsulate position data and implement operator overloads
    {
        public int2 Coordinate;
    }

    public struct TileComponent
    {
        public int OccupantEntityID;
    }
    public struct RenderComponent
    {
        public Matrix4x4 TRS;
        public float2 TextureOffset;
    }

    public struct QuadTreeLeafComponent
    {
        public int LeafID;
        public Rect Rect;
    }

    public struct MoverComponent
    {
        public bool HasPath;
        public int PathStepNumber;
        public NativeArray<int2> Path;
    }

    public struct AreaComponent
    {
        public int Width;
        public int Height;
    }

    public struct BuildingComponent
    {
       public ushort BuildingType;
    }

    public struct SoldierComponent
    {
        public ushort SoldierType;
    }

    public struct HealthComponent
    {
        public int Health;
    }

    public struct AttackComponent
    {
        public int Damage;
        public int TargetId;
    }
}

