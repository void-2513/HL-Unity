using UnityEngine;

public class CombineAnimEvent : MonoBehaviour
{
    public Combine combine;

    public void Pump()
    {
        combine.PlayPumpSound();
    }
}
