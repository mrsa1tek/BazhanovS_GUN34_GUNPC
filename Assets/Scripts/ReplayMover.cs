using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
            ////todo comment: зачем нужны эти проверки?
            ////!TryGetComponent(out _save) - это условие гарантирует, что текущий GameObject имеет необходимый нам компонет _save
            ////_save.Records.Count == 0 - условиеи, для того, чтобы понять, что в компоненте есть данные для обработки
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
				//todo comment: Для чего выключается этот компонент?
				//Таким способом мы обработаем ошибку, пренебрегая выброса исключенния

				enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
			//todo comment: Что проверяет это условие (с какой целью)? 
			// Проверяем, прошло ли время текущей записи, таким образом, мы определяем, когда нужно переключиться на след. запись
			if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
				//todo comment: Для чего нужна эта проверка?
				//Если записи закончились, отключаем компонент enabled = false, и сообщаем об окончании в консоль
				if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
			//todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
			// Это производится для плавности между точками вместо резких скачков

			var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
			//todo comment: Зачем нужна эта проверка?
			//Без этого может появиться визуальный артефакт, может сделать объект невидимым. Поэтому предварительно устанавливаем значение 0f, это гарантирует, что объект останется в начальной позиции
			if (float.IsNaN(delta)) delta = 0f;
			//todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
			//Сначла идет вычисления между конечной и текущей позицией, после чего умножаем эту разницу на прогресс(параметр интерполяции delta). Добавляем к начальной точке прогресс
			transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}