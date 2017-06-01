using UnityEngine;
using System.Collections;

public class RotationalPosition {

    float angleX = 0f, angleY = 0f;

    public float horizontalAngle { get { return angleX; } }
    public float verticalAngle { get { return angleY; } }

    public RotationalPosition()
    {
        
    }

    public RotationalPosition( Vector3 _aLocalVector)
    {
        CalcAngles(_aLocalVector);
    }

    public void CalcAngles(Vector3 _aLocalVector)//takes a local vector and gives you the closest rotation on the y (horizontal) and z (vecticle) axis.
    {
        angleX = FindAngle(new Vector2(_aLocalVector.x, _aLocalVector.z));
        angleY = FindAngle(new Vector2(_aLocalVector.y, _aLocalVector.z));
    }

    float FindAngle(Vector2 inVector)//the function to find the angles
    {
        if (inVector.x == 0 && inVector.y < 0)
        {
            return 180;
        }
        else if (inVector.x == 0 && inVector.y == 0)
        {
            return 0;
        }       

        float rFloat = Mathf.Rad2Deg * Mathf.Atan(inVector.x / inVector.y);

        if (inVector.y < 0)
        {
            if (inVector.x <= 0)
            {
                rFloat = -180 + rFloat;
            }
            else
            {
                rFloat = 180 + rFloat;
            }            
        }

        return rFloat;
    }
}
