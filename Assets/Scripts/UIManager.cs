using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject gradeDisplay;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Transform gradeUIPanel;
    [SerializeField] private GameObject gradeButtonPrefab;
    [SerializeField] private TMP_Text domainText;
    [SerializeField] private TMP_Text clusterText;
    [SerializeField] private TMP_Text standardIDText;

    public static event Action OnTestMyStackEvent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Block.OnBlockSelectedEvent += UpdateInfoUI;
    }

    private void OnDisable()
    {
        Block.OnBlockSelectedEvent -= UpdateInfoUI;
    }

    public void CreateGradeDisplay(string grade, float posX)
    {
        GameObject gradeUI = Instantiate(gradeDisplay, canvas.transform);
        gradeUI.transform.localPosition = new Vector3(posX, 0, 0);
        gradeUI.GetComponent<GradeDisplay>().SetGrade(grade);
    }

    public void ConfigureGradeUI(Dictionary<string, Transform> stackTransforms)
    {
        foreach (KeyValuePair<string, Transform> pair in stackTransforms)
        {
            GameObject gradeButton = Instantiate(gradeButtonPrefab, gradeUIPanel);
            gradeButton.GetComponent<GradeButton>().SetData(pair.Key, pair.Value);
        }
    }

    public void TestMyStack()
    {
        OnTestMyStackEvent.Invoke();
    }

    private void UpdateInfoUI(StackBlock blockInfo)
    {
        domainText.text = "Domain : " + blockInfo.domain;
        clusterText.text = "Cluster : " + blockInfo.cluster;
        standardIDText.text = "Standard ID : " + blockInfo.standardid;
    }
}
