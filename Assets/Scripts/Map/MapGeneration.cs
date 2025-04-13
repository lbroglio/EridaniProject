using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{   

    /// <summary>
    /// The minimum length/width for a side of a room 
    /// </summary>
    [SerializeField] private float minRoomSize = 1;

    /// <summary>
    /// The size to stop subdividing space 
    /// </summary>
    [SerializeField] private float roomSize = 1;

    /// <summary>
    /// Represent a space created or used during the BSP algorithm.
    /// Since all rooms are on the same level there they can represented as rectangles.
    /// </summary> 
    private struct Space{
        /// <summary>
        /// The size of this Space in the x direction (of the Unity scene).
        /// </summary>
        public float length;
        /// <summary>
        /// The size of this Space in the z direction (of the Unity scene).
        /// </summary>
        public float width;
        /// <summary>
        /// The coordinates of the left corner of this space.
        /// </summary>
        public Vector3 leftCorner;
    }   

    /// <summary>
    /// A node in a tree used in binary space partitoning 
    /// </summary> 
    private class BSPTreeNode{

        /// <summary>
        /// Create a new BSP tree node for the space represented by the given values.
        /// </summary>
        public BSPTreeNode(Vector3 leftCorner, float length, float width){
            _payload = new Space
            {
                leftCorner = leftCorner,
                length = length,
                width = width
            };
        }

        /// <summary>
        /// The space that this node represents in the BSP tree
        /// </summary> 
        private Space _payload;
        /// <summary>
        /// The left child of this node in the tree. This will be null if this is a leaf node
        /// </summary> 
        private BSPTreeNode _leftChild;
        /// <summary>
        /// The right child of this node in the tree. This will be null if this is a leaf node
        /// </summary> 
        private BSPTreeNode _rightChild;

        /// <summary>
        /// The space that this node represents in the BSP tree
        /// </summary> 
        public Space Payload { 
            get { return _payload;}
            set { _payload = value;}
        }

        /// <summary>
        /// The left child of this node in the tree. This will be null if this is a leaf node
        /// </summary> 
        public BSPTreeNode LeftChild {
            get { return _leftChild;}
            set { _leftChild = value;}
        }

        /// <summary>
        /// The right child of this node in the tree. This will be null if this is a leaf node
        /// </summary> 
        public BSPTreeNode RightChild{
            get { return _rightChild;}
            set { _rightChild = value;}
        }

    }  

    /// <summary>
    /// Divide a space as part of a 
    /// </summary>
    /// <param name="toPartition"></param> 
    private void PartitionSpace(ref BSPTreeNode toPartition){
        float spaceSize = toPartition.Payload.length * toPartition.Payload.width;

        // If this space is below the max room size don't subdivide
        if(spaceSize <= roomSize){
            toPartition.LeftChild = null;
            toPartition.RightChild = null;
            return;
        }

        // Otherwise subdivide 
        
        // Choose axis to split
        // 0 = x, 1 = y
        int splitAxis = Random.Range(0, 2);

        // Split on x
        if(splitAxis == 0){
            float max = toPartition.Payload.length - 1;
            float splitPoint = Random.Range(1.0f, max);

            float leftLength = splitPoint;
            float rightLength = toPartition.Payload.length - splitPoint;

            Vector3 rightChildLeftCorner = toPartition.Payload.leftCorner;
            rightChildLeftCorner.x += splitPoint;

            
            BSPTreeNode leftChild = new BSPTreeNode(toPartition.Payload.leftCorner, leftLength, 
                toPartition.Payload.width);
            BSPTreeNode rightChild = new BSPTreeNode(rightChildLeftCorner, rightLength, 
                toPartition.Payload.width);

            PartitionSpace(ref leftChild);
            PartitionSpace(ref rightChild);

            toPartition.LeftChild = leftChild;
            toPartition.RightChild = rightChild;

        }
        // Split on z
        if(splitAxis == 1){
            float max = toPartition.Payload.width - 1;
            float splitPoint = Random.Range(1.0f, max);

            float leftWidth = splitPoint;
            float rightWidth = toPartition.Payload.width - splitPoint;

            Vector3 rightChildLeftCorner = toPartition.Payload.leftCorner;
            rightChildLeftCorner.z += splitPoint;

            
            BSPTreeNode leftChild = new BSPTreeNode(toPartition.Payload.leftCorner, toPartition.Payload.length, 
                leftWidth);
            BSPTreeNode rightChild = new BSPTreeNode(rightChildLeftCorner, toPartition.Payload.length, 
                rightWidth);

            PartitionSpace(ref leftChild);
            PartitionSpace(ref rightChild);

            toPartition.LeftChild = leftChild;
            toPartition.RightChild = rightChild;

        }
        
    }

    private void BinarySpacePartition(){

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
