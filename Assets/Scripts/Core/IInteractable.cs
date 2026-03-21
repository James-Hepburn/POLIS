using UnityEngine;

public interface IInteractable
{
    Vector2 WorldPosition { get; }
    bool    IsEligible    { get; }
    void    ShowPrompt    (bool show);
}