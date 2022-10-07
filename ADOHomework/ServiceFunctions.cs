using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework
{
    /// <summary>
    /// Сервисные функции
    /// </summary>
    public static class ServiceFunctions
    {
        /// <summary>
        /// Меняет местами значения объектов
        /// </summary>
        /// <typeparam name="T">Тип класса</typeparam>
        /// <param name="obj1">Первый объект класса</param>
        /// <param name="obj2">Второй объект класса</param>
        public static void Swap<T>(ref T obj1, ref T obj2)
        {
            // нужно добавить проверку типа
            T temp = obj1;
            obj1 = obj2;
            obj2 = temp;
        }

        /// <summary>
        /// Меняет местами значения объектов в списке
        /// </summary>
        /// <typeparam name="T">Тип класса</typeparam>
        /// <param name="list">Список</param>
        /// <param name="index1">Индекс первого элемента списка</param>
        /// <param name="index2">индекс второго элемента списка</param>
        public static void Swap<T>(List<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }
    }
}
