using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenshotFromCamera : MonoBehaviour
{
    private int _screenshotNumber;
    private void Start()
    {
        _screenshotNumber = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("Screenshot_" + _screenshotNumber + ".png");
        _screenshotNumber++;
    }
}
