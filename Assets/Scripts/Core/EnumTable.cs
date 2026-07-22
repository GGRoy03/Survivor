using System;
using System.Linq;
using System.Collections;

using UnityEngine;

namespace Survivor.Core
{
    [Serializable]
    public class EnumTable<TEnum, TType> where TEnum : Enum
    {
        [SerializeField] private TType[] m_DataArray;

        private static readonly TEnum[] EnumValues   = (TEnum[])Enum.GetValues(typeof(TEnum));
        private static readonly int     EnumMinValue = EnumValues.Length > 0 ? Convert.ToInt32(EnumValues.Min()) : 0;

        private void EnsureSized()
        {
            if(m_DataArray == null)
            {
                m_DataArray = new TType[EnumValues.Length];
            }
            else if(m_DataArray.Length != EnumValues.Length)
            {
                m_DataArray = new TType[EnumValues.Length];
            }
        }

        public TType GetFromKey(TEnum key)
        {
            EnsureSized();

            TType result = default;

            if(m_DataArray != null)
            {
                int keyAsInt = Convert.ToInt32(key) - EnumMinValue;
                if(keyAsInt < m_DataArray.Length)
                {
                    result = m_DataArray[keyAsInt];
                }
            }

            return result;
        }

        public IEnumerator GetEnumerator()
        {
            EnsureSized();

            //
            // TODO:
            // x) This looks odd.
            //

            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(i => GetFromKey(i)).GetEnumerator();
        }
    }
}

