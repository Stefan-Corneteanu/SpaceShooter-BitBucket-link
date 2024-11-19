using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedNumber : MonoBehaviour
{
    public int number = 0;
    public AnimatedChar[] animatedChars;
    // Start is called before the first frame update
    void Start()
    {

        UpdateNumber(number);
    }

    public void UpdateNumber(int newNumber)
    {
        number = newNumber;
        string digits = number.ToString();
        int d = digits.Length - 1;
        for(int i = 0; i < animatedChars.Length; i++)
        {
            int n = 0;
            if (d >= 0)
            {
                n = digits[d]-'0';
            }
            animatedChars[i].digit = n;
            d--;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
