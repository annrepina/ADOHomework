using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework
{
    public class Builder<T>
        where T : class, new()
    {
        public Builder()
        {
            Reset();
        }

        public virtual T GetResult()
        {
            T result = _element;

            Reset();

            return result;
        }

        protected virtual void Reset()
        {
            _element = new T();
        }

        protected T _element;
    }
}
