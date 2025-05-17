using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		public struct Data
		{
			public Vector3 Position;
			public float Time;
		}

		private TextAsset _json;

		public List<Data> Records { get; private set; }

		private void Awake()
		{
			//todo comment: Что будет, если в теле этого условия не сделать выход из метода?
			// Без этого условия, все последующие оперцаии будут пытаться работать с _json=null
			if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			JsonUtility.FromJsonOverwrite(_json.text, this);
			//todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
			//искючает ситуации, когда код будет пытаться работать с неинициализированным списком
			if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
			//todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
			//Если Record неинициализирован, то обращение к нему вызовет критическую ошибку
			//Если список пуст, то нет смысла проводить дальнешие операции над ним
			if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
			//todo comment: Почему итерация начинается не с нулевого элемента?
			//Потому что нулевой элемент уже обработан до цикла, в переменную prev
			for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			//todo comment: Что происходит в этой строке?
			//Собирается полный путь к файлу в папку Assets прокета +
			//Создается или перезаписывается текстовый файл Path.txt, открываем поток FileStream для записи данных в этот файл(File.Create() возвращает FileStream), после чего сохраняем ссылку на поток в stream.
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав)
			//Она освобождает ресурсы, связанные с файловым потоком)
			//Если закомментить, то каждый незакрытый поток будет занимать память, файл останется заблоченным для других операций, возникнет исключение при попытке повторного доступа.
			//stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					return;
				}
			}
		}

		private void OnDestroy()
		{
			//todo logic...
		}
#endif
	}
}