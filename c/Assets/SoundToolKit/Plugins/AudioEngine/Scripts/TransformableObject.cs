/// \author Michal Majewski
/// \date 21.01.2019

using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// TransformableObject enables updating transform of SoundToolKit components with transform of Unity
    /// parent GameObject via TransformSynchronizer component usage.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public abstract class TransformableObject : MonoBehaviour
    {
        #region protected methods

        protected void SubscribeOnTransformChanged(ITransformable transformable)
        {
            var transformSynchronizer = gameObject.GetComponent<TransformSynchronizer>() ?? 
                gameObject.AddComponent<TransformSynchronizer>();

            transformSynchronizer.Transformables.Add(transformable);
        }

        protected void UnsubscribeFromTransformChanged(ITransformable transformable)
        {
            var transformSynchronizer = gameObject.GetComponent<TransformSynchronizer>();

            if (transformSynchronizer != null)
            {
                transformSynchronizer.Transformables.Remove(transformable);
            }
        }

        #endregion
    }
}
