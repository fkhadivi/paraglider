using UnityEngine;
using System.Collections;

public class BenjasMath : MonoBehaviour{

	

	public float restrict (float x, float limit_1, float limit_2){
		x = Mathf.Max(x, Mathf.Min(limit_1,limit_2));// x >= the lower limit
		x = Mathf.Min(x, Mathf.Max(limit_1,limit_2));// x <= the higer limit
		
		return x;
	}
	
	public Vector3 delayed_repositioning(Vector3 origin, Vector3 target, float delay){
		//delays the Movement from origin to target  to a maximum of "delay"
		
		if (delay >= 0) {
			target -= origin; //make target the vector from origin to target
					  
			
			if(target.magnitude > delay){
				return origin + delay * target.normalized;
			}
			else{
				return origin + target;
			}
		}
		return target;
	}

	public Vector3 VectorComponentProduct (Vector3 A ,Vector3 B )
	{
		A.x *= B.x;
		A.y *= B.y;
		A.z *= B.z;
		
		return A;
	}

	/*
       ▄▀▀■  █▄ ▄█  ▄▀▀▄  ▄▀▀▄  ▀▀█▀▀        █▀▀▀  █     ▄▀▀▄  ▄▀▀▄  ▀▀█▀▀ 
       ▀■■▄  █▀▄▀█  █  █  █  █    █          █■■   █     █  █  █■■█    █   
       ■▄▄▀  █ █ █  ▀▄▄▀  ▀▄▄▀    █          █     █▄▄▄  ▀▄▄▀  █  █    █   
	*/


public class smoothFloat{

	float[] array;
	
	public smoothFloat(int iterations, float startValue){
		
		if (iterations>0)
		{
			array = new float[iterations];
			for(int i = 0; i< iterations; i++){
				array[i]=startValue;
			}
		}
	}
	
	public float smooth(float newValue)
	{
	//use tis function to get the value smoothed
	
	
		if(array!=null){
			int iMax = array.Length-1;
			float temp = newValue;
			for (int i=0; i<iMax; i++) {
				temp += array[i];
				array[i]=array[i+1];
			}
			temp += array[iMax];
			array[iMax]=newValue;
			return temp /(iMax+2);
		}
		
		return newValue;
	}
}

public class smoothFloatFaster{
	
	float currentValue;
	int iteration;
	
	public smoothFloatFaster(int iterations, float startValue){
		currentValue = startValue;
		iteration = iterations;
	}
	
	public float smooth(float newValue)
	{
		//use tis function to get the value smoothed
		
		
		if(iteration>0){
				currentValue += (newValue-currentValue)/iteration;
			return currentValue;
		}
		
		return newValue;
	}
}

/*
 ▄▀▀■  █▄ ▄█  ▄▀▀▄  ▄▀▀▄  ▀▀█▀▀  █  █        █  █  █▀▀▀  ▄▀▀▄  ▀▀█▀▀  ▄▀▀▄  █▀▀▄ 
 ▀■■▄  █▀▄▀█  █  █  █  █    █    █■■█        █ █   █■■   █       █    █  █  █▀▀▄ 
 ■▄▄▀  █ █ █  ▀▄▄▀  ▀▄▄▀    █    █  █         █    █▄▄▄  ▀▄▄▀    █    ▀▄▄▀  █  █ 
*/



public class smoothVector3{
	
	Vector3 currentValue;
	int iteration;
	
	public smoothVector3(int iterations, Vector3 startValue){
		currentValue = startValue;
		iteration = iterations;
	}
	
	public Vector3 smooth(Vector3 newValue)
	{
		//use tis function to get the value smoothed
		
		if(iteration>0){
			//currentValue = (currentValue*iteration+newValue)*1f/(iteration+1);
			currentValue += (newValue - currentValue)*1f/(iteration);
			return currentValue;
		}
		
		return newValue;
	}
}

/*
 ▄▀▀■  █▄ ▄█  ▄▀▀▄  ▄▀▀▄  ▀▀█▀▀  █  █        █▀▀▄  ▄▀▀▄  ▀▀█▀▀  ▄▀▀▄  ▀▀█▀▀  ▀█▀  ▄▀▀▄  █▄ █ 
 ▀■■▄  █▀▄▀█  █  █  █  █    █    █■■█        █▀▀▄  █  █    █    █■■█    █     █   █  █  █▀▄█ 
 ■▄▄▀  █ █ █  ▀▄▄▀  ▀▄▄▀    █    █  █        █  █  ▀▄▄▀    █    █  █    █    ▄█▄  ▀▄▄▀  █ ▀█ 
*/

	public class smoothAngle{
		
		float currentValue;
		int iteration;
		
		public smoothAngle(int iterations, float startValue){
			currentValue = startValue;
			iteration = iterations;
		}
		
		public float smooth(float newValue)
		{
			//use tis function to get the value smoothed
			
			if(iteration>0){
				currentValue += Mathf.DeltaAngle(currentValue,newValue)/iteration;
				return currentValue;
			}
			
			return newValue;
		}
	}
	
	public class smoothRotation{
		
		Vector3 currentValue;
		int iteration;
		
		public smoothRotation(int iterations, Vector3 startValue){
			currentValue = startValue;
			iteration = iterations;
		}
		
		public Vector3 smooth(Vector3 newValue)
		{
			//use tis function to get the value smoothed
			
			if(iteration>0){
				currentValue.x += Mathf.DeltaAngle(currentValue.x,newValue.x)*1/iteration;
				currentValue.y += Mathf.DeltaAngle(currentValue.y,newValue.y)*1/iteration;
				currentValue.z += Mathf.DeltaAngle(currentValue.z,newValue.z)*1/iteration;
				return currentValue;
			}
			
			return newValue;
		}
	}

	public static Vector3 angularLerp(Vector3 from, Vector3 to, float t)
	{
		return new Vector3(	Mathf.LerpAngle(from.x,to.x,t),
							Mathf.LerpAngle(from.y,to.y,t),
							Mathf.LerpAngle(from.z,to.z,t));
	}

	public static float smooth01(float t)
	{
		return Mathf.Clamp01(Mathf.SmoothStep(0,1,t));
	}

	public static float smooth01trigonometrically(float t)
	{
		//uses cosinus shape between, input 0..1, output 0..1
		//return Mathf.Clamp01((1-Mathf.Cos(t*Mathf.PI))/2);
		return Mathf.Clamp01(Mathf.InverseLerp(1,-1, Mathf.Cos(t*Mathf.PI)));
	}


	public int[] randomIntArray(int min, int max, int count)
	{
		if(count>max-min)
		{
			Debug.LogError("randomIntArray Error, count of elements is bigger than Interval: "+count+">"+max+"-"+min+", just gave back array from min to max-1");
			count = max-min-1;
		}

		int[] ints = new int[count];
		for(int i=0;i<ints.Length;i++)
		{
			ints[i]=Random.Range(min,max-(count-i)+1);
			min=ints[i]+1;
			Debug.Log("randomInt count: "+count+" int nr "+i +" "+ints[i]);
		}
		return ints;
	}

	public static void randomizeArray(GameObject[] Array)
	{
	    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
			for (int t = 0; t < Array.Length; t++ )
	        {
					GameObject tmp = Array[t];
					int r = Random.Range(t, Array.Length);
					Array[t] = Array[r];
					Array[r] = tmp;
	        }
	    }
		
	}

	static public GameObject[] cleanUpArray(GameObject[] Array)
	{
		
		int j=0;
		for(int i = 0;i<Array.Length;i++)
		{
			if (Array[i]!=null)
			{
				if(j<i)
				{
					Array[j]=Array[i];
					Array[i]=null;
				}
				j++;
			}
		}
		GameObject[] Output = new GameObject[j];
		for(int i = 0;i<Output.Length;i++)
		{
			Output[i]=Array[i];
		}
		return Output;
	}

	public static Vector3 ClosestPointToIntersect( Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){

		float a = Vector3.Dot(lineVec1, lineVec1);
		float b = Vector3.Dot(lineVec1, lineVec2);
		float e = Vector3.Dot(lineVec2, lineVec2);
 
		float d = a*e - b*b;
 
		
		if(d == 0.0f)
		{
			//lines are parallel, return center between line points
			return (linePoint1+linePoint2)*0.5f;
		}
		else
		{
 
			Vector3 r = linePoint1 - linePoint2;
			float c = Vector3.Dot(lineVec1, r);
			float f = Vector3.Dot(lineVec2, r);
 
			float s = (b*f - c*e) / d;
			float t = (a*f - c*b) / d;
 
			return ((linePoint1 + lineVec1 * s)+(linePoint2 + lineVec2 * t))*0.5f;
		}
	}

    public static bool countdownToZero(ref float currentTime)
    {
        if (currentTime == 0)
        {
            return true;
        }
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            return true;
        }
        return false;
    }

}