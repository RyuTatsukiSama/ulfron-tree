using UnityEngine;

[System.Serializable]
public class SerializeVector3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    SerializeVector3(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    // Implicit conversion of UnityEngine.Vector3 to SerialiseVector3
    public static implicit operator SerializeVector3(Vector3 unityVector)
    {
        return new SerializeVector3(unityVector.x, unityVector.y, unityVector.z);
    }

    // Implicit conversion of SerialiseVector3 to UnityEngine.Vector3
    public static implicit operator Vector3(SerializeVector3 myVector)
    {
        return new Vector3(myVector.x, myVector.y, myVector.z);
    }
}
