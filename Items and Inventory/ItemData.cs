using System.Text;
using UnityEngine;



// �� Build Game ʱ �ǲ����� ������Ϸʱ�� Assets ���ļ���ַ���������� ������Ϸ���ʲ���ַ ��������ȥ��
// �����Ҫ Ϊ����� UNITY_EDITOR ���������� 
// ��˼�� ��ʹ�� Unity �༭��ʱ���ô� using���ſ���ʹ�� �ýű���ͬ������������, �������ܱ����� Build ��Ϸʱ ��Ϊ���� using UnityEditor ����
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum ItemType
{
    Material,
    Equipment

}

//������Unity�༭���д����Զ�����Դ����ʹ��������Ա��һ����ʱ��Unity�������û�ͨ���Ҽ������Ŀ�����е� Assets �ļ��� 
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject // ����һ����������������������������ݣ��������������ʵ��
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemId;

    [Range(0, 100)] // ����һ�� ��Unity�п��Ա䶯�Ļ���
    public float dropChance;

    protected StringBuilder sb = new StringBuilder(); // StringBuilder ��һ�����������ɱ��ַ�������

    private void OnValidate()
    {
#if UNITY_EDITOR
        // private void OnValidate() ���� ֻ�� ��� Unity ����Ч �����㹹����Ϸʱ �Ͳ���ʹ�����������
        // AssetDatabase.GetAssetPath(this) ����������ص�ǰ�ű�������ڵ��ʲ���·����this �ǵ�ǰ�ű������ʵ��
        string path = AssetDatabase.GetAssetPath(this);

        // AssetDatabase.AssetPathToGUID(path) ����������ʲ�·��ת��Ϊȫ��Ψһ��ʶ�� (GUID) GUID ��һ��Ψһ���ַ���������Ψһ��ʶһ���ʲ�
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif 
    }

    public virtual string GetDescription()
    {
        return "";
    }
}
