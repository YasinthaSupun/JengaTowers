using System;
using UnityEngine;
using TMPro;

public class GradeButton : MonoBehaviour
{
    private string Grade;
    private Transform StackTransform;
    public static event Action<string, Transform> OnButtonClickedEvent;
    [SerializeField] private TMP_Text text;

    public void SetData(string grade, Transform transform)
    {
        Grade = grade;
        StackTransform = transform;
        text.text = Grade;
    }

    public void OnButtonClick()
    {
        OnButtonClickedEvent.Invoke(Grade, StackTransform);
    }
}
