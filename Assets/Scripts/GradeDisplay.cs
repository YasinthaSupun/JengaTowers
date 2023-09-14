using UnityEngine;
using TMPro;

public class GradeDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text text1;
    [SerializeField] private TMP_Text text2;

    public void SetGrade(string grade)
    {
        text1.text = grade;
        text2.text = grade;
    }
}
