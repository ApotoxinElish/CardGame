using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhaseDisplayer : MonoBehaviour
{
    public TMP_Text phaseText;
    // Start is called before the first frame update
    void Start()
    {
        BattleManager.Instance.phaseChangeEvent.AddListener(UpdateText);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateText()
    {
        phaseText.text = BattleManager.Instance.GamePhase.ToString();
    }
}
