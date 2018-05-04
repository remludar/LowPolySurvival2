using UnityEngine;
public class FirstPersonCamera : MonoBehaviour
{
    public bool ViewBobbing = true;

    float timerX = 0.0f;
    float bobbingSpeedX = 0.06f;
    float bobbingWalkAmountX = 0.2f;
    float bobbingRunAmountX = 0.4f;
    float bobbingAmountX;
    float midpointX = 0f;

    float timerY = 0.0f;
    float bobbingSpeedY = 0.12f;
    float bobbingWalkAmountY = 0.1f;
    float bobbingRunAmountY = 0.2f;
    float bobbingAmountY;
    float midpointY = 0f;

    float walkRunLerpSpeed = 0.1f;

    void Update()
    {
        if (ViewBobbing)
            UpdateViewBobbing();
    }

    void UpdateViewBobbing()
    {
        if (InputManager.isLeftShift)
        {
            bobbingAmountX = bobbingRunAmountX;
            bobbingAmountY = bobbingRunAmountY;
        }
        else
        {
            bobbingAmountX = bobbingWalkAmountX;
            bobbingAmountY = bobbingWalkAmountY;
        }

        float wavesliceX = 0.0f;
        float wavesliceY = 0.0f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cSharpConversion = transform.localPosition;

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timerX = 0.0f;
            timerY = 0.0f;
        }
        else
        {
            wavesliceX = Mathf.Sin(timerX);
            wavesliceY = Mathf.Sin(timerY);
            timerX = timerX + bobbingSpeedX;
            timerY = timerY + bobbingSpeedY;
            if (timerX > Mathf.PI * 2)
            {
                timerX = timerX - (Mathf.PI * 2);
            }
            if (timerY > Mathf.PI * 2)
            {
                timerY = timerY - (Mathf.PI * 2);
            }
        }
        if (wavesliceX != 0)
        {
            float translateChange = wavesliceX * bobbingAmountX;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, -1.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            cSharpConversion.x = Mathf.Lerp(cSharpConversion.x, midpointX + translateChange, walkRunLerpSpeed);
        }
        else
        {
            cSharpConversion.x = Mathf.Lerp(cSharpConversion.x, midpointX, walkRunLerpSpeed);
        }
        if (wavesliceY != 0)
        {
            float translateChange = wavesliceY * bobbingAmountY;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            cSharpConversion.y = Mathf.Lerp(cSharpConversion.y, midpointY + translateChange, walkRunLerpSpeed);
        }
        else
        {
            cSharpConversion.y = Mathf.Lerp(cSharpConversion.y, midpointY, walkRunLerpSpeed);
        }

        transform.localPosition = cSharpConversion;
    }


}