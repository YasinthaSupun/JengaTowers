using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class JengaTowerBuilder : MonoBehaviour
{
    [SerializeField] private Transform stacksContainer;
    [SerializeField] private GameObject glassBlock;
    [SerializeField] private GameObject woodBlock;
    [SerializeField] private GameObject stoneBlock;

    Dictionary<string, List<StackBlock>> gradeStacks = new();
    Dictionary<string, Transform> stackTransforms = new ();

    void Start()
    {
        FetchStackData();
    }

    private void FetchStackData()
    {
        StackAPIManager stackAPIManager = new();
        stackAPIManager.FetchStackData().Then(stackBlocks =>
        {
            ProcessStackData(stackBlocks);
        }).Catch(error =>
        {
            Debug.Log("Failed to fetch stack data: " + error.Message);
        });
    }

    private void ProcessStackData(StackBlock[] stackBlocks)
    {
        Array.Sort(stackBlocks, (a, b) =>                                                              // Sort the stack blocks according to the specified order
        {
            int result = a.grade.CompareTo(b.grade);
            if (result == 0)
                result = a.domain.CompareTo(b.domain);
            if (result == 0)
                result = a.cluster.CompareTo(b.cluster);
            if (result == 0)
                result = a.standardid.CompareTo(b.standardid);
            return result;
        });

        for (int i = 0; i < stackBlocks.Length; i++)
        {
            StackBlock block = stackBlocks[i];
            if (!IsGradeFormatValid(block.grade))
            {
                continue;
            }

            if (gradeStacks.ContainsKey(block.grade))                                                   // Check if a new stack needs to be created
            {
                gradeStacks[block.grade].Add(block);
            }
            else
            {
                CreateStack(block.grade);
                gradeStacks[block.grade] = new List<StackBlock> { block };
            }

            InstantiateBlock(block, stackTransforms[block.grade]);                                       // Instantiate the block and add it to the current stack
        }

        UIManager.Instance.ConfigureGradeUI(stackTransforms);
    }
   
    private GameObject CreateStack(string grade)
    {
        GameObject stack = new GameObject("Stack " + grade);                                               // Create a stack game object and position it on the table
        int numberOfStacks = stackTransforms.Count;
        stack.transform.position = new Vector3(Utils.STACKOFFSET, 0, 0) * numberOfStacks;
        UIManager.Instance.CreateGradeDisplay(grade, Utils.STACKOFFSET * numberOfStacks);

        stackTransforms[grade] = stack.transform;
        stack.transform.SetParent(stacksContainer);

        return stack;
    }

    private void InstantiateBlock(StackBlock blockData, Transform parent)
    {
        BlockType blockType = GetBlockType(blockData.mastery);                                              // Determine the block type based on mastery level

        GameObject block = null;
        switch (blockType)
        {
            case BlockType.Glass:
                block = Instantiate(glassBlock, Vector3.zero, Quaternion.identity);
                break;
            case BlockType.Wood:
                block = Instantiate(woodBlock, Vector3.zero, Quaternion.identity);
                break;
            case BlockType.Stone:
                block = Instantiate(stoneBlock, Vector3.zero, Quaternion.identity);
                break;
        }

        block.GetComponent<Block>().SetBlockType(blockType, blockData);

        int layerIndex = parent.childCount / 3;                                                             // Index of the current layer
        int blockIndexInLayer = parent.childCount % 3;                                                      // Index of the block within the current layer

        float offsetX;
        float offsetY;
        float offsetZ;

        if (layerIndex % 2 == 0)
        {
            offsetX = GetBlockOffsetInLayer(blockIndexInLayer);
            offsetY = layerIndex * Utils.LAYEROFFSET;        
            offsetZ = 0;                         
        }
        else
        {
            offsetX = 0; 
            offsetY = layerIndex * Utils.LAYEROFFSET;           
            offsetZ = GetBlockOffsetInLayer(blockIndexInLayer);                          

            Vector3 centerInLayer = new Vector3(Utils.BLOCKOFFSET, offsetY, 0);
            if (blockIndexInLayer == 1)
            {
                block.transform.Rotate(Vector3.up, 90f);
            }
            else
            {
                block.transform.RotateAround(centerInLayer, Vector3.up, 90f);
            }
        }

        block.transform.SetParent(parent);
        block.transform.localPosition = new Vector3(offsetX, offsetY, offsetZ);
    }

    private float GetBlockOffsetInLayer(int blockIndexInLayer)
    {
        switch (blockIndexInLayer)
        {
            case 0:
                return -Utils.BLOCKOFFSET;
            case 2:
                return Utils.BLOCKOFFSET;
            default:
                return 0;
                
        }
    }

    private BlockType GetBlockType(int mastery)
    {
        switch (mastery)
        {
            case 0:
                return BlockType.Glass;
            case 1:
                return BlockType.Wood;
            case 2:
                return BlockType.Stone;
            default:
                return BlockType.Unknown;
        }
    }

    public static bool IsGradeFormatValid(string input)
    {
        string pattern = @"^[1-9](th|st|nd) Grade$";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(input);
        return match.Success;
    }
}
