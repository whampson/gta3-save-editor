namespace GTA3SaveEditor.Core
{
    public class VehicleModel : IdeObject
    {
        public string GameName { get; }

        public VehicleModel(short id, string modelName, string gameName)
            : base(id, modelName)
        {
            GameName = gameName;
        }

        //public override string ToString()
        //{
        //    return SaveEditor.GetGxtString(GameName);
        //}
    }
}
