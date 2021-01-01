using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.CMMException
{
    //错误表示在用户输入的文本的接口
    public interface IErrorShow
    {
        //line:错误所在行，charStartPosition:错误字符的位置
        void ShowErrorPositionUI(int line,int charStartPosition);
    }
}
