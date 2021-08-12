namespace GTA3SaveEditor.Core.Game
{
    public class IdeObject
    {
        public short Id { get; }
        public string ModelName { get; }
        public ModelIndex ModelIndex => (ModelIndex) Id;

        public IdeObject(short id, string model)
        {
            Id = id;
            ModelName = model;
        }
    }
}
