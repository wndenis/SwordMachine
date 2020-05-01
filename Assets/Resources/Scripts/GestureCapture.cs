using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class GestureCapture : MonoBehaviour
{
	private int PointsCount = 5; //old: 12
	// 02.07.2018 Я затеял жуткий рефактор. Надеюсь, я останусь в живых. Меняем два трансформа на один меч
	/*
	 * Не стоит трогать.
	 * Количество пропускаемых fixedupdate перед занесением очередной вершины в дек
	 * Больше - траектория менее точная, зато длинная, реакция дольше (upd: реакция больше не меняется, она моментальна)
	 * Меньше - траектория более точная, но короткая, реакция быстрее (upd: реакция больше не меняется, она моментальна) (вроде как)
	 */
	private int FixedUpdateDelayCount = 6; //old: 5
	private float _delay = 0;

	public Sword Sword;
	public bool UseLines;
	public bool Locked;

	public Material TrailMaterial;
	public Color TipLineColor;
	public Color HandleLineColor;
	
	private readonly LinkedList<Vector3> _tipPoints = new LinkedList<Vector3>();
	private readonly LinkedList<Vector3> _handlePoints = new LinkedList<Vector3>();
	
	private LineRenderer _tipLineRenderer;
	private LineRenderer _handleLineRenderer;

	private void OnValidate()
	{
		var p = GetComponent<PlayableUnit>();
		if (!p)
			Debug.LogWarning($"{name}: GestureCapture has no PlayableUnit");
	}

	public void Init()
	{
		
		if (UseLines)
		{
			var tempObj = new GameObject("TipLineRendererObj");
			tempObj.transform.parent = transform;
			_tipLineRenderer = tempObj.AddComponent<LineRenderer>();
			_tipLineRenderer.material = TrailMaterial;
			_tipLineRenderer.positionCount = PointsCount;
			_tipLineRenderer.startColor = TipLineColor;
			_tipLineRenderer.endColor = TipLineColor;
			_tipLineRenderer.widthMultiplier = 0.1f;
			
			tempObj = new GameObject("HandleLineRendererObj");
			tempObj.transform.parent = transform;
			_handleLineRenderer = tempObj.AddComponent<LineRenderer>();
			_handleLineRenderer.material = TrailMaterial;
			_handleLineRenderer.positionCount = PointsCount;
			_handleLineRenderer.startColor = HandleLineColor;
			_handleLineRenderer.endColor = HandleLineColor;
			_handleLineRenderer.widthMultiplier = 0.1f;
		}
		for (var i = 0; i < PointsCount; i++)
			_tipPoints.AddLast(Vector3.zero);
		
		for (var i = 0; i < PointsCount; i++)
			_handlePoints.AddLast(Vector3.zero);
	}

	private void FixedUpdate()
	{
		if (Locked)
			return;
		if (_delay-- > 0)
			return;

		_delay = FixedUpdateDelayCount;

		if (Sword)
		{
			_handlePoints.AddLast(Sword.HandleTransform.position);
			_tipPoints.AddLast(Sword.TipTransform.position);
		}

		if (UseLines)
		{
			_tipLineRenderer.SetPositions(_tipPoints.ToArray());
			_handleLineRenderer.SetPositions(_handlePoints.ToArray());
		}
		
		if (_handlePoints.Count > PointsCount)
		{
			_handlePoints.RemoveFirst();
			_tipPoints.RemoveFirst();
		}
	}

	public LinkedList<Vector3> GetTrace()
	{
		return new LinkedList<Vector3>(_handlePoints.Concat(_tipPoints));
	}
}
