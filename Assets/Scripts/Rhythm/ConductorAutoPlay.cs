using UnityEngine;
public class ConductorAutoPlay : MonoBehaviour
{
    public BeatConductor conductor;
    void Start() { if (conductor) conductor.Play(); }
}
