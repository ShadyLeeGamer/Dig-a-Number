using UnityEngine;

[CreateAssetMenu(fileName = "SoundsAsset", menuName = "ScriptableObjects/SoundsAsset", order = 1)]
public class SoundsAsset : ScriptableObject
{
    public AudioClip optionsTrack;

    public AudioClip[] numberCountSFX;
    public AudioClip digSFX;
    public AudioClip selectionSFX;
    public AudioClip correctOptionSFX;
}