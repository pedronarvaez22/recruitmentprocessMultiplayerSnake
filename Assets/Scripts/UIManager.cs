using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text instructionsText;
    public Text InstructionsText => instructionsText;

    public GameObject gameOverPanel;
}
