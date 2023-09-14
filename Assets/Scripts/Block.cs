using UnityEngine;
using System;

public class Block : MonoBehaviour
{
    private BlockType blockType;
    private StackBlock block;
    private Camera mainCamera;

    public static event Action<StackBlock> OnBlockSelectedEvent;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void SetBlockType(BlockType type, StackBlock blockData)
    {
        blockType = type;
        block = blockData;
    }

    private void OnEnable()
    {
        UIManager.OnTestMyStackEvent += ExecuteTestMyStack;
    }

    private void OnDisable()
    {
        UIManager.OnTestMyStackEvent -= ExecuteTestMyStack;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;          
                MeshCollider meshCollider = clickedObject.GetComponent<MeshCollider>();
                if (meshCollider != null && clickedObject == this.gameObject)
                {
                    OnBlockSelectedEvent.Invoke(block);
                }
            }
        }
    }

    private void ExecuteTestMyStack()
    {
        if(blockType == BlockType.Glass)
        {
            Destroy(gameObject);
        }
    }
}
