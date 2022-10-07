using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework.Builders
{
    /// <summary>
    /// Абстрактный обобщенный класс - Строитель
    /// </summary>
    /// <typeparam name="T">Объект, который строит строитель</typeparam>
    public class Builder<T>
        where T : class, new()
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Builder()
        {
            Reset();
        }

        /// <summary>
        /// Получить результат строительства - объект T
        /// </summary>
        /// <returns></returns>
        public virtual T GetResult()
        {
            T result = _element;

            Reset();

            return result;
        }

        /// <summary>
        /// Сбросить объект строительства до нового
        /// </summary>
        protected virtual void Reset()
        {
            _element = new T();
        }

        /// <summary>
        /// Объект строительства
        /// </summary>
        protected T _element;
    }
}
