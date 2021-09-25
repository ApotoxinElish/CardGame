using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCounter : MonoBehaviour
{
    public Text counterText;
    private int counter = 0;

    public void SetCounter(int _value)
    {
        counter += _value;
        OnCounterChange();
    }

    private void OnCounterChange()
    {
        counterText.text = counter.ToString();
    }
}
