using UnityEngine.UIElements;

namespace TM.UI
{
    public abstract class GenericUITemplateManager
    {
        protected VisualElement root; //available only to this class and Its childrens
        public VisualElement GetRoot() => root;
        public virtual void SetRoot(VisualElement root)
        {
            this.root = root;
        }
        public virtual void OnEnable() {}
        public virtual void OnDisable() {}
    }
}