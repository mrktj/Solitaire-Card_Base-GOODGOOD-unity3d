using UnityEngine;
using System.Collections.Generic;
using Facebook.MiniJSON;

public class FacebookResponse
{
	Dictionary<string, object> dataDict;
	List<object> dataList;
	string dataString;

	FacebookResponse(object data)
	{
		if (data is Dictionary<string, object>)
		{
			dataDict = data as Dictionary<string, object>;
		}
		else if (data is List<object>)
		{
			dataList = data as List<object>;
		}
		else
		{
			dataString = data as string;
		}
	}

	public static FacebookResponse Parse(FBResult result)
	{
		object data = Json.Deserialize(result.Text);
		FacebookResponse response = new FacebookResponse(data);
		return response;
	}

	public FacebookResponse this[string key] { get { return new FacebookResponse(dataDict[key]); } }
	public FacebookResponse this[int index] { get { return new FacebookResponse(dataList[index]); } }
	public string Value { get { return dataString; } }

	public int Count
	{
		get
		{
			if (dataDict != null) return dataDict.Count;
			else if (dataList != null) return dataList.Count;
			else return 0;
		}
	}

	public bool ContainsKey(string key)
	{
		return dataDict.ContainsKey(key);
	}
}
