using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TrafficSystemBuilder : Singleton<TrafficSystemBuilder>
{
	private enum RoadAttachmentPoint
	{
		NORTH = 0,
		EAST = 1,
		SOUTH = 2,
		WEST = 3,
		MAX = 4
	}

	// Start is called before the first frame update
	public static bool createRoads = false;
    public const int lenghtStraightLine = 8;
	public Vector3 offsetLine = new Vector3(-90f, 0f, 90f);

    public TrafficSystemPiece prefabStraight2Line;
	public TrafficSystemPiece prefabT2Intersection;
	public TrafficSystemPiece prefabX2Intersection;
	public GameObject fakeStraightPrefab;

	private float lengthOfStraight2Line = 8f;
	private int trafficSystemPiecesCount = 0;
	private RoadAttachmentPoint roadAttachmentPointIndex = RoadAttachmentPoint.NORTH;

	
	public void TestOrDebug()
    {

    }

	public void GenerateTrafficSystem(dynamic nodes, dynamic edges)
    {
		Vector2 normalPos = new Vector2(-1, -1);
		Vector2 currentPos = new Vector2(-1, -1);
		Vector2 endPos = new Vector2(-1, -1);

		foreach (dynamic node in nodes)
		{
			normalPos = new Vector2((float)node.Value.GetValue("x").Value, (float)node.Value.GetValue("y").Value);
			break;
		}

   //     foreach (dynamic node in nodes)
   //     {
   //         currentPos = new Vector2(((float)node.Value.GetValue("x") - normalPos.x), ((float)node.Value.GetValue("y").Value - normalPos.y)) * Mathf.Pow(10, TrafficSystem.Instance.m_roadScale + 2f);
			//CreateRoad(prefabStraight2Line, currentPos.x, currentPos.y, -1);
   //     }

  //      foreach (dynamic edge in edges)
  //      {
		//	currentPos = new Vector2( (float) nodes[edge.Name]["x"] - normalPos.x, (float)nodes[edge.Name]["y"] - normalPos.y) * Mathf.Pow(10, TrafficSystem.Instance.m_roadScale + 2f);
		//	string endNode = edge.Value.GetValue("end").ToString();
		//	endPos = new Vector2( (float) nodes[endNode]["x"] - normalPos.x, (float) nodes[endNode]["y"] - normalPos.y) * Mathf.Pow(10, TrafficSystem.Instance.m_roadScale + 2f);
		//	string name = CreateLine( Mathf.Sqrt(Mathf.Pow(currentPos.x - endPos.x, 2) + Mathf.Pow(currentPos.y - endPos.y, 2)), new Vector3(currentPos.x, currentPos.y, (float) edge.Value.GetValue("angle")));
		//	nodes[edge.Name]["name_piece"] = name;
		//}

		foreach (dynamic node in nodes)
        {
			//if(node.Value.GetValue("highway") == "traffic_signals")
   //         {

   //         }
			foreach(dynamic edge in edges)
            {
				if(edge.Name == node.Name)
                {
					currentPos = new Vector2((float)nodes[edge.Name]["x"] - normalPos.x, (float)nodes[edge.Name]["y"] - normalPos.y) * Mathf.Pow(10, TrafficSystem.Instance.m_roadScale + 2f);
					string endNode = edge.Value.GetValue("end").ToString();
					endPos = new Vector2((float)nodes[endNode]["x"] - normalPos.x, (float)nodes[endNode]["y"] - normalPos.y) * Mathf.Pow(10, TrafficSystem.Instance.m_roadScale + 2f);
					string name = CreateLine(LenghtOfTwoNodes(currentPos, endPos), edge.Name, new Vector3(currentPos.x, currentPos.y, (float)edge.Value.GetValue("angle")), new Vector2(endPos.x, endPos.y));
					nodes[edge.Name]["name_piece"] = name;
				}
            }
        }
	}

    public string CreateLine(float lenght, string nameNode, Vector3 beginPos, Vector2 endPos)
    {
		GameObject previousFakePiece = null;
		GameObject fakePiece = null;
		string head = "";

		int count = Mathf.RoundToInt(lenght / (TrafficSystem.Instance.m_roadScale * lengthOfStraight2Line)) - 1;

		if ((((endPos.x - beginPos.x) * Mathf.Cos(beginPos.z)  > 0 && beginPos.z > Mathf.PI) && ((endPos.y - beginPos.y) * Mathf.Sin(beginPos.z) > 0 && beginPos.z > Mathf.PI)) || (((endPos.x - beginPos.x) * Mathf.Cos(beginPos.z) < 0 && beginPos.z < Mathf.PI) && ((endPos.y - beginPos.y) * Mathf.Sin(beginPos.z) < 0 && beginPos.z < Mathf.PI))) beginPos.z -= Mathf.PI;

		if (count >= 0) head = CreateRoad(prefabStraight2Line, nameNode, beginPos.x, beginPos.y, beginPos.z);

		count--;

		if (count >= 0)
		{
			previousFakePiece = Instantiate(fakeStraightPrefab);
			CreateFakeRoad(previousFakePiece, TrafficSystem.Instance.AnchorTrafficSystemPiece);
		}

		for (int i = 0; i < count; i++)
        {
			fakePiece = Instantiate(fakeStraightPrefab);
			CreateFakeRoad(fakePiece, previousFakePiece);
			previousFakePiece = fakePiece;
		}

		return head;
	}

	// МОРДОР
	// МОРДОР
	// МОРДОР
	// МОРДОР
	// МОРДОР
	// Не лезь, убьет!!!!
	// МОРДОР
	// МОРДОР
	// Не лезь, убьет!!!!
	// МОРДОР
	// МОРДОР
	// МОРДОР
	// МОРДОР
	// МОРДОР
	// МОРДОР


	public void CreateFakeRoad(GameObject fakePiece, GameObject attachPiece)
    {
		if (fakePiece)
        {
			fakePiece.name = trafficSystemPiecesCount + " - " + fakePiece.name;
			fakePiece.transform.position = Vector3.zero;
			fakePiece.transform.rotation = Quaternion.identity;
			fakePiece.transform.localScale = TrafficSystem.Instance.m_roadScale * fakePiece.transform.localScale;
			trafficSystemPiecesCount++;

			PositionTrafficSystemFakePiece(fakePiece, attachPiece, false);

			fakePiece.transform.parent = TrafficSystem.Instance.transform;

		}
    }

	public void CreateFakeRoad(GameObject fakePiece, TrafficSystemPiece attachPiece)
    {
		if (fakePiece)
		{
			fakePiece.name = trafficSystemPiecesCount + " - " + fakePiece.name;
			fakePiece.transform.position = Vector3.zero;
			fakePiece.transform.rotation = Quaternion.identity;
			fakePiece.transform.localScale = TrafficSystem.Instance.m_roadScale * fakePiece.transform.localScale;
			trafficSystemPiecesCount++;

			PositionTrafficSystemFakePiece(fakePiece, attachPiece, false);

			fakePiece.transform.parent = TrafficSystem.Instance.transform;

		}
	}

	public string CreateRoad(TrafficSystemPiece prefabTrafficSystemPiece, string nameNode, float posX = -1f, float posY = -1f, float angle = -1f)
	{
		TrafficSystemPiece roadPiece = prefabTrafficSystemPiece;
		string name = "";

		if (roadPiece)
		{
			TrafficSystemPiece roadPieceClone = Instantiate(roadPiece) as TrafficSystemPiece;
			if (roadPieceClone)
			{
				roadPieceClone.name = trafficSystemPiecesCount + " - " + roadPieceClone.name;

				name = roadPieceClone.name;

				roadPieceClone.m_nameNode = nameNode;
				//							Vector3 posOffset = roadPieceClone.m_posOffset;
				//							if(AnchorTrafficSystemPiece)
				//							{
				//								Vector3 dir = AnchorTrafficSystemPiece.transform.position - roadPieceClone.transform.position;
				//								dir         = dir.normalized;
				//								posOffset.x = posOffset.x * dir.x; 
				//								posOffset.y = posOffset.y * dir.y; 
				//								posOffset.z = posOffset.z * dir.z; 
				//							}

				roadPieceClone.transform.position = Vector3.zero;
				roadPieceClone.transform.rotation = Quaternion.identity;
				roadPieceClone.transform.localScale = TrafficSystem.Instance.m_roadScale * roadPieceClone.transform.localScale;
				trafficSystemPiecesCount++;

				TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, roadPieceClone);
				if (TrafficSystem.Instance)
					TrafficSystem.Instance.ShowTooltip(TrafficSystem.TrafficSystemTooltip.EDIT, true);

				if (TrafficSystem.Instance.AnchorTrafficSystemPiece)
				{
					PositionTrafficSystemPiece(TrafficSystem.Instance.EditTrafficSystemPiece, TrafficSystem.Instance.AnchorTrafficSystemPiece, false);
				}
				else
				{
					TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystem.Instance.EditTrafficSystemPiece);
				}

				if (TrafficSystem.Instance.m_autoLinkOnSpawn)
				{
					CreateAllLinks(true);
				}

				// position
				if (TrafficSystem.Instance)
				{
					UpdateEditTrafficSystemPiecePos(posX, posY, angle);
					roadPieceClone.transform.parent = TrafficSystem.Instance.transform;


					// set the quality of the road
					if (TrafficSystem.Instance && TrafficSystem.Instance.m_spawnWithRoadQuality)
						roadPieceClone.ProcessRoadQuality(TrafficSystem.Instance.GetRoadQualityVal());
				}

				if (TrafficSystem.Instance.m_quickSpawn)
				{
					TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystem.Instance.EditTrafficSystemPiece);
					TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, null);
					if (TrafficSystem.Instance)
						TrafficSystem.Instance.ShowTooltip(TrafficSystem.TrafficSystemTooltip.EDIT, false);
				}
			}
		}

		SaveRoad();

		return name;
	}

	void PositionTrafficSystemFakePiece(GameObject a_currentPiece, GameObject a_attachToPiece, bool a_incIndex = true)
    {
		if (!a_currentPiece)
			return;

		if (!a_attachToPiece)
			return;

		if (a_incIndex)
			roadAttachmentPointIndex++;

		if (roadAttachmentPointIndex == RoadAttachmentPoint.MAX)
			roadAttachmentPointIndex = RoadAttachmentPoint.NORTH;

		a_currentPiece.transform.position = a_attachToPiece.transform.position;

		a_currentPiece.transform.eulerAngles += new Vector3 (offsetLine.x, 0, 0);
		a_currentPiece.transform.eulerAngles += new Vector3(0f, a_attachToPiece.transform.eulerAngles.y, 0f);

		Vector3 pos = a_attachToPiece.transform.position;

		Renderer m_currentRenderer = a_currentPiece.GetComponent<Renderer>(); 
		Renderer m_attachRenderer = a_attachToPiece.GetComponent<Renderer>();

		if (m_currentRenderer && m_currentRenderer)
		{
			m_currentRenderer.transform.position = m_attachRenderer.transform.position;
			pos = m_attachRenderer.transform.position;
		}

		float roadPieceSize = 8;

		switch (roadAttachmentPointIndex)
		{
			case RoadAttachmentPoint.EAST:
				{
					//if (m_attachRenderer && m_currentRenderer)
					//{
					//	float anchorSize = m_attachRenderer.bounds.extents.x;
					//	if (TrafficSystem.Instance.m_swapAnchorDimensions)
					//		anchorSize = m_attachRenderer.bounds.extents.z;

					//	float currentSize = m_currentRenderer.bounds.extents.x;
					//	if (TrafficSystem.Instance.m_swapEditDimensions)
					//		currentSize = m_currentRenderer.bounds.extents.z;

					//	roadPieceSize = anchorSize + currentSize;
					//}
					//pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);

				}
				break;
			case RoadAttachmentPoint.SOUTH:
				{
					if (m_attachRenderer && m_currentRenderer)
					{
						float angle = a_attachToPiece.transform.eulerAngles.y;
						a_attachToPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

						float anchorSize = a_attachToPiece.GetComponent<Renderer>().bounds.extents.x;
						if (TrafficSystem.Instance.m_swapAnchorDimensions)
							anchorSize = m_attachRenderer.bounds.extents.z;

						a_attachToPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

						angle = a_currentPiece.transform.eulerAngles.y;
						a_currentPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

						float currentSize = a_currentPiece.GetComponent<Renderer>().bounds.extents.x;
						if (TrafficSystem.Instance.m_swapEditDimensions)
							currentSize = m_currentRenderer.bounds.extents.z;

						a_currentPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

						roadPieceSize = currentSize + anchorSize;
					}
					//pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					pos.z = m_attachRenderer.transform.position.z - roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
				}
				break;
			case RoadAttachmentPoint.WEST:
				{
					//if (m_attachRenderer && m_currentRenderer)
					//{
					//	float angle = a_attachToPiece.transform.eulerAngles.y;
					//	a_attachToPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

					//	float anchorSize = a_attachToPiece.GetComponent<Renderer>().bounds.extents.x;
					//	if (TrafficSystem.Instance.m_swapAnchorDimensions)
					//		anchorSize = m_attachRenderer.bounds.extents.z;

					//	a_attachToPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

					//	angle = a_currentPiece.transform.eulerAngles.y;
					//	a_currentPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

					//	float currentSize = a_currentPiece.GetComponent<Renderer>().bounds.extents.x;
					//	if (TrafficSystem.Instance.m_swapEditDimensions)
					//		currentSize = m_currentRenderer.bounds.extents.z;

					//	a_currentPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

					//	roadPieceSize = currentSize + anchorSize;
					//}
					////pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					////pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.z = m_attachRenderer.transform.position.z - roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
				}
				break;

			
			case RoadAttachmentPoint.NORTH:
				{
					if (m_attachRenderer && m_currentRenderer)
					{
						float angle = a_attachToPiece.transform.eulerAngles.y;
						a_attachToPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

						float anchorSize = a_attachToPiece.GetComponent<Renderer>().bounds.extents.x;
						if (TrafficSystem.Instance.m_swapAnchorDimensions)
							anchorSize = m_attachRenderer.bounds.extents.z;

						a_attachToPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

						angle = a_currentPiece.transform.eulerAngles.y;
						a_currentPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

						float currentSize = a_currentPiece.GetComponent<Renderer>().bounds.extents.x;
						if (TrafficSystem.Instance.m_swapEditDimensions)
							currentSize = m_currentRenderer.bounds.extents.z;

						a_currentPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

						roadPieceSize = currentSize + anchorSize;
					}
					//pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					pos.x = m_attachRenderer.transform.position.x - roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
				}
				break;
		}

		a_currentPiece.transform.position = pos;
	}

	void PositionTrafficSystemFakePiece(GameObject a_currentPiece, TrafficSystemPiece a_attachToPiece, bool a_incIndex = true)
	{
		if (!a_currentPiece)
			return;

		if (!a_attachToPiece)
			return;

		if (a_incIndex)
			roadAttachmentPointIndex++;

		if (roadAttachmentPointIndex == RoadAttachmentPoint.MAX)
			roadAttachmentPointIndex = RoadAttachmentPoint.NORTH;

		a_currentPiece.transform.position = a_attachToPiece.transform.position;

		a_currentPiece.transform.eulerAngles += offsetLine;
		a_currentPiece.transform.eulerAngles += new Vector3(0f, a_attachToPiece.transform.eulerAngles.y, 0f);

		Vector3 pos = a_attachToPiece.transform.position;

		Renderer m_currentRenderer = a_currentPiece.GetComponent<Renderer>();
		Renderer m_attachRenderer = a_attachToPiece.m_renderer;

		if (m_currentRenderer && m_currentRenderer)
		{
			m_currentRenderer.transform.position = m_attachRenderer.transform.position;
			pos = m_attachRenderer.transform.position;
		}

		float roadPieceSize = 8;

		switch (roadAttachmentPointIndex)
		{
			case RoadAttachmentPoint.EAST:
				{
					//if (m_attachRenderer && m_currentRenderer)
					//{
					//	float anchorSize = m_attachRenderer.bounds.extents.x;
					//	if (TrafficSystem.Instance.m_swapAnchorDimensions)
					//		anchorSize = m_attachRenderer.bounds.extents.z;

					//	float currentSize = m_currentRenderer.bounds.extents.x;
					//	if (TrafficSystem.Instance.m_swapEditDimensions)
					//		currentSize = m_currentRenderer.bounds.extents.z;

					//	roadPieceSize = anchorSize + currentSize;
					//}
					//pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);

				}
				break;
			case RoadAttachmentPoint.SOUTH:
				{
					if (m_attachRenderer && m_currentRenderer)
					{
						float angle = a_attachToPiece.transform.eulerAngles.y;
						a_attachToPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

						float anchorSize = a_attachToPiece.m_renderer.bounds.extents.z;
						if (TrafficSystem.Instance.m_swapAnchorDimensions)
							anchorSize = m_attachRenderer.bounds.extents.x;

						a_attachToPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

						angle = a_currentPiece.transform.eulerAngles.y;
						a_currentPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

						float currentSize = a_currentPiece.GetComponent<Renderer>().bounds.extents.x;
						if (TrafficSystem.Instance.m_swapEditDimensions)
							currentSize = m_currentRenderer.bounds.extents.z;

						a_currentPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

						roadPieceSize = currentSize + anchorSize;
					}
					//pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					pos.z = m_attachRenderer.transform.position.z - roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
				}
				break;
			case RoadAttachmentPoint.WEST:
				{
					//if (m_attachRenderer && m_currentRenderer)
					//{
					//	float angle = a_attachToPiece.transform.eulerAngles.y;
					//	a_attachToPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

					//	float anchorSize = a_attachToPiece.m_renderer.bounds.extents.z;
					//	if (TrafficSystem.Instance.m_swapAnchorDimensions)
					//		anchorSize = m_attachRenderer.bounds.extents.x;

					//	a_attachToPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

					//	angle = a_currentPiece.transform.eulerAngles.y;
					//	a_currentPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

					//	float currentSize = a_currentPiece.GetComponent<Renderer>().bounds.extents.x;
					//	if (TrafficSystem.Instance.m_swapEditDimensions)
					//		currentSize = m_currentRenderer.bounds.extents.z;

					//	a_currentPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

					//	roadPieceSize = currentSize + anchorSize;
					//}
					////pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					////pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.z = m_attachRenderer.transform.position.z - roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
				}
				break;

			
			case RoadAttachmentPoint.NORTH:
				{
					if (m_attachRenderer && m_currentRenderer)
					{
                        float angle = a_attachToPiece.transform.eulerAngles.y;
                        a_attachToPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

                        float anchorSize = a_attachToPiece.m_renderer.bounds.extents.z;
                        if (TrafficSystem.Instance.m_swapAnchorDimensions)
                            anchorSize = m_attachRenderer.bounds.extents.x;

                        a_attachToPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

                        angle = a_currentPiece.transform.eulerAngles.y;
                        a_currentPiece.transform.eulerAngles -= new Vector3(0f, angle, 0f);

                        float currentSize = a_currentPiece.GetComponent<Renderer>().bounds.extents.x;
                        if (TrafficSystem.Instance.m_swapEditDimensions)
                            currentSize = m_currentRenderer.bounds.extents.z;

                        a_currentPiece.transform.eulerAngles += new Vector3(0f, angle, 0f);

						roadPieceSize = currentSize + anchorSize;
					}
					//pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					//pos.x = m_attachRenderer.transform.position.x + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					pos.x = m_attachRenderer.transform.position.x - roadPieceSize * Mathf.Cos(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
					pos.z = m_attachRenderer.transform.position.z + roadPieceSize * Mathf.Sin(a_currentPiece.transform.eulerAngles.y * Mathf.PI / 180);
				}
				break;
		}

		a_currentPiece.transform.position = pos;

	}

	void PositionTrafficSystemPiece(TrafficSystemPiece a_currentPiece, TrafficSystemPiece a_attachToPiece, bool a_incIndex = true, bool a_useEditOffset = false, bool a_useAnchorOffset = false)
	{
		if (!a_currentPiece)
			return;

		if (!a_attachToPiece)
			return;

		if (a_incIndex)
			roadAttachmentPointIndex++;

		if (roadAttachmentPointIndex == RoadAttachmentPoint.MAX)
			roadAttachmentPointIndex = RoadAttachmentPoint.NORTH;

		TrafficSystem.Instance.EditTrafficSystemPiece.transform.position = a_attachToPiece.transform.position;

		if (TrafficSystem.Instance.m_quickSpawn)
			TrafficSystem.Instance.EditTrafficSystemPiece.transform.rotation = a_attachToPiece.transform.rotation;

		Vector3 pos = a_attachToPiece.transform.position;

		if (a_currentPiece.m_renderer && a_attachToPiece.m_renderer)
		{
			TrafficSystem.Instance.EditTrafficSystemPiece.transform.position = a_attachToPiece.m_renderer.transform.position;
			//			EditTrafficSystemPiece.transform.rotation = a_attachToPiece.m_renderer.transform.rotation;
			pos = a_attachToPiece.m_renderer.transform.position;
		}

		float roadPieceSize = 8;

		switch (roadAttachmentPointIndex)
		{
			case RoadAttachmentPoint.EAST:
				{
					if (a_attachToPiece.m_renderer && a_currentPiece.m_renderer)
					{
						float anchorSize = a_attachToPiece.GetRenderBounds().extents.x;
						if (TrafficSystem.Instance.m_swapAnchorDimensions)
							anchorSize = a_attachToPiece.GetRenderBounds().extents.z;

						float currentSize = a_currentPiece.GetRenderBounds().extents.x;
						if (TrafficSystem.Instance.m_swapEditDimensions)
							currentSize = a_currentPiece.GetRenderBounds().extents.z;

						roadPieceSize = anchorSize + currentSize;
					}
					pos.x = a_attachToPiece.m_renderer.transform.position.x + roadPieceSize;
				}
				break;
			case RoadAttachmentPoint.SOUTH:
				{
					if (a_attachToPiece.m_renderer && a_currentPiece.m_renderer)
					{
						float anchorSize = a_attachToPiece.GetRenderBounds().extents.z;
						if (TrafficSystem.Instance.m_swapAnchorDimensions)
							anchorSize = a_attachToPiece.GetRenderBounds().extents.x;

						float currentSize = a_currentPiece.GetRenderBounds().extents.z;
						if (TrafficSystem.Instance.m_swapEditDimensions)
							currentSize = a_currentPiece.GetRenderBounds().extents.x;

						roadPieceSize = anchorSize + currentSize;
					}
					pos.z = a_attachToPiece.m_renderer.transform.position.z - roadPieceSize;
				}
				break;
			case RoadAttachmentPoint.WEST:
				{
					if (a_attachToPiece.m_renderer && a_currentPiece.m_renderer)
					{
						float anchorSize = a_attachToPiece.GetRenderBounds().extents.x;
						if (TrafficSystem.Instance.m_swapAnchorDimensions)
							anchorSize = a_attachToPiece.GetRenderBounds().extents.z;

						float currentSize = a_currentPiece.GetRenderBounds().extents.x;
						if (TrafficSystem.Instance.m_swapEditDimensions)
							currentSize = a_currentPiece.GetRenderBounds().extents.z;

						roadPieceSize = anchorSize + currentSize;
					}
					pos.x = a_attachToPiece.m_renderer.transform.position.x - roadPieceSize;
				}
				break;
			case RoadAttachmentPoint.NORTH:
				{
					if (a_attachToPiece.m_renderer && a_currentPiece.m_renderer)
					{
						float anchorSize = a_attachToPiece.GetRenderBounds().extents.z;
						if (TrafficSystem.Instance.m_swapAnchorDimensions)
							anchorSize = a_attachToPiece.GetRenderBounds().extents.x;

						float currentSize = a_currentPiece.GetRenderBounds().extents.z;
						if (TrafficSystem.Instance.m_swapEditDimensions)
							currentSize = a_currentPiece.GetRenderBounds().extents.x;

						roadPieceSize = anchorSize + currentSize;
					}
					pos.z = a_attachToPiece.m_renderer.transform.position.z + roadPieceSize;
				}
				break;
		}

		Vector3 posOffset = a_currentPiece.m_posOffset;
		if (a_useAnchorOffset)
			posOffset = a_attachToPiece.m_posOffset;

		Vector3 dir = a_attachToPiece.transform.position - pos;

		if (a_attachToPiece.m_renderer)
			dir = a_attachToPiece.m_renderer.transform.position - pos;

		dir = dir.normalized;

		if (a_useEditOffset || a_useAnchorOffset)
		{
			//			if(TrafficSystem.Instance.m_swapOffsetDir)
			//			{
			//				if(TrafficSystem.Instance.m_swapOffsetSize)
			//				{
			//					posOffset.x = posOffset.z * dir.z; 
			//					posOffset.y = posOffset.y * dir.y; 
			//					posOffset.z = posOffset.x * dir.x; 
			//				}
			//				else
			//				{
			//					posOffset.x = posOffset.x * dir.z; 
			//					posOffset.y = posOffset.y * dir.y; 
			//					posOffset.z = posOffset.z * dir.x; 
			//				}
			//			}
			//			else
			//			{
			if (TrafficSystem.Instance.m_swapOffsetSize)
			{
				float x = posOffset.x;
				posOffset.x = posOffset.z;
				posOffset.y = posOffset.y;
				posOffset.z = x;
			}
			else
			{
				posOffset.x = posOffset.x;
				posOffset.y = posOffset.y;
				posOffset.z = posOffset.z;
			}
			//			}

			if (TrafficSystem.Instance.m_negateOffsetSize)
			{
				posOffset.x = -posOffset.x;
				posOffset.y = -posOffset.y;
				posOffset.z = -posOffset.z;
			}

			a_currentPiece.transform.position = pos + posOffset;
		}
		else
			a_currentPiece.transform.position = pos;

		UpdateEditTrafficSystemPiecePos(-1, -1, 0);
	}

	private void UpdateEditTrafficSystemPiecePos(float posX, float posY, float angle)
	{
		TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, TrafficSystem.Instance.EditTrafficSystemPiece); // force reposition of edit icon

		if (posX != -1 && posY != -1)
		{
			TrafficSystem.Instance.EditTrafficSystemPiece.transform.position = new Vector3(posX, TrafficSystem.Instance.EditTrafficSystemPiece.transform.position.y, posY);
			TrafficSystem.Instance.EditTrafficSystemPiece.transform.eulerAngles = new Vector3(TrafficSystem.Instance.EditTrafficSystemPiece.transform.eulerAngles.x, FromPI(angle), TrafficSystem.Instance.EditTrafficSystemPiece.transform.eulerAngles.z);
		}
	}

	private void CreateAllLinks(bool a_anchorToEdit, bool a_revealOnly = false, bool a_linkSameLaneOnly = false)
	{
		TrafficSystem.Instance.ClearCLRevealObjsFrom();
		TrafficSystem.Instance.ClearCLRevealObjsTo();

		if (TrafficSystem.Instance.EditTrafficSystemPiece != TrafficSystem.Instance.AnchorTrafficSystemPiece)
		{
			if (TrafficSystem.Instance.m_autoReverseAnchorToEdit)
				a_anchorToEdit = !a_anchorToEdit;

			if (IsOffRampPiece(TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryLeftLaneNodes) ||
			   IsOffRampPiece(TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryRightLaneNodes) ||
			   IsOffRampPiece(TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryLeftLaneNodes) ||
			   IsOffRampPiece(TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryRightLaneNodes))
			{
				if (a_anchorToEdit)
				{
					CreateOffRampConnection(TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryLeftLaneNodes, a_revealOnly);
					CreateOffRampConnection(TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryRightLaneNodes, TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryRightLaneNodes, a_revealOnly);
				}
				else
				{
					CreateOffRampConnection(TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryLeftLaneNodes, TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, a_revealOnly);
					CreateOffRampConnection(TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryRightLaneNodes, TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryRightLaneNodes, a_revealOnly);
				}
			}
			else
			{
				if (a_anchorToEdit)
				{
					CreateConnections(TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly);
					CreateConnections(TrafficSystem.Instance.AnchorTrafficSystemPiece.m_secondaryLeftLaneNodes, TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly);
					CreateConnections(TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryRightLaneNodes, TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly);
					CreateConnections(TrafficSystem.Instance.EditTrafficSystemPiece.m_secondaryRightLaneNodes, TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly);
				}
				else
				{
					CreateConnections(TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryLeftLaneNodes, TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly);
					CreateConnections(TrafficSystem.Instance.EditTrafficSystemPiece.m_secondaryLeftLaneNodes, TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryLeftLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly);
					CreateConnections(TrafficSystem.Instance.AnchorTrafficSystemPiece.m_primaryRightLaneNodes, TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly);
					CreateConnections(TrafficSystem.Instance.AnchorTrafficSystemPiece.m_secondaryRightLaneNodes, TrafficSystem.Instance.EditTrafficSystemPiece.m_primaryRightLaneNodes, false, 0, a_revealOnly, true, false, a_linkSameLaneOnly);
				}
			}
		}
	}

	private bool IsOffRampPiece(List<TrafficSystemNode> a_nodes)
	{
		bool isOfframp = false;
		for (int lNIndex = 0; lNIndex < a_nodes.Count; lNIndex++)
		{
			TrafficSystemNode lastNode = a_nodes[lNIndex];

			if (lastNode.m_roadType == TrafficSystem.RoadType.OFFRAMP)
			{
				isOfframp = true;
				break;
			}
		}

		return isOfframp;
	}

	private void CreateOffRampConnection(List<TrafficSystemNode> a_nodesLookingToConnect, List<TrafficSystemNode> a_nodesToConnectWith, bool a_revealOnly = false)
	{
		for (int lNIndex = 0; lNIndex < a_nodesLookingToConnect.Count; lNIndex++)
		{
			TrafficSystemNode lastNode = a_nodesLookingToConnect[lNIndex];

			for (int cNIndex = 0; cNIndex < a_nodesToConnectWith.Count; cNIndex++)
			{
				TrafficSystemNode currentNode = a_nodesToConnectWith[cNIndex];

				if (lastNode.m_driveSide != currentNode.m_driveSide)
					continue;

				if (lastNode.m_lane == 0 && currentNode.m_lane == 0)
				{
					if (a_revealOnly)
					{
						TrafficSystem.Instance.AddToCLRevealObjsFrom(lastNode.transform);
						TrafficSystem.Instance.AddToCLRevealObjsTo(currentNode.transform);
					}
					else
					{
						lastNode.AddConnectedNode(currentNode);
					}
				}
			}
		}
	}

	private void CreateConnections(List<TrafficSystemNode> a_nodesLookingToConnect, List<TrafficSystemNode> a_nodesToConnectWith, bool a_isOutbound = false, int a_nodeID = 0, bool a_revealOnly = false, bool a_useConnectedNodeArray = false, bool a_clearReveal = true, bool a_linkSameLaneOnly = false)
	{
		if (a_clearReveal)
		{
			TrafficSystem.Instance.ClearCLRevealObjsFrom();
			TrafficSystem.Instance.ClearCLRevealObjsTo();
		}

		for (int lNIndex = 0; lNIndex < a_nodesLookingToConnect.Count; lNIndex++)
		{
			TrafficSystemNode lastNode = a_nodesLookingToConnect[lNIndex];

			if (a_isOutbound && (lastNode.m_isInbound || (int)lastNode.m_changeLaneID != a_nodeID))
				continue;

			for (int cNIndex = 0; cNIndex < a_nodesToConnectWith.Count; cNIndex++)
			{
				TrafficSystemNode currentNode = a_nodesToConnectWith[cNIndex];

				bool nodeFound = false;
				for (int cIndex = 0; cIndex < lastNode.m_connectedNodes.Count; cIndex++)
				{
					TrafficSystemNode connectedNode = lastNode.m_connectedNodes[cIndex];

					if (connectedNode == currentNode)
					{
						nodeFound = true;
						break;
					}
				}

				if (!nodeFound)
				{
					if (a_linkSameLaneOnly || (currentNode.m_onlyConnectWithSameLane || lastNode.m_onlyConnectWithSameLane))
					{
						if (lastNode.m_lane == currentNode.m_lane)
						{
							if (a_revealOnly)
							{
								TrafficSystem.Instance.AddToCLRevealObjsFrom(lastNode.transform);
								TrafficSystem.Instance.AddToCLRevealObjsTo(currentNode.transform);
							}
							else
							{
								if (a_useConnectedNodeArray)
									lastNode.AddConnectedNode(currentNode);
								else
									lastNode.AddChangeLaneNode(currentNode);
							}
						}
					}
					else
					{
						if (a_revealOnly)
						{
							TrafficSystem.Instance.AddToCLRevealObjsFrom(lastNode.transform);
							TrafficSystem.Instance.AddToCLRevealObjsTo(currentNode.transform);
						}
						else
						{
							if (a_useConnectedNodeArray)
								lastNode.AddConnectedNode(currentNode);
							else
								lastNode.AddChangeLaneNode(currentNode);
						}
					}
				}
			}
		}
	}

	private void SaveRoad()
    {
		if (TrafficSystem.Instance)
		{
			TrafficSystem.Instance.ClearCLRevealObjsFrom();
			TrafficSystem.Instance.ClearCLRevealObjsTo();
			TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.ANCHOR, TrafficSystem.Instance.EditTrafficSystemPiece);
			TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, null);
			TrafficSystem.Instance.ShowTooltip(TrafficSystem.TrafficSystemTooltip.EDIT, false);
			//m_roadAttachmentPointIndex = RoadAttachmentPoint.SOUTH;
		}
	}

	public void SelectRoadAsEdit(TrafficSystemPiece objectEdit)
    {
        if (TrafficSystem.Instance)
        {
            TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.EDIT, objectEdit);
            TrafficSystem.Instance.ShowTooltip(TrafficSystem.TrafficSystemTooltip.EDIT, true);
        }
    }

	public void SelectRoadAsAnchor(TrafficSystemPiece objectAnchor)
    {
        if (TrafficSystem.Instance)
        {
            TrafficSystem.Instance.SetTrafficSystemPiece(TrafficSystem.TrafficSystemTooltip.ANCHOR, objectAnchor);
            TrafficSystem.Instance.ShowTooltip(TrafficSystem.TrafficSystemTooltip.ANCHOR, true);
        }
    }

	public void SetRoadsScale(float scale)
    {
		if (TrafficSystem.Instance)
        {
			TrafficSystem.Instance.m_roadScale = scale;

		}
    }

    private float ToPI(float value) => value * Mathf.PI / 180;

	private float FromPI(float value) => value * 180 / Mathf.PI;

	private float LenghtOfTwoNodes(Vector2 value1, Vector2 value2) => Mathf.Sqrt(Mathf.Pow(value1.x - value2.x, 2) + Mathf.Pow(value1.y - value2.y, 2));

}
