/// \author Michal Majewski
/// \date 21.01.2019

using SoundToolKit.Unity.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Simulataneously updates the transform of multiple TransformableObjects with Unity transform
    /// </summary>
    public class TransformSynchronizer : MonoBehaviour
    {
        #region internal properties

        internal List<ITransformable> Transformables { get; set; }

        #endregion

        #region private methods

        #region unity methods

        private void Awake()
        {
            Transformables = new List<ITransformable>();
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                Transformables.ForEach(x => UpdateTransform(x));
                transform.hasChanged = false;
            }
        }

        #endregion

        private void UpdateTransform(ITransformable transformable)
        {
            transformable.Position = Vector3Extensions.ToNumerics(transform.position);
            transformable.Rotation = QuaternionExtensions.ToNumerics(transform.rotation);
        }

        #endregion
    }
}
