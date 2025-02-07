using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// �����������ռ��ṩ�����������ļ���������
using System;
using System.IO;
using Unity.Collections;

public class FileDataHandler
{
    // �������� ������ļ�·��
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
        // Path.Combine ��.NET Framework �� System.IO.Path ���һ����̬����
        // ���ڽ�һ�������ַ���·����ϳ�һ����һ��·���ַ���
        // Path.Combine ��������һ���ַ�������ʾ���غ������·��
        string fullPath = Path.Combine(dataDirpath, dataFileName);

        try  // �� try û�гɹ����оͻ� ���� catch ����
        {
            // �˷������ fullPath ����ȡ��Ŀ¼������ Ҳ�������淽���е� dataDirpath ����Ӧ���ļ�Ŀ¼��
            // Directory.CreateDirectory ʹ����һ����ȡ����Ŀ¼·����������Ŀ¼(�������б������Ŀ¼)
            // ���Ŀ¼�Ѿ����ڣ��򲻻�ִ���κβ�����Ҳ�����׳��쳣
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // ���ڽ��������л�Ϊ JSON �ַ���
            // _data ������Ҫ���л��Ķ������������κ����͵����ݽṹ
            // true ���� false ����ָ���Ƿ��� (pretty pring) Ư���ĸ�ʽ ��� JSON �ַ���������������ͻ��з���ʹ��������Ķ�
            // JsonUtility.ToJson ����ֻ�����л������ֶκ����ԡ���������л�˽���ֶλ���������ʹ�� [System��Serializeble] ���Ա����
            string dataToStore = JsonUtility.ToJson(_data, true);

            if(encryptData)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // FileStream ���캯��
            // fullPath ���ļ�������·���������ļ���
            // FileMode.Create ��һ��ö��ֵ ָʾ����ļ��Ѵ����򸲸���������ļ��������򴴽����ļ�
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {   
                //ʹ�� using ���ȷ���ڴ�������ʱ�Զ��رպ��ͷ���Դ���ɷ�ֹ�ļ����й¶



                // StreamWriter ���캯��
                // stream ��ǰ�洴���� FileStream ���� StreamWriter ʹ����� �� ��д������
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    // dataToStore ������Ҫд���ļ����ַ�������
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

        if(File.Exists(fullPath)) // ����ļ��Ƿ����
        {
            try
            {
                string dataToLoad = "";

                // ����һ�� FileStream �����Դ��ļ���FlieMode.Open ��ʾ�����е��ļ�
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    // ʹ�� StreamReader ���ļ����ж�ȡ���е����ݣ�������洢�� dataToLoad �ַ���������
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if(encryptData)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // JsonUtility.FromJson ��������ȡ���� JSON �ַ��������л�Ϊ GameData ����
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

    private string EncryptDecrypt(string _data) // һ�� ���� �����ݵ� ����
    {
        string modifieData = "";

        for(int i = 0; i < _data.Length; i++)
        {
            modifieData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifieData;
    }

}
