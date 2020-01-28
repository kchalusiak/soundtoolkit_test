/// \author Michal Majewski
/// \date 17.04.2019
///
using SoundToolKit.Unity.Utils;
using System.IO;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Controls writing to and reading from binary geometry data file.
    /// </summary>
    /// <note>Associated data file is stored in StreamingAssets/SoundToolKit/SerializedGeometry</note>
    [System.Serializable]
    public class SerializedGeometry : ScriptableObject
    {
        #region editor fields

        [SerializeField, HideInInspector]
        private string m_filename = null;

        #endregion

        #region internal properties

        /// <summary>
        /// Name of the associated data file.
        /// </summary>
        internal string Filename
        {
            get { return m_filename; }
            private set
            {
                m_filename = value;
            }
        }

        /// <summary>
        /// Whether a SerializedGeometry has a corresponding valid data file.
        /// </summary>
        internal bool Valid
        {
            get
            {
                if (Filename == null || Filename.Length == 0)
                {
                    return false;
                }

                return File.Exists(DataFilePath());
            }
        }

        #endregion

        #region internal methods

        internal void WriteData(string filename, byte[] geometryData)
        {
            Filename = Path.ChangeExtension(filename, "dat");
            var targetPath = DataFilePath();
            File.WriteAllBytes(targetPath, geometryData);

            SoundToolKitDebug.Log("Successful serialization of a MeshCluster. Geometry data saved to file: " + Filename);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        internal byte[] ReadData()
        {
            var data = new byte[0];

            if (Valid)
            {
                data = File.ReadAllBytes(DataFilePath());
            }
            else
            {
                SoundToolKitDebug.LogError("Tried to read a non-existant SerializedGeometry file. " +
                    "Make sure to serialize geometry before attempting to deserialize it.");
            }

            return data;
        }

        internal void EraseFromDisk()
        {
            if (Valid)
            {
                File.Delete(DataFilePath());
                File.Delete(DataFilePath() + ".meta");
            }

            DestroyImmediate(this, true);
        }

        #endregion

        #region private methods

        private string StreamingAssetsDataDirectory()
        {
            var serializedDataDirectory = Path.Combine(Application.streamingAssetsPath, SERIALIZED_GEOMETRY_DATA_PATH);
            Directory.CreateDirectory(serializedDataDirectory);
            return serializedDataDirectory;
        }

        private string DataFilePath()
        {
            return Path.Combine(StreamingAssetsDataDirectory(), Filename);
        }

        #endregion

        #region private members

        private const string SERIALIZED_GEOMETRY_DATA_PATH = @"SoundToolKit\SerializedGeometry";

        #endregion
    }
}
