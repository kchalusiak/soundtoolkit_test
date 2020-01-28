/// \author Marcin Misiek
/// \date 13.08.2018
/// 
/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 11.02.2019

using System.Linq;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// This class is a wrapper for internal Stk Indicator.
    /// </summary>
    public class SerializedStatusIndicator
    {
        #region public properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IndicatorType Type { get; private set; }

        public IndicatorUnits Units { get; private set; }

        public string ShortName { get; private set; }

        public string Value
        {
            get { return m_value; }
            private set
            {
                Utils.SoundToolKitDebug.Assert(value != null, "Value of Indicator cannot be null.");
                if (value != Value)
                {
                    m_value = value;
                }
            }
        }

        #endregion

        #region internal constructor

        internal Indicator Native;

        internal SerializedStatusIndicator(Indicator indicator, string indicatorValue)
        {
            Native = indicator;

            Name = indicator.Name;
            Description = indicator.Description;
            Type = indicator.Type;
            Units = indicator.Units;
            ShortName = indicator.Name.Split('.').Last();

            m_value = indicatorValue;
            indicator.OnValueChanged += (newValue) => { Value = newValue; };
        }

        #endregion

        #region private members

        private string m_value;

        #endregion
    }
}
