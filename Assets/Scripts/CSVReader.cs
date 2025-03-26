/* 
 * 내용: CSV파일을 읽는 용도
 * CSV파일의 기본위치는 GameManager 스크립트에서 변경가능
 * Ver 0.0.1
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
 
public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };
 
    public static List<Dictionary<string, object>> Read(string file, string part = "")
    {
        var list = new List<Dictionary<string, object>>();
        string partEndText = "end";
        string AllText = "";
        int partNum = 5;
        bool isList = false;
        bool isAllList = false;


        //주석처리된건 Resources를 이용하지 않는 방식임
        //근데 그 대신 StreamingAssets이용하든가 빌드후 넣어두던가 해야함
        TextAsset data = Resources.Load (file) as TextAsset;
        //string data = File.ReadAllText(GameManager.instance.CSVFolder.ToString() + file +".csv");

        var lines = Regex.Split (data.text, LINE_SPLIT_RE);
        //var lines = Regex.Split (data, LINE_SPLIT_RE);

        if(part == AllText)
        { 
            isList = true;
            isAllList = true;
        }

 
        if(lines.Length <= 1)
        {     
            return list;
        }

        var header = Regex.Split(lines[0], SPLIT_RE);

        for(var i=1; i < lines.Length; i++) 
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if(values.Length == 0 ||values[0] == "")
            {     
                continue;
            }
 
            var entry = new Dictionary<string, object>();
            for(var j=0; j < header.Length && j < values.Length; j++ ) 
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if(int.TryParse(value, out n)) 
                {
                    finalvalue = n;
                } 
                else if (float.TryParse(value, out f)) 
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            //Debug.LogWarning(entry[header[3]]);

            if(isList == false)
            { 
                if(entry[header[partNum]].ToString() == part)
                { 
                    isList = true;    
                }
            }

            if(isList == true)
            { 
                list.Add (entry);    
                
                if((entry[header[partNum]].ToString() == partEndText) && (isAllList == false))
                { 
                    isList = false; 
                }
            }      
            
        }
        return list;
    }
}
