using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class S_Timer : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _textTimer;
    
    private float _hours, _minutes, _seconds, _milliseconds;
    private float _timerTime, _startTime, _stopTime;

    private bool _timerPlay = false;

    private void Start()
    {
        _timerTime = Time.time;
    }

    private void Update()
    {
        //calcule temps en Numerique
        _timerTime = _stopTime + (Time.time - _startTime);

        _hours = (int)(_timerTime / 3600f);
        _minutes = (int)(_timerTime / 60f) % 60;
        _seconds = (int)(_timerTime % 60f);
        _milliseconds = (int)(_timerTime * 1000f) % 1000;



        if (_timerPlay) // affichage 
        {
            if (_minutes < 10)// ajoute un 0 devant les 10 premiere min
            {
                if (_seconds < 10)// ajoute un 0 devant les 10 premiere sec
                {

                    if (_milliseconds < 100)

                        _textTimer.text = "0" + _minutes + ":" + "0" + _seconds + ":" + "0" + _milliseconds;

                     else
                        _textTimer.text = "0" + _minutes + ":" + "0" + _seconds + ":" + _milliseconds;
                }
                else
                {
                     if (_milliseconds < 100)
                            
                        _textTimer.text = "0" + _minutes + ":" + _seconds + ":" + "0" + _milliseconds;

                     else
                        _textTimer.text = "0" + _minutes + ":" + _seconds + ":" + _milliseconds;
                }
            }
            else
            {
                if (_seconds < 10)
                {
                     if (_milliseconds < 100)
                         _textTimer.text = _minutes + ":" + "0" + _seconds + ":" + "0" + _milliseconds;
                     else
                         _textTimer.text = _minutes + ":" + "0" + _seconds + ":" + _milliseconds;
                }
                else
                {
                    if (_milliseconds < 100)
                        _textTimer.text = _minutes + ":" + _seconds + ":" + "0" + _milliseconds;
                    else
                        _textTimer.text = _minutes + ":" + _seconds + ":" + _milliseconds;

                }
                   
            }
        }  
    }


    public void TimerStart()
    {   
        if (!_timerPlay)
        {
            Debug.Log("StartTimer");
            _timerPlay = true ;
            _startTime = Time.time;
        
        }
    }

    public void TimerStop()
    {
        if (_timerPlay)
        {
            Debug.Log("StopTimer : " +_timerTime );
            _timerPlay = false;
            _stopTime = _timerTime;

        }
    }
    public void TimerReset()
    {
        Debug.Log("ResetTimer");
        TimerStop();
        _textTimer.text = "00:00:000";
        _startTime = Time.time;
        _stopTime = 0f;

    }
}

