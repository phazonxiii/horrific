using UnityEngine;

namespace HorrorEngine
{
    public interface IUIInput
    {
        Vector2 GetPrimaryAxis();
        bool IsConfirmDown();
        bool IsCancelDown();
        bool IsDismissDown();
        bool IsTogglePauseDown();
        bool IsToggleInventoryDown();

        bool IsToggleMapDown();
        bool IsToggleMapListDown();
        bool IsPrevSubmapDown();
        bool IsNextSubmapDown();

        void Flush();
    }
}