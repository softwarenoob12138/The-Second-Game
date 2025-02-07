using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 这两个命名空间提供了许多基础和文件操作功能
using System;
using System.IO;
using Unity.Collections;

public class FileDataHandler
{
    // 用于设置 保存的文件路径
    private string dataDirpath = "";
    private string dataFileName = "";

    private bool encryptData = false;
    private string codeWord = "fuck you";

    public FileDataHandler(string _dataDirpath, string _dataFileName, bool _encryptData)
    {
        dataDirpath = _dataDirpath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
        
    }

    public void Save(GameData _data)
    {
        // Path.Combine 是.NET Framework 中 System.IO.Path 类的一个静态方法
        // 用于将一个或多个字符串路径组合成一个单一的路径字符串
        // Path.Combine 方法返回一个字符串，表示返回后的完整路径
        string fullPath = Path.Combine(dataDirpath, dataFileName);

        try  // 当 try 没有成功运行就会 调用 catch 报错
        {
            // 此方法会从 fullPath 中提取出目录名部分 也就是上面方法中的 dataDirpath 所对应的文件目录名
            // Directory.CreateDirectory 使用上一步获取到的目录路径来创建该目录(及其所有必须的子目录)
            // 如果目录已经存在，则不会执行任何操作，也不会抛出异常
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // 用于将对象序列化为 JSON 字符串
            // _data 是你想要序列化的对象，它可以是任何类型的数据结构
            // true 或者 false 用于指定是否以 (pretty pring) 漂亮的格式 输出 JSON 字符串，其包含缩进和换行符，使其更易于阅读
            // JsonUtility.ToJson 方法只能序列化公共字段和属性。如果想序列化私有字段或属，可以使用 [System。Serializeble] 属性标记类
            string dataToStore = JsonUtility.ToJson(_data, true);

            if(encryptData)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // FileStream 构造函数
            // fullPath 是文件的完整路径，包括文件名
            // FileMode.Create 是一个枚举值 指示如果文件已存在则覆盖它，如果文件不存在则创建新文件
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {   
                //使用 using 语句确保在代码块结束时自动关闭和释放资源，可防止文件句柄泄露



                // StreamWriter 构造函数
                // stream 是前面创建的 FileStream 对象 StreamWriter 使用这个 流 来写入数据
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    // dataToStore 是你想要写入文件的字符串数据
                    writer.Write(dataToStore);
                }
            }
        }

        catch(Exception e)
        {
            Debug.Log("yes");
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirpath, dataFileName);
        GameData loadData = null;

        if(File.Exists(fullPath)) // 检查文件是否存在
        {
            try
            {
                string dataToLoad = "";

                // 创建一个 FileStream 对象以打开文件，FlieMode.Open 表示打开现有的文件
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    // 使用 StreamReader 从文件流中读取所有的内容，并将其存储在 dataToLoad 字符串变量中
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if(encryptData)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // JsonUtility.FromJson 方法将读取到的 JSON 字符串反序列化为 GameData 对象
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log("Good");
            }
        }

        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirpath, dataFileName);

        if(File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private string EncryptDecrypt(string _data) // 一个 加密 你数据的 方法
    {
        string modifieData = "";

        for(int i = 0; i < _data.Length; i++)
        {
            modifieData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifieData;
    }

}
