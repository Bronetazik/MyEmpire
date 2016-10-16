using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public static float CAMERA_Z_ROTATION = -20.0f; 
    public float coef; // коэфициент сили скольжения камеры

    [HideInInspector] public bool isReplace; // отключение движения камеры
    [HideInInspector] public bool needEdgeMove; // отключение камеры, когда включается Hand (например посадка грядок)

    private RaycastHit hit;
    private Vector3 point;
    private Ray ray;

    // переменные расчета позиции
    private Vector3 tempPosition; 
    private Vector3 tempCameraPosition; 
    private Vector3 newTemp;
    private Vector3 start;
    private Vector3 finish;
    private Vector3 force;
    private Vector3 direction;
    private Vector3 lastVec = Vector3.zero;
    private float traceTime = 0;
    private float lastTraceTime = 0;
    private float speed = 0;
    private float prev_speed;
	private float cameraWidth;
	private float cameraHeight;

    public Vector2 clampTopLeft; // ограничение позиции камеры по верхнему левому углу
    public Vector2 clampRightBottom; // ограничение камеры по нижнему правому углу

	private IEnumerator fade_move = null;

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        pcControlTest ();
        #else
        mobileControl();
        #endif

        if (needEdgeMove)
            Move();
    }

    private float t = 0;
	private void pcControlTest()
	{
		t += Time.deltaTime;

		if (isReplace)
		{
			if (fade_move != null)
				StopCoroutine(fade_move);

			tempPosition = Input.mousePosition;
			lastVec = Vector3.zero;
			traceTime = 0;
			lastTraceTime = 0;
			
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
            if (fade_move != null)
			{
				StopCoroutine(fade_move);
			}

			tempPosition = Input.mousePosition;
			lastVec = Vector3.zero;
			traceTime = 0;
			lastTraceTime = 0;
		}

		if (Input.GetMouseButton(0) && t > 0.01f)
		{
            t = 0;

			Vector3 v1 = Vector3.zero;
			Vector3 v2 = Vector3.zero;

			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("World")))
				v1 = hit.point;

			if (tempPosition == Vector3.zero)
				tempPosition = Input.mousePosition;

			ray = Camera.main.ScreenPointToRay(tempPosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("World")))
				v2 = hit.point;

			var v = v1 - v2; 
			v = -v;
			v.z = 0;

			lastVec = v;
			lastTraceTime = traceTime;
			traceTime = 0;
		
			Camera.main.transform.localPosition += v;
			Camera.main.transform.localPosition = new Vector3(Mathf.Clamp(Camera.main.transform.localPosition.x, clampTopLeft.x + cameraWidth, clampRightBottom.x - cameraWidth),
															  Mathf.Clamp(Camera.main.transform.localPosition.y, clampRightBottom.y + cameraHeight, clampTopLeft.y - cameraHeight),
															  Camera.main.transform.localPosition.z);

			tempPosition = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp(0))
		{
            if (lastVec != Vector3.zero && lastTraceTime != 0)
			{
				float speed = lastVec.magnitude / lastTraceTime;
				if (speed > 5f)
				{
					lastVec = lastVec.normalized * 5f;
					lastTraceTime = 1f;
				}

				fade_move = fadeMove(lastVec, lastTraceTime);
				StartCoroutine(fade_move);
			}

			lastVec = Vector3.zero;
			traceTime = 0;
			lastTraceTime = 0;
		}

		traceTime += Time.deltaTime;
	}

	// замедляем ход после освобождения контроллера 
	private IEnumerator fadeMove(Vector3 v, float speed)
	{
		v = v.normalized;
		float t = 0.8f;
		float dsp = speed / t;

		while (t > 0)
		{
			var tv = v * speed * Time.deltaTime;
			tv.z = 0;

			Camera.main.transform.localPosition += tv;
			Camera.main.transform.localPosition = new Vector3(Mathf.Clamp(Camera.main.transform.localPosition.x, clampTopLeft.x + cameraWidth, clampRightBottom.x - cameraWidth),
															  Mathf.Clamp(Camera.main.transform.localPosition.y, clampRightBottom.y + cameraHeight, clampTopLeft.y - cameraHeight),
															  Camera.main.transform.localPosition.z);

			speed -= dsp * Time.deltaTime;
			t -= Time.deltaTime;

			yield return null;
		}
	}

	// контролирует свайпы на тачпаде
	private void mobileControl()
	{
		if (isReplace)
		{
			tempPosition = Vector3.zero;
			if (fade_move != null)
				StopCoroutine(fade_move);

			return;
		}

		//if (WindowManager.instance.openWindows.Count > 0)
		//{
		//	tempPosition = Vector3.zero;
		//	return;
		//}

		if (Input.touchCount == 1)
		{
			// получаем тач
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Began)
			{
				if (fade_move != null)
					StopCoroutine(fade_move);

				tempPosition = touch.position;
				speed = prev_speed = 0;
				traceTime = 0;
				//traceDist = 0;
			}

			if (touch.phase == TouchPhase.Moved)
			{
				Vector3 v1 = Vector3.zero;
				Vector3 v2 = Vector3.zero;

				if (tempPosition == Vector3.zero)
					tempPosition = touch.position;

				var ray = Camera.main.ScreenPointToRay(touch.position);
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("World")))
					v1 = hit.point;
				else
					return;
				
				ray = Camera.main.ScreenPointToRay(tempPosition);
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("World")))
					v2 = hit.point;
				else 
					return;

				if (v1 == v2)
					return;

				// двигаем камеру
				Vector3 v = v1 - v2;
				v = -v;
				v.z = 0;

				lastVec = v;
				prev_speed = speed;
				if (traceTime != 0)
					speed = v.magnitude / traceTime;

				traceTime = 0;

				Camera.main.transform.localPosition += v;
				Camera.main.transform.localPosition = new Vector3(Mathf.Clamp(Camera.main.transform.localPosition.x, clampTopLeft.x + cameraWidth, clampRightBottom.x - cameraWidth),
																  Mathf.Clamp(Camera.main.transform.localPosition.y, clampRightBottom.y + cameraHeight, clampTopLeft.y - cameraHeight),
																  Camera.main.transform.localPosition.z);

				tempPosition = touch.position;
			}

			if (touch.phase == TouchPhase.Stationary)
			{
				speed = prev_speed = 0;
				traceTime = 0;
			}

			if (touch.phase == TouchPhase.Ended)
			{
				if (lastVec != Vector3.zero)
				{
					fade_move = fadeMove(lastVec, prev_speed / 3f);
					StartCoroutine(fade_move);
				}

				lastVec = Vector3.zero;
				speed = prev_speed = 0;
				tempPosition = touch.position;
			}

			traceTime += Time.deltaTime;
		}
	}

	// двигаем камеру когда дошли до края
	private void MoveEdge() 
	{
        Move();
    }

    static float cameraEdgedMinValue = 0.25f;
    static float MaxEdgeSpeed = 45;
    [Header("Edge Settings")]
    public EdgeVariables edge;
    public EdgeVariables unitEdge;

    private void Move() //непосредственно движение камеры у края экрана
	{
		// определяем нужно ли нам двигатся
		var mp = Input.mousePosition;
		var vp = Camera.main.ScreenToViewportPoint(mp);

        if (vp.x > cameraEdgedMinValue && vp.x < 1 - cameraEdgedMinValue &&
            vp.y > cameraEdgedMinValue && vp.y < 1 - cameraEdgedMinValue)
        {
            return;
        }

        //coeff in [0-cameraEdgedMinValue]
        Vector2 currentCoeffMin = new Vector2(vp.x, vp.y);
        if (vp.x > cameraEdgedMinValue)
        {
            currentCoeffMin.x -= 1 - cameraEdgedMinValue;
        }
        if (vp.y > cameraEdgedMinValue)
        {
            currentCoeffMin.y -= 1 - cameraEdgedMinValue;
        }
        //inverse
        if (vp.x < cameraEdgedMinValue)
        {
            currentCoeffMin.x = Mathf.Abs(currentCoeffMin.x - cameraEdgedMinValue);
        }
        if (vp.y < cameraEdgedMinValue)
        {
            currentCoeffMin.y = Mathf.Abs(currentCoeffMin.y - cameraEdgedMinValue);
        }


        if (vp.x > cameraEdgedMinValue && vp.x < 1 - cameraEdgedMinValue)
        {
            currentCoeffMin.x = 0;
        }
        if (vp.y > cameraEdgedMinValue && vp.y < 1 - cameraEdgedMinValue)
        {
            currentCoeffMin.y = 0;
        }

        Vector2 currentEdgeSpeed = MaxEdgeSpeed * currentCoeffMin;

        //set move
        Vector2 move = Vector2.one;
        if (vp.x < cameraEdgedMinValue)
        {
            move.x = -1;
        }
        if (vp.y < cameraEdgedMinValue)
        {
            move.y = -1;
        }

        Vector3 tempPos = new Vector3(
            transform.position.x + move.x * currentEdgeSpeed.x * Time.deltaTime,
            transform.position.y + move.y * currentEdgeSpeed.y * Time.deltaTime,
            transform.position.z
            );

        float xCoord = tempPos.x;
        float yCoord = tempPos.y;

        //check X Coord Border
        if (xCoord < edge.EdgeXmin)
        {
            xCoord = edge.EdgeXmin;
        }
        else if (xCoord > edge.EdgeXmax)
        {
            xCoord = edge.EdgeXmax;
        }

        //check Y Coord Border
        if (yCoord < edge.EdgeYmin)
        {
            yCoord = edge.EdgeYmin;
        }
        else if (yCoord > edge.EdgeYmax)
        {
            yCoord = edge.EdgeYmax;
        }

        Vector3 newPos = new Vector3(xCoord, yCoord, tempPos.z);

        //set new pos for camera
        this.transform.position = newPos;
    }        

	public void FindCameraSize()
	{
		cameraHeight = Mathf.Abs(Camera.main.transform.localPosition.z) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
		cameraWidth = cameraHeight * Camera.main.aspect;
	}

	public float CameraWidth {
		get {
			return cameraWidth;
		}
	}

	public float CameraHeight {
		get {
			return cameraHeight;
		}
	}
}

[System.Serializable]
public class EdgeVariables
{
    public float EdgeXmin;
    public float EdgeXmax;
    public float EdgeYmin;
    public float EdgeYmax;
}