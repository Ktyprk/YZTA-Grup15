using UnityEngine;

namespace HappyLama
{
    /// <summary>
    /// Interface for any movement controller that needs to be controlled by the dialogue system.
    /// Implement this interface in your player controller to automatically integrate with the dialogue system.
    /// </summary>
    public interface IMovementController
    {
        /// <summary>
        /// Gets or sets whether the player can move
        /// </summary>
        bool CanMove { get; set; }

        /// <summary>
        /// Gets the GameObject this controller is attached to
        /// </summary>
        GameObject GameObject { get; }
    }
}