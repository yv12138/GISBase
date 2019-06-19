using System;
using System.Collections.Generic;
using System.Text;

namespace Service.DBOperator
{
    /// </summary>
    /// <typeparam name="TKey">�ؼ�ֵ--����ֵ</typeparam>
    /// <typeparam name="TValue">��ʾֵ</typeparam>
    public class ItemInfo<TKey, TValue>
    {
        private TKey innerValue;
        private TValue displayValue;

        private ItemInfo() { }

        /// <summary>
        /// ����ֵ
        /// </summary>
        public TKey InnerValue
        {
            get { return innerValue; }
            private set { this.innerValue = value; }
        }

        /// <summary>
        /// ��ʾֵ
        /// </summary>
        public TValue DisplayValue
        {
            get { return displayValue; }
            private set { this.displayValue = value; }
        }

        /// <summary>
        /// ��ʼ��ֵ��
        /// </summary>
        /// <param name="innervalue">�ؼ�ֵ</param>
        /// <param name="displayvalue">��ʾֵ</param>
        public ItemInfo(TKey innervalue, TValue displayvalue)
        {
            this.InnerValue = innervalue;
            this.DisplayValue = displayvalue;
        }

        /// <summary>
        /// ��������ʾΪ�ַ���
        /// </summary>
        /// <returns>��ʾ���ַ���</returns>
        public override string ToString()
        {
            return DisplayValue.ToString();
        }

        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="param0"></param>
        /// <param name="param1"></param>
        /// <returns></returns>
        public static bool operator ==(ItemInfo<TKey, TValue> param0, ItemInfo<TKey, TValue> param1)
        {
            object obj0 = param0;
            object obj1 = param1;
            if (obj0 == null && obj1 == null) return true;
            if (obj0 == null) return false;
            if (obj1 == null) return false;
            return param0.InnerValue.Equals(param1.InnerValue);
        }

        /// <summary>
        /// �Ƿ񲻵�
        /// </summary>
        /// <param name="param0"></param>
        /// <param name="param1"></param>
        /// <returns></returns>
        public static bool operator !=(ItemInfo<TKey, TValue> param0, ItemInfo<TKey, TValue> param1)
        {
            object obj0 = param0;
            object obj1 = param1;
            if (obj0 == null && obj1 == null) return false;
            if (obj0 == null) return true;
            if (obj1 == null) return true;
            return !param0.InnerValue.Equals(param1.InnerValue);
        }

        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="item">�Ƚ϶���</param>
        /// <returns></returns>
        public bool Equals(ItemInfo<TKey, TValue> item)
        {
            return this == item;
        }
        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="obj">�Ƚ϶���</param>
        /// <returns></returns>
        [Obsolete("��Ҫʹ�ô˷���")]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <summary>
        /// ��ȡHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
