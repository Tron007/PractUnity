using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pr1 : MonoBehaviour
{
    // Время, прошедшее с последней генерации числа
    private float elapsedTime = 0f;

    // Временной интервал между генерациями чисел (в секундах)
    private float interval = 20f;

    // Счетчик активных объектов
    private static int activeObjectCount = 0;

    void OnEnable()
    {
        // Увеличиваем счетчик при активации объекта
        activeObjectCount++;
        Debug.Log("Активных объектов: " + activeObjectCount);
    }

    void OnDisable()
    {
        // Уменьшаем счетчик при деактивации объекта
        activeObjectCount--;
        Debug.Log("Активных объектов: " + activeObjectCount);
    }
    void Start()
    {
        
    }

    void Update()
    {
        // Увеличиваем прошедшее время на время прошедшее с момента последнего обновления
        elapsedTime += Time.deltaTime;

        // Если прошло достаточно времени для генерации числа
        if (elapsedTime >= interval)
        {
            // Генерируем случайное число от 0 до 100 (включительно)
            int randomNumber = Random.Range(0, 101);

            // Выводим число в консоль
            Debug.Log("Случайное число: " + randomNumber);

            // Обнуляем прошедшее время
            elapsedTime = 0f;
        }
    }
}
