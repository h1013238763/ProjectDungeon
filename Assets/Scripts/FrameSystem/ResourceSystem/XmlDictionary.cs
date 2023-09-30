using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class XmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    public XmlSchema GetSchema()
    {
        return (null);
    }

    /// <summary>
    /// Modified Dictionary Serilize Rule
    /// </summary>
    /// <param name="reader"></param>
    public void ReadXml(XmlReader reader)
    {
        XmlSerializer key_ser = new XmlSerializer(typeof(TKey));
        XmlSerializer value_ser = new XmlSerializer(typeof(TValue));

        reader.Read();

        while(reader.NodeType != XmlNodeType.EndElement)
        {
            TKey key = (TKey)key_ser.Deserialize(reader);
            TValue value = (TValue)value_ser.Deserialize(reader);
            this.Add(key, value);
        }
    }

    /// <summary>
    /// Modified Dictionary Deserilize Rule
    /// </summary>
    /// <param name="writer"></param>
    public void WriteXml(XmlWriter writer)
    {
        XmlSerializer key_ser = new XmlSerializer(typeof(TKey));
        XmlSerializer value_ser = new XmlSerializer(typeof(TValue));

        foreach(KeyValuePair<TKey, TValue> pair in this)
        {
            key_ser.Serialize(writer, pair.Key);
            value_ser.Serialize(writer, pair.Value);
        }
    }
}
