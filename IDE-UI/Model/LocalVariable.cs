using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE_UI.Model
{
    class LocalVariable
    {
        /// <summary>
        /// 地址
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// 符号名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
