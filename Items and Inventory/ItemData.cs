using System.Text;
using UnityEngine;



// 在 Build Game 时 是不允许将 制作游戏时的 Assets 等文件地址及其其他的 制作游戏的资产地址 给包括进去的
// 因此需要 为其加入 UNITY_EDITOR 等限制条件 
// 意思是 在使用 Unity 编辑器时启用此 using，才可以使用 该脚本下同样的限制内容, 这样就能避免在 Build 游戏时 因为存在 using UnityEditor 报错
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum ItemType
{
    Material,
    Equipment

}

//用于在Unity编辑器中创建自定义资源，当使用这个属性标记一个类时，Unity将允许用户通过右键点击项目窗口中的 Assets 文件夹 
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject // 这是一个数据容器，可用来保存大量数据，用来独立于类的实例
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemId;

    [Range(0, 100)] // 设置一个 在Unity中可以变动的滑块
    public float dropChance;

    protected StringBuilder sb = new StringBuilder(); // StringBuilder 是一个用来创建可变字符串的类

    private void OnValidate()
    {
#if UNITY_EDITOR
        // private void OnValidate() 方法 只在 你的 Unity 中有效 但当你构建游戏时 就不能使用这个功能了
        // AssetDatabase.GetAssetPath(this) 这个方法返回当前脚本组件所在的资产的路径，this 是当前脚本组件的实例
        string path = AssetDatabase.GetAssetPath(this);

        // AssetDatabase.AssetPathToGUID(path) 这个方法将资产路径转换为全局唯一标识符 (GUID) GUID 是一个唯一的字符串，用来唯一标识一个资产
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif 
    }

    public virtual string GetDescription()
    {
        return "";
    }
}
