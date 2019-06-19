using System;
using System.Collections.Generic;
using System.Text;

namespace Service.DBOperator
{
    /// </summary>
    /// <typeparam name="TKey">关键值--索引值</typeparam>
    /// <typeparam name="TValue">显示值</typeparam>
    public class ItemInfo<TKey, TValue>
    {
        private TKey innerValue;
        private TValue displayValue;

        private ItemInfo() { }

        /// <summary>
        /// 对象值
        /// </summary>
        public TKey InnerValue
        {
            get { return innerValue; }
            private set { this.innerValue = value; }
        }

        /// <summary>
        /// 显示值
        /// </summary>
        public TValue DisplayValue
        {
            get { return displayValue; }
            private set { this.displayValue = value; }
        }

        /// <summary>
        /// 初始化值项
        /// </summary>
        /// <param name="innervalue">关键值</param>
        /// <param name="displayvalue">显示值</param>
        public ItemInfo(TKey innervalue, TValue displayvalue)
        {
            this.InnerValue = innervalue;
            this.DisplayValue = displayvalue;
        }

        /// <summary>
        /// 将对象显示为字符串
        /// </summary>
        /// <returns>显示的字符串</returns>
        public override string ToString()
        {
            return DisplayValue.ToString();
        }

        /// <summary>
        /// 是否相等
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
        /// 是否不等
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
        /// 是否相等
        /// </summary>
        /// <param name="item">比较对象</param>
        /// <returns></returns>
        public bool Equals(ItemInfo<TKey, TValue> item)
        {
            return this == item;
        }
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns></returns>
        [Obsolete("不要使用此方法")]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <summary>
        /// 获取HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
