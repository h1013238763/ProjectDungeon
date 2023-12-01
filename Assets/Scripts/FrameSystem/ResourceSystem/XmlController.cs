using System.Net;
using System.Net.Mime;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

/// <summary>
/// XML file writer and reader class
/// </summary>
public class XmlController : BaseController<XmlController>
{

    /// <summary>
    /// Save data to XML File
    /// </summary>
    /// <param name="data">data object</param>
    /// <param name="file_name">file name</param>
    /// <param name="dir">the sub folder</param>
    public void SaveData(object data, string file_name, string dir = "")
    {
        Debug.Log(data);
        // get the save path
        string path = Application.persistentDataPath + "/" + dir;
        // path checking
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);
        path += "/" + file_name + ".xml";

        // create a writer and serialize
        using(StreamWriter writer = new StreamWriter(path))
        {
            XmlSerializer s = new XmlSerializer(data.GetType());
            s.Serialize(writer, data);
        }
    }

    /// <summary>
    /// read data from xml file
    /// </summary>
    /// <param name="type">object type</param>
    /// <param name="file_name">name of file</param>
    /// <param name="dir">the sub folder</param>
    /// <returns></returns>
    public object LoadData(Type type, string file_name, string dir = "")
    {
        // try to find file in two paths
        string path = Application.persistentDataPath + "/" + dir + file_name + ".xml";
        if(!File.Exists(path))
            path = Application.streamingAssetsPath + "/" + dir + file_name + ".xml";
        if(!File.Exists(path))
            path = dir + file_name + ".xml";
        if(!File.Exists(path))
            return null;  // return a default file if not found
        
        // create a reader and deserialize
        try{
            using (StreamReader reader = new StreamReader(path))
            {
                XmlSerializer s = new XmlSerializer(type);
                return s.Deserialize(reader);
            }
        }
        // if fail read file, return null and ask for create new file
        catch( Exception )
        {
            return null;
        }
    }

    public object DeserializeFile(Type type, TextAsset file)
    {
        if(file == null)
            return null;

        try{
            using (StringReader reader = new StringReader(file.text))
            {
                XmlSerializer s = new XmlSerializer(type);
                return s.Deserialize(reader);
            }
        }
        // if fail read file, return null and ask for create new file
        catch( Exception )
        {
            return null;
        }
    }

    /// <summary>
    /// delete file by given name
    /// </summary>
    /// <param name="file_name">name of file</param>
    /// <param name="dir">the sub folder</param>
    public void DeleteData(string file_name, string dir = "")
    {
        string path = Application.persistentDataPath + "/" + dir + file_name + ".xml";

        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
