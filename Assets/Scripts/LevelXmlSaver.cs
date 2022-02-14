using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("LevelCollection")]
public class LevelXmlSaver 
{

    [XmlArray("Levels"), XmlArrayItem("Level")]
    public Level[] Levels;
    
    /// <summary>
    /// Загрузка данных
    /// </summary>
    /// <returns></returns>
    public static LevelXmlSaver Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        var serializer = new XmlSerializer(typeof(LevelXmlSaver));
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            return serializer.Deserialize(stream) as LevelXmlSaver;
        }
    }

    /// <summary>
    /// Сохранение данных
    /// </summary>
    public void Save(string filePath)
    {
        var serializer = new XmlSerializer(typeof(LevelXmlSaver));
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }
}
