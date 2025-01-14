using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using Game.ECS.Base;
using Game.Factory;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using System.Data;

public class QuadtreeCreationSystem : IInitSystem
{
    private ECSWorld _world;
    private FactoryManager _factoryManager;
    public void Init(SystemManager systemManager)
    {
        _world = systemManager.GetWorld();
     
        _world.QuadTreeData.TileQuadtreeRoot = CreateQuadTree(_world.GetComponentContainer<QuadTreeLeafComponent>().Components, MapSettings.MapWidth, MapSettings.MapHeight, MapSettings.TileEdgeSize);

    }

    public QuadtreeCreationSystem(FactoryManager factoryManager)
    {
        _factoryManager = factoryManager;
    }



    private QuadTreeNodeData CreateQuadTree(NativeArray<QuadTreeLeafComponent> quadTreeLeafComponents, int mapWidth, int mapHeight, float tileWidth)
    {
        AddQuadtreeNode(new Rect(-tileWidth / 2, -tileWidth / 2, mapWidth * tileWidth, mapHeight * tileWidth));


        for (int i = 0; i < quadTreeLeafComponents.Length; i++)
        {

            InsertLeaf(0, quadTreeLeafComponents, quadTreeLeafComponents[i].LeafID);
        }
        return _world.QuadTreeData.QuadTreeNodeDatas[0];
    }


    private void InsertLeaf(int rootIndex, NativeArray<QuadTreeLeafComponent> quadTreeLeafComponents, int leafId)
    {

        QuadTreeNodeData rootNode = _world.QuadTreeData.QuadTreeNodeDatas[rootIndex];

        if (rootNode.IsDivided)
        {

            for (int i = 0; i < 4; i++)
            {
                if (IsLeafInNode(_world.QuadTreeData.QuadTreeNodeDatas[_world.QuadTreeData.QuadtreeNodeIndexes[rootNode.NodesStart + i]], quadTreeLeafComponents[leafId]))
                {
                    InsertLeaf(_world.QuadTreeData.QuadtreeNodeIndexes[rootNode.NodesStart + i], quadTreeLeafComponents, leafId);
                    _world.QuadTreeData.QuadTreeNodeDatas[rootIndex] = rootNode;
                    return;
                }
            }
        }
        else
        {

            if (rootNode.LeafCount >= rootNode.Capacity)
            {
                Subdivide(ref rootNode);
                for (int k = 0; k < rootNode.LeafCount; k++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int leafIndexValue = _world.QuadTreeData.QuadtreeLeafIndexes[rootNode.LeavesStart + k];

                        if (IsLeafInNode(_world.QuadTreeData.QuadTreeNodeDatas[_world.QuadTreeData.QuadtreeNodeIndexes[rootNode.NodesStart + i]], quadTreeLeafComponents[leafIndexValue]))
                        {
                            _world.QuadTreeData.QuadTreeNodeDatas[rootIndex] = rootNode;
                            InsertLeaf(_world.QuadTreeData.QuadtreeNodeIndexes[rootNode.NodesStart + i], quadTreeLeafComponents, leafIndexValue);
                        }
                    }
                }
                for (int i = 0; i < 4; i++)
                {

                    if (IsLeafInNode(_world.QuadTreeData.QuadTreeNodeDatas[_world.QuadTreeData.QuadtreeNodeIndexes[rootNode.NodesStart + i]], quadTreeLeafComponents[leafId]))
                    {
                        _world.QuadTreeData.QuadTreeNodeDatas[rootIndex] = rootNode;
                        InsertLeaf(_world.QuadTreeData.QuadtreeNodeIndexes[rootNode.NodesStart + i], quadTreeLeafComponents, leafId);
                    }
                }

                for (int i = 0; i < rootNode.Capacity; i++)
                {
                    _world.QuadTreeData.QuadtreeLeafIndexes[rootNode.LeavesStart + i] = -1;
                }
            }
            else
            {
                _world.QuadTreeData.QuadtreeLeafIndexes[rootNode.LeavesStart + rootNode.LeafCount] = leafId;

                rootNode.LeafCount++;

            }
        }
        _world.QuadTreeData.QuadTreeNodeDatas[rootIndex] = rootNode;
    }

    private bool IsLeafInNode(QuadTreeNodeData node, QuadTreeLeafComponent leaf)
    {

        return node.Rect.Contains(leaf.Rect.center);
    }


    private void Subdivide(ref QuadTreeNodeData node)
    {
        float halfWidth = node.Rect.width / 2f;
        float halfHeight = node.Rect.height / 2f;

        AddQuadtreeNode(new Rect(node.Rect.x, node.Rect.y, halfWidth, halfHeight));
        AddQuadtreeNode(new Rect(node.Rect.x + halfWidth, node.Rect.y, halfWidth, halfHeight));
        AddQuadtreeNode(new Rect(node.Rect.x, node.Rect.y + halfHeight, halfWidth, halfHeight));
        AddQuadtreeNode(new Rect(node.Rect.x + halfWidth, node.Rect.y + halfHeight, halfWidth, halfHeight));

        for (int i = 0; i < 4; i++)
        {
            _world.QuadTreeData.QuadtreeNodeIndexes[node.NodesStart + i] = _world.QuadTreeData.QuadTreeNodeDatas.Length - 4 + i;

        }


        node.IsDivided = true;
    }

    //public int GetDepth(QuadTreeNodeData rootNode, int currentDepth = 0)
    //{
    //    if (rootNode.IsDivided)
    //    {
    //        currentDepth++;
    //        return GetDepth(_world.quadTreeNodeDatas[rootNode.NodeIDs[0]], currentDepth);
    //    }
    //    else
    //    {
    //        return currentDepth;
    //    }
    //}

    public void AddQuadtreeNode(Rect rect)
    {
        QuadTreeNodeData nodeData = new QuadTreeNodeData()
        {
            LeafCount = 0,
            Rect = rect,
            Capacity = 4,
            IsDivided = false,
            NodesStart = _world.QuadTreeData.QuadtreeNodeIndexes.Length,
            LeavesStart = _world.QuadTreeData.QuadtreeLeafIndexes.Length
        };

        _world.QuadTreeData.QuadTreeNodeDatas.Add(nodeData);
        _world.QuadTreeData.QuadtreeNodeIndex++;
        for (int i = 0; i < nodeData.Capacity; i++)
        {
            _world.QuadTreeData.QuadtreeNodeIndexes.Add(-1);
            _world.QuadTreeData.QuadtreeLeafIndexes.Add(-1);
        }

    }


}